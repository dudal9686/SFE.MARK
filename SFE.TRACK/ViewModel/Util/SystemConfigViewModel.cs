using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;
using CoreCSMac;

namespace SFE.TRACK.ViewModel.Util
{
    public class SystemConfigViewModel : ViewModelBase
    {
        public List<SystemCfgCls> arSystemCfgList { get; set; }
        public List<SystemCfgCls> arCotCfgList { get; set; }
        public List<SystemCfgCls> arDevLCfgList { get; set; }
        public List<SystemCfgCls> arDevRCfgList { get; set; }
        public RelayCommand SaveSystemCfgRelayCommand { get; set; }
        public RelayCommand SaveDevLCfgRelayCommand { get; set; }
        public RelayCommand SaveDevRCfgRelayCommand { get; set; }
        public RelayCommand SaveCotCfgRelayCommand { get; set; }
        public RelayCommand<object> SystemConfigGridDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> DevLConfigGridDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> DevRConfigGridDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> CotConfigGridDoubleClickRelayCommand { get; set; }
        int systemSelectedIndex = 0;
        int cotSelectedIndex = 0;
        int devLSelectedIndex = 0;
        int devRSelectedIndex = 0;
        public SystemCfgCls SystemConfig { get; set; }
        public SystemCfgCls DevLConfig { get; set; }
        public SystemCfgCls DevRConfig { get; set; }
        public SystemCfgCls CotConfig { get; set; }
        public SystemConfigViewModel()
        {
            SaveSystemCfgRelayCommand = new RelayCommand(SaveSystemCfgCommand);
            SaveDevLCfgRelayCommand = new RelayCommand(SaveDevLCfgCommand);
            SaveDevRCfgRelayCommand = new RelayCommand(SaveDevRCfgCommand);
            SaveCotCfgRelayCommand = new RelayCommand(SaveCotCfgCommand);

            SystemConfigGridDoubleClickRelayCommand = new RelayCommand<object>(SystemConfigGridDoubleClickCommand);
            DevLConfigGridDoubleClickRelayCommand = new RelayCommand<object>(DevLConfigGridDoubleClickCommand);
            DevRConfigGridDoubleClickRelayCommand = new RelayCommand<object>(DevRConfigGridDoubleClickCommand);
            CotConfigGridDoubleClickRelayCommand = new RelayCommand<object>(CotConfigGridDoubleClickRCommand);

            arSystemCfgList = Global.STSystemCfgList.FindAll(x => x.BlockNo == 0 && x.ModuleNo == 0).ToList();
            arCotCfgList = Global.STSystemCfgList.FindAll(x => x.BlockNo == 2 && x.ModuleNo == 1).ToList();
            arDevLCfgList = Global.STSystemCfgList.FindAll(x => x.BlockNo == 2 && x.ModuleNo == 2).ToList();
            arDevRCfgList = Global.STSystemCfgList.FindAll(x => x.BlockNo == 2 && x.ModuleNo == 3).ToList();
        }

        public int SystemSelectedIndex
        {
            get { return systemSelectedIndex; }
            set { systemSelectedIndex = value; RaisePropertyChanged("SystemSelectedIndex"); }
        }
        public int DevLSelectedIndex
        {
            get { return devLSelectedIndex; }
            set { devLSelectedIndex = value; RaisePropertyChanged("DevLSelectedIndex"); }
        }
        public int DevRSelectedIndex
        {
            get { return devRSelectedIndex; }
            set { devRSelectedIndex = value; RaisePropertyChanged("DevRSelectedIndex"); }
        }
        public int CotSelectedIndex
        {
            get { return cotSelectedIndex; }
            set { cotSelectedIndex = value; RaisePropertyChanged("CotSelectedIndex"); }
        }

        private void SaveSystemCfgCommand()
        {
            foreach(SystemCfgCls system in arSystemCfgList)
            {
                PrgCfgItem prgItem = Global.MachineWorker.Reader.GetConfigItem("SystemChamber", system.Name);
                prgItem.SetValue(Convert.ToInt32(system.Value));
            }
            Global.MessageOpen(enMessageType.OK, "It has been Saved");
        }

        private void SaveDevLCfgCommand()
        {
            foreach (SystemCfgCls system in arDevLCfgList)
            {
                PrgCfgItem prgItem = Global.MachineWorker.Reader.GetConfigItem("SystemChamber", system.Name);
                prgItem.SetValue(Convert.ToInt32(system.Value));
            }
            Global.MessageOpen(enMessageType.OK, "It has been Saved");
        }

        private void SaveDevRCfgCommand()
        {
            foreach (SystemCfgCls system in arDevRCfgList)
            {
                PrgCfgItem prgItem = Global.MachineWorker.Reader.GetConfigItem("SystemChamber", system.Name);
                prgItem.SetValue(Convert.ToInt32(system.Value));
            }
            Global.MessageOpen(enMessageType.OK, "It has been Saved");
        }

        private void SaveCotCfgCommand()
        {
            foreach (SystemCfgCls system in arCotCfgList)
            {
                PrgCfgItem prgItem = Global.MachineWorker.Reader.GetConfigItem("SystemChamber", system.Name);

                if(system.Name == "Cot_Pump_Calibration")
                    prgItem.SetValue(Convert.ToDouble(system.Value));
                else
                    prgItem.SetValue(Convert.ToInt32(system.Value));
            }
            Global.MessageOpen(enMessageType.OK, "It has been Saved");
        }

        private void SystemConfigGridDoubleClickCommand(object o)
        {
            if (SystemConfig == null) return;
            SystemConfig.Value = Global.KeyPad(Convert.ToSingle(SystemConfig.Value)).ToString();
        }

        private void DevLConfigGridDoubleClickCommand(object o)
        {
            if (DevLConfig == null) return;
            DevLConfig.Value = Global.KeyPad(Convert.ToSingle(DevLConfig.Value)).ToString();
        }

        private void DevRConfigGridDoubleClickCommand(object o)
        {
            if (DevRConfig == null) return;
            DevRConfig.Value = Global.KeyPad(Convert.ToSingle(DevRConfig.Value)).ToString();
        }

        private void CotConfigGridDoubleClickRCommand(object o)
        {
            if (CotConfig == null) return;
            CotConfig.Value = Global.KeyPad(Convert.ToSingle(CotConfig.Value)).ToString();
        }
    }
}
