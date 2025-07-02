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
        public RelayCommand PutReadyRelayCommand { get; set; }
        public RelayCommand GetReadyRelayCommand { get; set; }
        public RelayCommand UnLoadReadyRelayCommand { get; set; }
        public RelayCommand ProcessStartRelayCommand { get; set; }
        public List<Model.IODataCls> DIList { get; set; }
        public List<Model.IODataCls> DOList { get; set; }
        int blockNo = 0;
        int moduleNo = 0;
        public MotionPRAViewModel()
        {
            ModuleRelayCommand = new RelayCommand(ModuleCommand);
            PickMotionRelayCommand = new RelayCommand(PickMotionCommand);
            PlaceMotionRelayCommand = new RelayCommand(PlaceMotionCommand);
            PutReadyRelayCommand = new RelayCommand(PutReadyCommand);
            GetReadyRelayCommand = new RelayCommand(GetReadyCommand);
            UnLoadReadyRelayCommand = new RelayCommand(UnLoadCommand);
            ProcessStartRelayCommand = new RelayCommand(ProcessStartCommand);
            DIList = Global.STDIList.FindAll(x => x.BlockNo == 2).OrderBy(x => x.ModuleNo).ThenBy(x => x.IONum).ToList();
            DOList = Global.STDOList.FindAll(x => x.BlockNo == 2).OrderBy(x => x.ModuleNo).ThenBy(x => x.IONum).ToList();
        }

        private void PutReadyCommand()
        {
            if (Module == null) return;
            string fileName = string.Empty;
            if (Module.MachineName.ToUpper().IndexOf("CPL") != -1) fileName = "CPL_CHANGE";
            else if (Module.MachineName.ToUpper().IndexOf("COT") != -1) fileName = "COT_TEST";
            else if (Module.MachineName.ToUpper().IndexOf("DEV") != -1) fileName = "DEV_RECIPE";
            else if (Module.MachineName.ToUpper().IndexOf("HHP") != -1) fileName = "HHP_RECIPE";


            string command = string.Format("CHAMBER:{0}:{1}:{2}", Module.BlockNo, Module.ModuleNo, fileName);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__PutReady, command);
        }

        private void GetReadyCommand()
        {
            if (Module == null) return;
            string command = string.Format("CHAMBER:{0}:{1}", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__GetReady, command);
        }

        private void UnLoadCommand()
        {
            if (Module == null) return;
            string command = string.Format("CHAMBER:{0}:{1}", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__UnLoad, command);
        }

        private void ProcessStartCommand()
        {
            if (Module == null) return;
            string command = string.Format("CHAMBER:{0}:{1}", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Status, EnumCommand_Status.Chamber__StartProcess, command);
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
            //Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__PickManualMove, msg);

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
            //Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Move__PlaceManualMove, msg);
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
