using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintenanceModeViewModel : ViewModelBase
    {
        public List<ModuleBaseCls> ModuleList { get; set; }
        public ModuleBaseCls ModuleInfo { get; set; }
        int ModuleSelectedIndex_ = 0;
        public RelayCommand<object> MaintDoubleClickRelayCommand { get; set; }

        public MaintenanceModeViewModel()
        {
            MaintDoubleClickRelayCommand = new RelayCommand<object>(MaintDoubleClickCommand);
            ModuleList = Global.STModuleList.FindAll(x => x.ModuleNo != 0);
        }

        public int ModuleSelectedIndex
        {
            get { return ModuleSelectedIndex_; }
            set { ModuleSelectedIndex_ = value; RaisePropertyChanged("ModuleSelectedIndex"); }
        }

        private void MaintDoubleClickCommand(object o)
        {
            if (ModuleInfo == null) return;

            int maintMode = Global.STAccessDB.GetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo);
            if (maintMode == -1) return;
            ModuleInfo.MaintMode = (enMaintenanceMode)maintMode;

            if (ModuleInfo.MaintMode == enMaintenanceMode.NONE)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Would you like to change the maintenance mode?"))
                {
                    if(Global.STAccessDB.SetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo, (int)enMaintenanceMode.MAINTMODE)) ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.MAINTMODE;
                }
            }
            else if (ModuleInfo.MaintMode == enMaintenanceMode.MAINTMODE)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Do you want to disable maintenance mode?"))
                {
                    if (Global.STAccessDB.SetMaintMode(ModuleInfo.BlockNo, ModuleInfo.ModuleNo, (int)enMaintenanceMode.NONE)) ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.NONE;
                }
            }
            else
            {
                Global.MessageOpen(enMessageType.OKCANCEL, "Using maintenance mode.");
                ModuleList[ModuleSelectedIndex].MaintMode = enMaintenanceMode.MAINTMODE_USE;
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
