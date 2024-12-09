using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Util
{
    public class RobotConfigViewModel : ViewModelBase
    {
        public List<ModuleBaseCls> ModuleList { get; set; }
        public ModuleBaseCls ModuleInfo { get; set; }
        public RelayCommand SaveRobotContainRelayCommand { get; set; }
        public RelayCommand<object> RobotContainDoubleClickRelayCommand { get; set; }

        int ModuleSelectedIndex_ = 0;

        public RobotConfigViewModel()
        {
            ModuleList = Global.STModuleList.FindAll(x => x.ModuleNo != 0);
            SaveRobotContainRelayCommand = new RelayCommand(SaveRobotContainCommand);
            RobotContainDoubleClickRelayCommand = new RelayCommand<object>(RobotContainDoubleClickCommand);
        }

        public int ModuleSelectedIndex
        {
            get { return ModuleSelectedIndex_; }
            set { ModuleSelectedIndex_ = value; RaisePropertyChanged("ModuleSelectedIndex"); }
        }

        public void SaveRobotContainCommand()
        {
            if (Global.STDataAccess.SaveModuleData()) Global.MessageOpen(enMessageType.OK, "It has been saved.");
            else Global.MessageOpen(enMessageType.OK, "Not saved.");
        }

        public void RobotContainDoubleClickCommand(object o)
        {
            if (ModuleInfo == null) return;

            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 3:
                    ModuleInfo.IsUseCRA = ModuleInfo.IsUseCRA.Equals(true) ? false : true;
                    break;
                case 4:
                    ModuleInfo.IsUsePRA = ModuleInfo.IsUsePRA.Equals(true) ? false : true;
                    break;
                case 5:
                    ModuleInfo.IsUseIRA = ModuleInfo.IsUseIRA.Equals(true) ? false : true;
                    break;
            }
        }
    }
}
