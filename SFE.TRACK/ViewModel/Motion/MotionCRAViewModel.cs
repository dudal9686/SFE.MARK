﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using CoreCSBase.IPC;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Motion
{
    public class MotionCRAViewModel : ViewModelBase
    {
        bool[] isCheck = new bool[4] { true, false, false, false };
        public List<Model.IODataCls> DIList { get; set; }
        public List<Model.IODataCls> DOList { get; set; }
        int cstIndex = 1;
        public RelayCommand CstIndexRelayCommand { get; set; }
        public RelayCommand PickMotionRelayCommand { get; set; }
        public RelayCommand PlaceMotionRelayCommand { get; set; }
        public RelayCommand RobotChangeMotionRelayCommand { get; set; }

        public MotionCRAViewModel()
        {
            CstIndexRelayCommand = new RelayCommand(CstIndexCommand);
            PickMotionRelayCommand = new RelayCommand(PickMotionCommand);
            PlaceMotionRelayCommand = new RelayCommand(PlaceMotionCommand);
            RobotChangeMotionRelayCommand = new RelayCommand(RobotChangeMotionCommand);
            DIList = Global.STDIList.FindAll(x => x.BlockNo == 1).OrderBy(x => x.IONum).ToList();
            DOList = Global.STDOList.FindAll(x => x.BlockNo == 1).OrderBy(x => x.IONum).ToList();
        }
        public bool[] IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("IsCheck"); }
        }
        public int CstIndex
        {
            get { return cstIndex; }
            set { cstIndex = value; RaisePropertyChanged("CstIndex"); }
        }

        private void CstIndexCommand()
        {
            CstIndex = Convert.ToInt32(Global.KeyPad(CstIndex));

            if (CstIndex < 1) CstIndex = 1;
            if (CstIndex > 25) CstIndex = 25;
        }
        private void PickMotionCommand()
        {
            int cstNo = GetCstNo();
            string msg = string.Format("AssyCRA:DoPickDropWaferOnCassette {0},{1},{2}", 0, cstNo, CstIndex - 1);
            Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, msg,true,20000);
            //Global.ManualMessageOpen();
        }
        private void PlaceMotionCommand()
        {
            int cstNo = GetCstNo();
            string msg = string.Format("AssyCRA:DoPickDropWaferOnCassette {0},{1},{2}", 1, cstNo, CstIndex - 1);
            Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, msg, true, 20000);
            //Global.ManualMessageOpen();
        }
        private void RobotChangeMotionCommand()
        {
            Global.SendCommand(Global.MCS_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, "AssyCRA:DoRobotMoveToPRA,", true, 20000);
        }
        private int GetCstNo()
        {
            int moduleIndex = 0;

            if (IsCheck[0]) moduleIndex = 0;
            if (IsCheck[1]) moduleIndex = 1;
            if (IsCheck[2]) moduleIndex = 2;
            if (IsCheck[3]) moduleIndex = 3;

            return moduleIndex;
        }
    }
}
