using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreCSBase.IPC;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MachineDefine;

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
            if(AlarmSelectedItem != null)
            {
                string command = string.Format("{0}:{1}:{2}", AlarmSelectedItem.Code, AlarmSelectedItem.Owner, AlarmSelectedItem.Param);
                Global.MachineWorker.SendCommand(AlarmSelectedItem.SendID, IPCNetClient.DataType.String, EnumCommand.Alarm, EnumCommand_Alarm.Request___AlarmClear, command);
                Global.STAlarmList.Remove(AlarmSelectedItem);
                if (Global.STAlarmList.Count == 0)
                    CommonServiceLocator.ServiceLocator.Current.GetInstance<MainViewModel>().ClearAlarm();

                if (Global.STAlarmList.Count > 0) AlarmSelectedIndex = 0;
            }
        }

        private void ClearWarningCommand()
        {
            if (WarningSelectedItem != null)
            {
                Global.MachineWorker.SendCommand(WarningSelectedItem.SendID, IPCNetClient.DataType.String, EnumCommand.Warning, EnumCommand_Warning.Send___Clear, WarningSelectedItem.Message);
                Global.STWarningList.Remove(WarningSelectedItem);
                if (Global.STWarningList.Count == 0)
                    CommonServiceLocator.ServiceLocator.Current.GetInstance<MainViewModel>().ClearWarning();

                if (Global.STWaferList.Count > 0) WarningSelectedIndex = 0;
            }
        }

        private void BuzzerOffCommand()
        {
            Global.SendCommand(IPCNetClient.DataType.String, EnumCommand.Action, EnumCommand_Action.Request___BuzzerOff, "OFF");
        }
    }
}
