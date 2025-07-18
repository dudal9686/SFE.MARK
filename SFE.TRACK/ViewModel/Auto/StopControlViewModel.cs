﻿using System;
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
            //Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.Cassette__Stop, "");
            //Global.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Machine__Run, "Stop");
            Global.STMachineStatus = enMachineStatus.STOP;
            o.DialogResult = true;
        }
        private void RecoveryCommand(Window o)
        {
            //Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Machine__Run, "RecoveryStop");
            //Global.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Machine__Run, "RecoveryStop");
            //Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__Stop, "RecoveryStop");
            //Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.Cassette__Stop, "");
            Global.STMachineStatus = enMachineStatus.STOP;
            o.DialogResult = true;
        }
        private void CancelCommand(Window o)
        {
            o.DialogResult = false;
        }
    }
}
