using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Maint
{
    public class EditMonitoringDataViewModel : ViewModelBase
    {
        List<MonitoringDataCls> list { get; set; }
        public MonitoringDataCls Info { get; set; } = null;
        public RelayCommand SaveRelayCommand { get; set; }
        public RelayCommand<object> GridDoubleClickRelayCommand { get; set; }
        int selectedIndex = 0;
        int nGridValue;
        float fGridValue;
        public EditMonitoringDataViewModel()
        {
            DataList = Global.STMonitoringList.ToList();
            SaveRelayCommand = new RelayCommand(SaveCommand);
            GridDoubleClickRelayCommand = new RelayCommand<object>(GridDoubleClickCommand);
        }

        public List<MonitoringDataCls> DataList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("DataList"); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }
        private void SaveCommand()
        {
            if(Global.STDataAccess.SaveMonitoringData()) Global.MessageOpen(enMessageType.OK, "Saved");
            else Global.MessageOpen(enMessageType.OK, "Not Saved");
        }
        private void GridDoubleClickCommand(object o)
        {
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch(index)
            {
                case 4:
                    Info.InitTemp = Global.KeyPad(Info.InitTemp);
                    break;
                case 5:
                    Info.OverTemp = Global.KeyPad(Info.OverTemp);
                    break;
                case 6:
                    Info.SettlingDetermTime = Convert.ToInt32(Global.KeyPad(Info.SettlingDetermTime));
                    break;
                case 7:
                    Info.SettlingTimeOut = Convert.ToInt32(Global.KeyPad(Info.SettlingTimeOut));
                    break;
                case 8:
                    Info.RangeMax = Global.KeyPad(Info.RangeMax);
                    break;
                case 9:
                    Info.RangeMin = Global.KeyPad(Info.RangeMin);
                    break;
            }
        }
    }
}
