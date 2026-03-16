using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using CoreCSBase;
using CoreCSMac;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MachineDefine;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintenanceModeViewModel : ViewModelBase
    {
        public List<ModuleBaseCls> ModuleList { get; set; }
        public ModuleBaseCls ModuleInfo { get; set; }
        int ModuleSelectedIndex_ = 0;
        public RelayCommand<object> MaintDoubleClickRelayCommand { get; set; }
        List<string> arChamberInfo = new List<string>();

        public MaintenanceModeViewModel()
        {
            MaintDoubleClickRelayCommand = new RelayCommand<object>(MaintDoubleClickCommand);
            ModuleList = Global.STModuleList.FindAll(x => x.ModuleNo != 0 && x.BlockNo != 1);
        }

        public int ModuleSelectedIndex
        {
            get { return ModuleSelectedIndex_; }
            set { ModuleSelectedIndex_ = value; RaisePropertyChanged("ModuleSelectedIndex"); }
        }

        private void MaintDoubleClickCommand(object o)
        {
            if (ModuleInfo == null) return;
            PrgCfgItem item = null;
            string command = string.Empty;
            Tokenizer t = null;

            arChamberInfo.Clear();
            int maintMode = Global.STAccessDB.GetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo);
            List<ModuleBaseCls> arModule = null;
            if (maintMode == -1) return;
            ModuleInfo.MaintMode = (enMaintenanceMode)maintMode;

            if (ModuleInfo.MaintMode == enMaintenanceMode.NONE)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Would you like to change the maintenance mode?"))
                {
                    if(Global.STAccessDB.SetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo, (int)enMaintenanceMode.MAINTMODE)) ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.MAINTMODE;
                    maintMode = (int)enMaintenanceMode.MAINTMODE;
                }
            }
            else if (ModuleInfo.MaintMode == enMaintenanceMode.MAINTMODE)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Do you want to disable maintenance mode?"))
                {
                    if (Global.STAccessDB.SetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo, (int)enMaintenanceMode.NONE)) ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.NONE;
                    maintMode = (int)enMaintenanceMode.NONE;
                }
            }
            else
            {
                Global.MessageOpen(enMessageType.OKCANCEL, "Using maintenance mode.");
                ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.MAINTMODE_USE;
                return;
            }

            if (ModuleInfo.ModuleType == enModuleType.SPINCHAMBER)
            {
                if (ModuleInfo.MachineName == "COT")
                {
                    item = Global.MachineWorker.Reader.GetConfigItem(EnumPrgCfg.Environment__CoaterInfo);
                    arModule = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("COT") != -1).OrderBy(x => x.ModuleNo).ToList();

                    for(int i = 0; i < arModule.Count; i++)
                    {
                        ModuleBaseCls mod = arModule[i];
                        t = new Tokenizer(item.GetString(i), ",");
                        if (mod.BlockNo == ModuleInfo.BlockNo && mod.ModuleNo == ModuleInfo.ModuleNo)
                            command = string.Format("{0}, {1}, {2}, {3}, {4}", t.GetInt(0), t.GetInt(1), t.GetInt(2), t.GetBoolean(3), maintMode);
                        else command = item.GetString(i);

                        arChamberInfo.Add(command);
                    }
                    item.SetValueAll(arChamberInfo);
                }
                else if (ModuleInfo.MachineName == "DEV")
                {
                    item = Global.MachineWorker.Reader.GetConfigItem(EnumPrgCfg.Environment__DeveloperInfo);
                    arModule = Global.STModuleList.FindAll(x => x.MachineName.IndexOf("DEV") != -1).OrderBy(x => x.ModuleNo).ToList();

                    for (int i = 0; i < arModule.Count; i++)
                    {
                        ModuleBaseCls mod = arModule[i];
                        t = new Tokenizer(item.GetString(i), ",");
                        if (mod.BlockNo == ModuleInfo.BlockNo && mod.ModuleNo == ModuleInfo.ModuleNo)
                            command = string.Format("{0}, {1}, {2}, {3}, {4}", t.GetInt(0), t.GetInt(1), t.GetInt(2), t.GetBoolean(3), maintMode);
                        else command = item.GetString(i);

                        arChamberInfo.Add(command);
                    }
                    item.SetValueAll(arChamberInfo);
                }
            }
            else if(ModuleInfo.ModuleType == enModuleType.CHAMBER)
            {
                item = Global.MachineWorker.Reader.GetConfigItem(EnumPrgCfg.Environment__ChamberInfo);
                arModule = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.CHAMBER).OrderBy(x => x.ModuleNo).ToList();
                
                for (int i = 0; i < arModule.Count; i++)
                {
                    ModuleBaseCls mod = arModule[i];
                    t = new Tokenizer(item.GetString(i), ",");
                    if (mod.BlockNo == ModuleInfo.BlockNo && mod.ModuleNo == ModuleInfo.ModuleNo)
                        command = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", t.GetInt(0), t.GetString(1), t.GetInt(2), t.GetInt(3), t.GetBoolean(4), maintMode);
                    else command = item.GetString(i);

                    arChamberInfo.Add(command);
                }
                item.SetValueAll(arChamberInfo);
            }
        }
    }

    public class MaintValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            enMaintenanceMode maintMode = (enMaintenanceMode)value;
            if (maintMode == enMaintenanceMode.NONE) return new SolidColorBrush(Colors.White);

            return new SolidColorBrush(Colors.Orchid);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
