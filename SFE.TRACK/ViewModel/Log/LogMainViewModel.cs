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
        public RelayCommand DateRelayCommand { get; set; }
        string dateDisplay = string.Empty;
        public LogMainViewModel()
        {
            dateDisplay = DateTime.Now.ToString("yyyy-MM-dd");
            DateRelayCommand = new RelayCommand(DateCommand);
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
