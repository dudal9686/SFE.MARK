using System;
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
    
    public class MotionPRAViewModel : ViewModelBase
    {
        bool[] isCheckArm = new bool[3] { true, false, false };
        Model.ModuleBaseCls module { get; set; }
        public RelayCommand ModuleRelayCommand { get; set; }
        public RelayCommand PickMotionRelayCommand { get; set; }
        public RelayCommand PlaceMotionRelayCommand { get; set; }
        public List<Model.IODataCls> DIList { get; set; }
        public List<Model.IODataCls> DOList { get; set; }
        int blockNo = 0;
        int moduleNo = 0;
        public MotionPRAViewModel()
        {
            ModuleRelayCommand = new RelayCommand(ModuleCommand);
            PickMotionRelayCommand = new RelayCommand(PickMotionCommand);
            PlaceMotionRelayCommand = new RelayCommand(PlaceMotionCommand);
            DIList = Global.STDIList.FindAll(x => x.BlockNo == 2);
            DOList = Global.STDOList.FindAll(x => x.BlockNo == 2);
        }

        public Model.ModuleBaseCls Module
        {
            get { return module; }
            set { module = value; RaisePropertyChanged("Module"); }
        }

        public bool[] IsCheckArm
        {
            get { return isCheckArm; }
            set { isCheckArm = value; RaisePropertyChanged("IsCheckArm"); }
        }

        private  void ModuleCommand()
        {
            if(Module == null) { blockNo = 0; moduleNo = 0; }
            else { blockNo = Module.BlockNo; moduleNo = Module.ModuleNo; }

            if (Global.GetModuleOpen("MOTIONMODULE", blockNo, moduleNo))
            {
                blockNo = Global.STModulePopUp.BlockNo;
                moduleNo = Global.STModulePopUp.ModuleNo;
                Module = Global.GetModule(blockNo, moduleNo);
            }
        }
        private void PickMotionCommand()
        {
            if(Module == null)
            {
                Global.MessageOpen(enMessageType.OK, "Please Select Module!");
                return;
            }

            blockNo = Module.BlockNo;
            moduleNo = Module.ModuleNo;

            string msg = string.Format("PRA,{0},{1},{2},{3}", GetArmIndex(), blockNo, moduleNo, 1);
            Global.MachineWorker.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___PickManualMove, msg);

            //Protocol
        }

        private void PlaceMotionCommand()
        {
            if (Module == null)
            {
                Global.MessageOpen(enMessageType.OK, "Please Select Module!");
                return;
            }

            blockNo = Module.BlockNo;
            moduleNo = Module.ModuleNo;

            string msg = string.Format("PRA,{0},{1},{2},{3}", GetArmIndex(), blockNo, moduleNo, 1);
            Global.MachineWorker.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move___PlaceManualMove, msg);
        }

        private int GetArmIndex()
        {
            int armIndex = 0;
            if (IsCheckArm[0]) armIndex = 1;
            if (IsCheckArm[1]) armIndex = 2;
            if (IsCheckArm[2]) armIndex = 3;

            return armIndex;
        }
    }
}
