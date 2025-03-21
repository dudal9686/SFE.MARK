using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Util
{
    public class EditPumpControlViewModel : ViewModelBase
    {
        public RelayCommand<Window> SaveRelayCommand { get; set; }
        public RelayCommand<Window> CloseRelayCommand { get; set; }
        public List<DispenseInfoCls> DispenseInfoList { get; set; }
        public RelayCommand<object> GridSystemDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> GridCountDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> GridCalDoubleClickRelayCommand { get; set; }
        DispenseInfoCls dispenseInfo { get; set; }

        public EditPumpControlViewModel()
        {
            DispenseInfoList = new List<DispenseInfoCls>();
            GridSystemDoubleClickRelayCommand = new RelayCommand<object>(GridSystemDoubleClickCommand);
            GridCountDoubleClickRelayCommand = new RelayCommand<object>(GridCountDoubleClickCommand);
            GridCalDoubleClickRelayCommand = new RelayCommand<object>(GridCalDoubleClickCommand);
            SaveRelayCommand = new RelayCommand<Window>(SaveCommand);
            CloseRelayCommand = new RelayCommand<Window>(CloseCommand);
        }

        public void GridSystemDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.PumpControlData.PumpCapa = Global.KeyPad(DispenseInfo.PumpControlData.PumpCapa);
                    break;
                case 1:
                    DispenseInfo.PumpControlData.PassOper = !DispenseInfo.PumpControlData.PassOper;
                    break;
                case 2:
                    DispenseInfo.PumpControlData.PulseCount = (int)Global.KeyPad(DispenseInfo.PumpControlData.PulseCount);
                    break;
                case 3:
                    DispenseInfo.PumpControlData.SpareReload = (int)Global.KeyPad(DispenseInfo.PumpControlData.SpareReload);
                    break;
            }
        }
        public void GridCountDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.PumpControlData.TotalDispCountAlarm = (int)Global.KeyPad(DispenseInfo.PumpControlData.TotalDispCountAlarm);
                    break;
                case 1:
                    DispenseInfo.PumpControlData.TotalDispCountStop = (int)Global.KeyPad(DispenseInfo.PumpControlData.TotalDispCountStop);
                    break;
            }
        }
        public void GridCalDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.PumpControlData.Calibration = Global.KeyPad(DispenseInfo.PumpControlData.Calibration);
                    break;
            }
        }

        public DispenseInfoCls DispenseInfo
        {
            get { return dispenseInfo; }
            set { dispenseInfo = value;
                if(dispenseInfo != null)
                {
                    DispenseInfoList.Clear();
                    DispenseInfoList.Add(dispenseInfo);
                }
                RaisePropertyChanged("DispenseInfo");
            }
        }

        private void SaveCommand(Window o)
        {
            if(Global.STDataAccess.SetPumpControlData(DispenseInfo)) Global.MessageOpen(enMessageType.OK, "It has been Saved.");
            o.DialogResult = true;
        }

        private void CloseCommand(Window o)
        {
            Global.STDataAccess.GetPumpControlData(DispenseInfo);
            o.DialogResult = false;
        }
    }
}
