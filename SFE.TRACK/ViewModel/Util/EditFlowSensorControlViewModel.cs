using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Util
{
    public class EditFlowSensorControlViewModel : ViewModelBase
    {
        public RelayCommand<Window> SaveRelayCommand { get; set; }
        public RelayCommand<Window> CloseRelayCommand { get; set; }
        public ObservableCollection<DispenseInfoCls> DispenseInfoList { get; set; }
        public RelayCommand<object> GridDataDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> GridMonitDataDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> GridRangeDataDoubleClickRelayCommand { get; set; }
        DispenseInfoCls dispenseInfo { get; set; }

        public EditFlowSensorControlViewModel()
        {
            DispenseInfoList = new ObservableCollection<DispenseInfoCls>();
            GridDataDoubleClickRelayCommand = new RelayCommand<object>(GridDataDoubleClickCommand);
            GridMonitDataDoubleClickRelayCommand = new RelayCommand<object>(GridMonitDataDoubleClickCommand);
            GridRangeDataDoubleClickRelayCommand = new RelayCommand<object>(GridRangeDataDoubleClickCommand);
            SaveRelayCommand = new RelayCommand<Window>(SaveCommand);
            CloseRelayCommand = new RelayCommand<Window>(CloseCommand);
        }

        public DispenseInfoCls DispenseInfo
        {
            get { return dispenseInfo; }
            set
            {
                dispenseInfo = value;
                if (dispenseInfo != null)
                {
                    DispenseInfoList.Clear();
                    DispenseInfoList.Add(dispenseInfo);
                }
                RaisePropertyChanged("DispenseInfo");
            }
        }

        private void GridDataDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.FlowControlData.PulseRate = Global.KeyPad(DispenseInfo.FlowControlData.PulseRate);
                    break;
                case 1:
                    DispenseInfo.FlowControlData.SamplingDelayTime = Global.KeyPad(DispenseInfo.FlowControlData.SamplingDelayTime);
                    break;
            }
        }

        private void GridMonitDataDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.FlowControlData.CheckTiming = !DispenseInfo.FlowControlData.CheckTiming;
                    break;
                case 1:
                    DispenseInfo.FlowControlData.FlowMonitoring = !DispenseInfo.FlowControlData.FlowMonitoring;
                    break;
                case 2:
                    DispenseInfo.FlowControlData.ReferenceValue = Global.KeyPad(DispenseInfo.FlowControlData.ReferenceValue);
                    break;
                case 3:
                    DispenseInfo.FlowControlData.Calibration = Global.KeyPad(DispenseInfo.FlowControlData.Calibration);
                    break;
            }
        }

        private void GridRangeDataDoubleClickCommand(object o)
        {
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 0:
                    DispenseInfo.FlowControlData.AlarmUpper = Global.KeyPad(DispenseInfo.FlowControlData.AlarmUpper);
                    break;
                case 1:
                    DispenseInfo.FlowControlData.AlarmLower = Global.KeyPad(DispenseInfo.FlowControlData.AlarmLower);
                    break;
                case 2:
                    DispenseInfo.FlowControlData.StopUpper = Global.KeyPad(DispenseInfo.FlowControlData.StopUpper);
                    break;
                case 3:
                    DispenseInfo.FlowControlData.StopLower = Global.KeyPad(DispenseInfo.FlowControlData.StopLower);
                    break;
            }
        }

        private void SaveCommand(Window o)
        {
            if(Global.STDataAccess.SetFlowControlData(DispenseInfo)) Global.MessageOpen(enMessageType.OK, "It has been Saved.");
            o.DialogResult = true;
        }

        private void CloseCommand(Window o)
        {
            Global.STDataAccess.GetFlowControlData(DispenseInfo);
            o.DialogResult = false;
        }
    }
}
