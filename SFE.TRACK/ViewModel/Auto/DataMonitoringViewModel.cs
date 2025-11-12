using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
namespace SFE.TRACK.ViewModel.Auto
{
    public class DataMonitoringViewModel : ViewModelBase
    {
        List<MonitoringDataCls> MonitoringList_ { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand LoadRelayCommand { get; set; }
        public DataMonitoringViewModel()
        {
            LoadRelayCommand = new RelayCommand(LoadCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);             
        }

        public List<MonitoringDataCls> MonitoringList
        {
            get { return MonitoringList_; }
            set { MonitoringList_ = value; RaisePropertyChanged("MonitoringList"); }
        }

        private void LoadCommand()
        {
            MonitoringList = Global.STMonitoringList.FindAll(x => x.IsUse == true).ToList();
        }

        private void OKCommand(Window o)
        {
            o.DialogResult = true;
        }
    }
}
