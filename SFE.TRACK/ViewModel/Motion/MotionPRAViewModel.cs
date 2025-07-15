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

            string command = string.Format("CHAMBER:{0}:{1}:PutReady", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__StepRequest, command);
        }

        private void GetReadyCommand()
        {
            if (Module == null) return;
            string command = string.Format("CHAMBER:{0}:{1}:GetReady", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__StepRequest, command);
        }

        private void UnLoadCommand()
        {
            if (Module == null) return;
            string command = string.Format("CHAMBER:{0}:{1}:Idle", Module.BlockNo, Module.ModuleNo);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__StepRequest, command);
        }

        private void ProcessStartCommand()
        {
            if (Module == null) return;

            string fileName = string.Empty;
            if (Module.MachineName.ToUpper().IndexOf("CPL") != -1) fileName = "CPL_CHANGE";
            else if (Module.MachineName.ToUpper().IndexOf("COT") != -1) fileName = "COT_TEST";
            else if (Module.MachineName.ToUpper().IndexOf("DEV") != -1) fileName = "DEV_RECIPE";
            else if (Module.MachineName.ToUpper().IndexOf("HHP") != -1) fileName = "HHP_RECIPE";

            string command = string.Format("CHAMBER:{0}:{1}:Processing:{2}", Module.BlockNo, Module.ModuleNo, fileName);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Chamber__StepRequest, command);//Chamber__StartProcess
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
            string msg = string.Empty;
            int index = 0;
            EnumCustomProcess customType = EnumCustomProcess.None;

            if (Module.BlockNo == 1)
            {
                msg = string.Format("AssyPRA:DoPickDropWaferOnCRA 0,{0}", GetArmIndex());
            }
            else
            {
                if (Module.ModuleType == enModuleType.CHAMBER)
                {
                    customType = EnumCustomProcess.Chamber;
                    index = Module.ModuleNo - 5;                    
                }
                else if(Module.ModuleType == enModuleType.SPINCHAMBER)
                {
                    if (Module.MachineName.IndexOf("DEV") != -1)
                    {
                        customType = EnumCustomProcess.Developer;
                        index = Module.ModuleNo - 2;
                    }
                    else
                    {
                        customType = EnumCustomProcess.Coater;
                    }
                }

                msg = string.Format("AssyPRA:DoPickDropWaferOnProcess 0,{0},{1},{2}", GetArmIndex(), (int)customType, index);
            }
            
            Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, msg);
        }

        private void PlaceMotionCommand()
        {
            if (Module == null)
            {
                Global.MessageOpen(enMessageType.OK, "Please Select Module!");
                return;
            }

            string msg = string.Empty;
            int index = 0;
            EnumCustomProcess customType = EnumCustomProcess.None;

            if (Module.BlockNo == 1)
            {
                msg = string.Format("AssyPRA:DoPickDropWaferOnCRA 1,{0}", GetArmIndex());
            }
            else
            {
                if (Module.ModuleType == enModuleType.CHAMBER)
                {
                    customType = EnumCustomProcess.Chamber;
                    index = Module.ModuleNo - 5;
                }
                else if (Module.ModuleType == enModuleType.SPINCHAMBER)
                {
                    if (Module.MachineName.IndexOf("DEV") != -1)
                    {
                        customType = EnumCustomProcess.Developer;
                        index = Module.ModuleNo - 2;
                    }
                    else
                    {
                        customType = EnumCustomProcess.Coater;
                    }
                }

                msg = string.Format("AssyPRA:DoPickDropWaferOnProcess 1,{0},{1},{2}", GetArmIndex(), (int)customType, index);
            }

            Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.TermManual__Do, msg);
        }

        private int GetArmIndex()
        {
            int armIndex = 0;
            if (IsCheckArm[0]) armIndex = 0;
            if (IsCheckArm[1]) armIndex = 1;
            if (IsCheckArm[2]) armIndex = 2;

            return armIndex;
        }
    }
}
