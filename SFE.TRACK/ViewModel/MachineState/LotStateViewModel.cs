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
    public class LotStateViewModel : ViewModelBase
    {
        public RelayCommand LotCreateRelayCommand { get; set; }
        public RelayCommand LotClearRelayCommand { get; set; }
        public RelayCommand<object> CloseRelayCommand { get; set; }

        public LotStateViewModel()
        {
            LotCreateRelayCommand = new RelayCommand(LotCreateCommand);
            LotClearRelayCommand = new RelayCommand(LotClearCommand);
            CloseRelayCommand = new RelayCommand<object>(CloseCommand);
        }

        private void LotCreateCommand()
        {
            RunStatus.EnumRunningStatus runStatus = Global.MachineWorker.GetController("SFETrack").MyRunStatus;
            EnumLotStatus lotStatus = Global.MachineWorker.GetController("SFETrack").GetLot().GetLotStatus("SFETrack", out var result);
            if (lotStatus == EnumLotStatus.Idle && (runStatus == RunStatus.EnumRunningStatus.Stop || runStatus == RunStatus.EnumRunningStatus.Idle)) Global.MachineWorker.GetController("SFETrack").LotCreate();
        }

        private void LotClearCommand()
        {
            RunStatus.EnumRunningStatus runStatus = Global.MachineWorker.GetController("SFETrack").MyRunStatus;
            EnumLotStatus lotStatus = Global.MachineWorker.GetController("SFETrack").GetLot().GetLotStatus("SFETrack", out var result);
            if ((lotStatus == EnumLotStatus.Idle || lotStatus == EnumLotStatus.Booked || lotStatus == EnumLotStatus.Running) && (runStatus == RunStatus.EnumRunningStatus.Stop || runStatus == RunStatus.EnumRunningStatus.Idle)) 
                Global.MachineWorker.GetController("SFETrack").LotClear();
        }
        private void CloseCommand(object o)
        {
            Window window = o as Window;
            window.DialogResult = true;
        }
    }
}
