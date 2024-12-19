using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Alarm
{
    public class AlarmMainViewModel : ViewModelBase
    {
        public RelayCommand ClearAlarmRelayCommand { get; set; }
        public RelayCommand ClearWarningRelayCommand { get; set; }
        public RelayCommand BuzzerOffRelayCommand { get; set; }
        public AlarmLogCls AlarmSelectedItem { get; set; }
        public AlarmLogCls WarningSelectedItem { get; set; }        

        int alarmSelectedIndex = 0;
        int warningSelectedIndex = 0;

        public AlarmMainViewModel()
        {
            ClearAlarmRelayCommand = new RelayCommand(ClearAlarmCommand);
            ClearWarningRelayCommand = new RelayCommand(ClearWarningCommand);
            BuzzerOffRelayCommand = new RelayCommand(BuzzerOffCommand);
        }

        public int AlarmSelectedIndex
        {
            get { return alarmSelectedIndex; }
            set { alarmSelectedIndex = value; RaisePropertyChanged("AlarmSelectedIndex"); }
        }

        public int WarningSelectedIndex
        {
            get { return warningSelectedIndex; }
            set { warningSelectedIndex = value; RaisePropertyChanged("WarningSelectedIndex"); }
        }

        private void ClearAlarmCommand()
        {
            Global.STAlarmList.Clear();
            CommonServiceLocator.ServiceLocator.Current.GetInstance<MainViewModel>().ClearAlarm();
        }

        private void ClearWarningCommand()
        {
            if (WarningSelectedItem == null) return;
            Global.STAlarmList.Clear();
        }

        private void BuzzerOffCommand()
        {
            //Buzzer Off
        }
    }
}
