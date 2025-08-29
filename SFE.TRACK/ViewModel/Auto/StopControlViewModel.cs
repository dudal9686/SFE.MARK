using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Windows;
using CoreCSBase.IPC;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Auto
{
    public class StopControlViewModel : ViewModelBase
    {
        public RelayCommand<Window> StopRelayCommand { get; set; }
        public RelayCommand<Window> RecoveryRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }

        public StopControlViewModel()
        {
            StopRelayCommand = new RelayCommand<Window>(StopCommand);
            RecoveryRelayCommand = new RelayCommand<Window>(RecoveryCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
        }

        private void StopCommand(Window o)
        {
            Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.MCS__WorkStop, "Stop");
            Global.STMachineStatus = enMachineStatus.STOP;
            o.DialogResult = true;
        }
        private void RecoveryCommand(Window o)
        {
            Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.MCS__WorkStop, "Cancel");
            Global.STMachineStatus = enMachineStatus.STOP;
            o.DialogResult = true;
        }
        private void CancelCommand(Window o)
        {
            o.DialogResult = false;
        }
    }
}
