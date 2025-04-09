using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Util
{
    public class UtilMainViewModel : ViewModelBase
    {
        bool isMachineConfig = false;
        bool isDispenseConfig = false;
        bool isRobotConfig = true;
        bool isCassetteConfig = false;
        bool isLampConfig = false;
        bool isSystemConfig = false;
        public bool IsMachineConfig
        {
            get { return isMachineConfig; }
            set { isMachineConfig = value; RaisePropertyChanged("IsMachineConfig"); }
        }
        public bool IsDispenseConfig
        {
            get { return isDispenseConfig; }
            set { isDispenseConfig = value; RaisePropertyChanged("IsDispenseConfig"); }
        }
        public bool IsRobotConfig
        {
            get { return isRobotConfig; }
            set { isRobotConfig = value; RaisePropertyChanged("IsRobotConfig"); }
        }
        public bool IsCassetteConfig
        {
            get { return isCassetteConfig; }
            set { isCassetteConfig = value; RaisePropertyChanged("IsCassetteConfig"); }
        }
        public bool IsLampConfig
        {
            get { return isLampConfig; }
            set { isLampConfig = value; RaisePropertyChanged("IsLampConfig"); }
        }
        public bool IsSystemConfig
        {
            get { return isSystemConfig; }
            set { isSystemConfig = value; RaisePropertyChanged("IsSystemConfig"); }
        }
    }
}
