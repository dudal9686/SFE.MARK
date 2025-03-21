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
    public class EditAutoSupplyControlViewModel : ViewModelBase
    {
        public RelayCommand<Window> SaveRelayCommand { get; set; }
        public RelayCommand<Window> CloseRelayCommand { get; set; }
        public List<DispenseInfoCls> DispenseInfoList { get; set; }
        public RelayCommand<object> GridDataDoubleClickRelayCommand { get; set; }
        DispenseInfoCls dispenseInfo { get; set; }

        public EditAutoSupplyControlViewModel()
        {
            DispenseInfoList = new List<DispenseInfoCls>();
            GridDataDoubleClickRelayCommand = new RelayCommand<object>(GridDataDoubleClickCommand);
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
                    DispenseInfo.AutoSupplyControlData.AutoMode = !DispenseInfo.AutoSupplyControlData.AutoMode;
                    break;
                case 1:
                    DispenseInfo.AutoSupplyControlData.SupplyTime = (int)Global.KeyPad(DispenseInfo.AutoSupplyControlData.SupplyTime);
                    break;
                case 2:
                    DispenseInfo.AutoSupplyControlData.SupplyDelayTime = (int)Global.KeyPad(DispenseInfo.AutoSupplyControlData.SupplyDelayTime);
                    break;
                case 3:
                    break;
                case 4:
                    DispenseInfo.AutoSupplyControlData.VacuumeTime = (int)Global.KeyPad(DispenseInfo.AutoSupplyControlData.VacuumeTime);
                    break;
                case 5:
                    DispenseInfo.AutoSupplyControlData.PurgeTime = (int)Global.KeyPad(DispenseInfo.AutoSupplyControlData.PurgeTime);
                    break;
                case 6:
                    DispenseInfo.AutoSupplyControlData.WaitTime = (int)Global.KeyPad(DispenseInfo.AutoSupplyControlData.WaitTime);
                    break;
            }
        }

        private void SaveCommand(Window o)
        {
            if (Global.STDataAccess.SetAutoSupplyControlData(DispenseInfo)) Global.MessageOpen(enMessageType.OK, "It has been Saved.");
            o.DialogResult = true;
        }

        private void CloseCommand(Window o)
        {
            Global.STDataAccess.GetAutoSupplyControlData(DispenseInfo);
            o.DialogResult = false;
        }
    }
}
