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

        public AlarmMainViewModel()
        {
            ClearAlarmRelayCommand = new RelayCommand(ClearAlarmCommand);
            ClearWarningRelayCommand = new RelayCommand(ClearWarningCommand);
        }

        private void ClearAlarmCommand()
        {

        }

        private void ClearWarningCommand()
        {

        }
    }
}
