using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using MachineDefine;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintChamberViewModel : ViewModelBase
    {
        List<ChamberCls> ModuleList_ { get; set; }
        ChamberCls ModuleInfo_ { get; set; }
        int SelectedIndex_ = -1;
        string MainTitle_ = string.Empty;
        public RelayCommand TempRelayCommand { get; set; }
        public RelayCommand RunRelayCommand { get; set; }
        public RelayCommand StopRelayCommand { get; set; }
        public RelayCommand AutoTuningRelayCommand { get; set; }
        public RelayCommand PinHomeRelayCommand { get; set; }
        public RelayCommand PinReadyRelayCommand { get; set; }
        public RelayCommand PinDownRelayCommand { get; set; }
        public RelayCommand PinUpRelayCommand { get; set; }
        public RelayCommand ServoOnRelayCommand { get; set; }
        public RelayCommand ServoOffRelayCommand { get; set; }
        public RelayCommand ShutterOpenRelayCommand { get; set; }
        public RelayCommand ShutterCloseRelayCommand { get; set; }
        public AxisInfoCls AxisInfo { get; set; }
        float tempValue = 20;
        public MaintChamberViewModel()
        {
            ModuleList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.CHAMBER && x.Use == true).Cast<ChamberCls>().ToList();
            if (ModuleList.Count > 0)
            {
                ModuleInfo = ModuleList[0];
                AxisInfo = Global.STAxis.Find(x => x.BlockNo == ModuleInfo.BlockNo && x.ModuleNo == ModuleInfo.ModuleNo);
                SelectedIndex = 0;
            }

            TempRelayCommand = new RelayCommand(TempCommand);
            RunRelayCommand = new RelayCommand(RunCommand);
            StopRelayCommand = new RelayCommand(StopCommand);
            AutoTuningRelayCommand = new RelayCommand(AutoTuningCommand);
            PinHomeRelayCommand = new RelayCommand(PinHomeCommand);
            PinReadyRelayCommand = new RelayCommand(PinReadyCommand);
            PinDownRelayCommand = new RelayCommand(PinDownCommand);
            PinUpRelayCommand = new RelayCommand(PinUpCommand);
            ServoOnRelayCommand = new RelayCommand(ServoOnCommand);
            ServoOffRelayCommand = new RelayCommand(ServoOffCommand);
            ShutterOpenRelayCommand = new RelayCommand(ShutterOpenCommand);
            ShutterCloseRelayCommand = new RelayCommand(ShutterCloseCommand);
        }
        public float TempValue
        {
            get { return tempValue; }
            set { tempValue = value; RaisePropertyChanged("TempValue"); }
        }
        
        public List<ChamberCls> ModuleList
        {
            get { return ModuleList_; }
            set { ModuleList_ = value; RaisePropertyChanged("ModuleList"); }
        }

        public ChamberCls ModuleInfo
        {
            get { return ModuleInfo_; }
            set { ModuleInfo_ = value;
                MainTitle = string.Format("Data {0}-{1} {2}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo, ModuleInfo.MachineName);
                AxisInfo = Global.STAxis.Find(x => x.BlockNo == ModuleInfo.BlockNo && x.ModuleNo == ModuleInfo.ModuleNo);
                Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ChamberState, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
                RaisePropertyChanged("ModuleInfo"); }
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set { SelectedIndex_ = value;                
                RaisePropertyChanged("SelectedIndex"); }
        }

        public string MainTitle
        {
            get { return MainTitle_; }
            set { MainTitle_ = value; RaisePropertyChanged("MainTitle"); }
        }
        #region Event
        private void TempCommand()
        {
            float max = 0;
            float min = 0;

            if(ModuleInfo.MachineName.ToUpper().IndexOf("ADH") != -1) { max = 180; min = 50; }
            else if (ModuleInfo.MachineName.ToUpper().IndexOf("CPL") != -1) { max = 28; min = 15; }
            else if (ModuleInfo.MachineName.ToUpper().IndexOf("LHP") != -1) { max = 180; min = 50; }
            else if (ModuleInfo.MachineName.ToUpper().IndexOf("HHP") != -1) { max = 300; min = 50; }

            ModuleInfo.SetValue = Global.KeyPad(ModuleInfo.SetValue, max, min);
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___SetTemperature, string.Format("Chamber:{0}:{1}:{2}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo, ModuleInfo.SetValue));
        }
        private void RunCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___Run, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void StopCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___Stop, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void AutoTuningCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___AutoTuning, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void PinHomeCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinHome, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void PinReadyCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinReady, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void PinDownCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinDown, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void PinUpCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___PinUp, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void ServoOnCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ServoOn, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void ServoOffCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ServoOff, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void ShutterOpenCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ShutterOpen, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        private void ShutterCloseCommand()
        {
            if (ModuleInfo == null) return;
            Global.MachineWorker.SendCommand(Global.CHAMBER_ID, CoreCSBase.IPC.IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.ChamberManual___ShutterClose, string.Format("Chamber:{0}:{1}", ModuleInfo.BlockNo, ModuleInfo.ModuleNo));
        }
        #endregion
    }
}
