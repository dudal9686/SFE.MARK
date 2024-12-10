using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Auto
{
    public class MotorInitialViewModel : ViewModelBase
    {
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> HomeCheckDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> HomeCheckClickRelayCommand { get; set; }
        public RelayCommand AllCheckRelayCommand { get; set; }
        public AxisInfoCls Axis { get; set; }
        int selectedIndex = 0;
        bool check = false;

        public MotorInitialViewModel()
        {
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            HomeCheckDoubleClickRelayCommand = new RelayCommand<object>(HomeCheckDoubleClickCommand);
            HomeCheckClickRelayCommand = new RelayCommand<object>(HomeCheckClickCommand);
            AllCheckRelayCommand = new RelayCommand(AllCheckCommand);
        }

        public List<AxisInfoCls> AxisList
        {
            get { return Global.STAxis; }
            set { Global.STAxis = value; }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void OKCommand(Window window)
        {
            foreach(Model.AxisInfoCls axis in Global.STAxis)
            {
                //ModuleBaseCls module = Global.GetModule(axis.BlockNo, axis.ModuleNo);
                //Console.WriteLine("{0} [{1}]", module.MachineTitle, axis.AxisID);
            }

            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void AllCheckCommand()
        {
            foreach (AxisInfoCls axis in Global.STAxis)
            {
                axis.IsHomeChecked = !check;

                if (axis.HomeSituation == enHomeState.HOME_NONE) axis.HomeSituation = enHomeState.HOMMING;
                else if (axis.HomeSituation == enHomeState.HOMMING) axis.HomeSituation = enHomeState.HOME_OK;
                else if (axis.HomeSituation == enHomeState.HOME_OK) axis.HomeSituation = enHomeState.HOME_ERROR;
                else if (axis.HomeSituation == enHomeState.HOME_ERROR) axis.HomeSituation = enHomeState.HOME_NONE;
            }
            check = !check;
        }

        private void HomeCheckClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            if (index == 0)
            {
                Axis.IsHomeChecked = !Axis.IsHomeChecked;
            }
        }

        private void HomeCheckDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                case 1:
                case 3:
                case 4:
                    Axis.IsHomeChecked = !Axis.IsHomeChecked;
                    break;
                case 2:
                    bool ishomecheck = Axis.IsHomeChecked;
                    foreach(AxisInfoCls axis in Global.STAxis)
                    {
                        if (axis.BlockNo == Axis.BlockNo && axis.ModuleNo == Axis.ModuleNo) axis.IsHomeChecked = !ishomecheck;
                    }
                    break;
            }

            if (index == 0)
            {
                
            }
        }
    }
}
