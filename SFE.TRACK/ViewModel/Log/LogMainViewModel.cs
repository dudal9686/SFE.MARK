using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Log
{
    public class LogMainViewModel : ViewModelBase 
    {
        bool isChamber = true;
        bool isPRA = false;
        bool isCRA = false;
        public RelayCommand DateRelayCommand { get; set; }
        string dateDisplay = string.Empty;
        public LogMainViewModel()
        {
            dateDisplay = DateTime.Now.ToString("yyyy-MM-dd");
            DateRelayCommand = new RelayCommand(DateCommand);
        }

        public bool IsChamber
        {
            get { return isChamber; }
            set { isChamber = value; RaisePropertyChanged("IsChamber"); }
        }

        public bool IsPRA
        {
            get { return isPRA; }
            set { isPRA = value; RaisePropertyChanged("IsPRA"); }
        }

        public bool IsCRA
        {
            get { return isCRA; }
            set { isCRA = value; RaisePropertyChanged("IsCRA"); }
        }

        public string DateDisplay
        {
            get { return dateDisplay; }
            set { dateDisplay = value; RaisePropertyChanged("DateDisplay"); }
        }

        private void DateCommand()
        {
            if(Global.GetDateOpen(DateDisplay))
            {
                DateDisplay = Global.STDateMessage.Date.ToString("yyyy-MM-dd");
            }
        }
    }
}
