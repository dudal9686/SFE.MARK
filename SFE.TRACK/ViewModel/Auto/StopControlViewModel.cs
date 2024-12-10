using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Windows;

namespace SFE.TRACK.ViewModel.Auto
{
    public class StopControlViewModel : ViewModelBase
    {
        public RelayCommand<Window> StopRelayCommand { get; set; }
        public RelayCommand<Window> RecoveryRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }

        public StopControlViewModel()
        {
            StopRelayCommand = new RelayCommand<Window>(StopCommand);
            RecoveryRelayCommand = new RelayCommand<Window>(RecoveryCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
        }

        private void StopCommand(Window o)
        {
            o.DialogResult = true;
        }
        private void RecoveryCommand(Window o)
        {
            o.DialogResult = true;
        }
        private void CancelCommand(Window o)
        {
            o.DialogResult = false;
        }
    }
}
