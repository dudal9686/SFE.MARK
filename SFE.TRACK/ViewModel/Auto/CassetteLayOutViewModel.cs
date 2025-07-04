﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Auto
{
    public class CassetteLayOutViewModel : ViewModelBase
    {
        bool isCheck = false;
        public RelayCommand<string> CassetteScanRelayCommand { get; set; }
        public CassetteLayOutViewModel()
        {
            CassetteScanRelayCommand = new RelayCommand<string>(CassetteScanCommand);
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; 
                
            foreach(WaferCls wafer in Global.STWaferList)
            {
                if (IsCheck) wafer.Diplay = wafer.Recipe.Name;
                else wafer.Diplay = string.Format("{0}-{1}", wafer.ModuleNo, wafer.Index);
            }

            RaisePropertyChanged("IsCheck"); }
        }

        public void CassetteScanCommand(string cst)
        {
            string message = string.Empty; ;
            if (Global.STMachineStatus == enMachineStatus.STOP)
            {
                //message = string.Format("AssyCRA:DoScanCassette {0}", cst);
                //Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, message, true, 10000);

                Global.MachineWorker.SendCommand(CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.Cassette__ScanSet, cst);
                Global.MachineWorker.GetController("SFETrack").StartMachine();
            }
            //message = string.Format("Cassette:{0}", cst);
            //Global.SendCommand(Global.MCS_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Cassette__Scan, message);
        }
    }
}
