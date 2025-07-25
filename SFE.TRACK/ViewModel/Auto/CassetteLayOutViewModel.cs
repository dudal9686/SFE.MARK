using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using MachineDefine;
using CoreCSBase.IPC;

namespace SFE.TRACK.ViewModel.Auto
{
    public class CassetteLayOutViewModel : ViewModelBase
    {
        bool isCheck = false;
        public RelayCommand<string> CassetteScanRelayCommand { get; set; }
        public RelayCommand<string> CassetteScanRunStopRelayCommand { get; set; }
        public FoupCls foup1 { get; set; }
        public FoupCls foup2 { get; set; }
        public FoupCls foup3 { get; set; }
        public FoupCls foup4 { get; set; }
        public CassetteLayOutViewModel()
        {
            CassetteScanRelayCommand = new RelayCommand<string>(CassetteScanCommand);
            CassetteScanRunStopRelayCommand = new RelayCommand<string>(CassetteScanRunStopCommand);
            foup1 = Global.STModuleList.Find(x => x.BlockNo == 1 && x.ModuleNo == 1) as FoupCls;
            foup2 = Global.STModuleList.Find(x => x.BlockNo == 1 && x.ModuleNo == 2) as FoupCls;
            foup3 = Global.STModuleList.Find(x => x.BlockNo == 1 && x.ModuleNo == 3) as FoupCls;
            foup4 = Global.STModuleList.Find(x => x.BlockNo == 1 && x.ModuleNo == 4) as FoupCls;
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

        public void CassetteScanRunStopCommand(string cst)
        {
            FoupCls foup = null;

            if (cst == "0") foup = foup1;
            else if (cst == "1") foup = foup2;
            else if (cst == "2") foup = foup3;
            else if (cst == "3") foup = foup4;

            if(foup.StorageUseStep == DefaultBase.WaferStorageUseStep.IsRun) Global.MachineWorker.SendCommand(IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.Cassette__Stop, cst);
            else if (foup.StorageUseStep == DefaultBase.WaferStorageUseStep.IsStop) Global.MachineWorker.SendCommand(IPCNetClient.DataType.String, EnumCommand.Setting, EnumCommand_Setting.Cassette__Start, cst);
        }
    }
}
