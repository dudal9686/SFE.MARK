using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CoreCSRunSim;

namespace SFE.TRACK.ViewModel.MachineState
{
    public class RobotStateViewModel : ViewModelBase
    {
        public RelayCommand RunRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand<object> CloseRelayCommand { get; set; }

        public RobotStateViewModel()
        {
            RunRelayCommand = new RelayCommand(RunCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            CloseRelayCommand = new RelayCommand<object>(CloseCommand);
        }
        private void RunCommand()
        {
            RunStatus.EnumRunningStatus runStatus = Global.MachineWorker.GetController("SFETrack").MyRunStatus;

            if (runStatus == RunStatus.EnumRunningStatus.Stop || runStatus == RunStatus.EnumRunningStatus.Idle)
                Global.MachineWorker.GetController("SFETrack").StartMachine();
        }
        private void StopCommand()
        {
            RunStatus.EnumRunningStatus runStatus = Global.MachineWorker.GetController("SFETrack").MyRunStatus;

            if (runStatus == RunStatus.EnumRunningStatus.Run)
                Global.MachineWorker.GetController("SFETrack").StopMachine();
        }
        private void CloseCommand(object o)
        {
            Window window = o as Window;
            window.DialogResult = true;
        }
    }
}
