using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace SFE.TRACK.ViewModel.Maint
{
    public class SelSupportDateViewModel : ViewModelBase
    {
        DateTime selectDate;
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }

        public SelSupportDateViewModel()
        {
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            Messenger.Default.Register<DateMessageCls>(this, OnReceiveMessageAction);
        }

        public DateTime SelectDate
        {
            get { return selectDate; }
            set { selectDate = value; RaisePropertyChanged("SelectDate"); }
        }

        private void OnReceiveMessageAction(DateMessageCls o)
        {
            SelectDate = o.Date;
        }

        private void OKCommand(Window window)
        {
            Global.STDateMessage.Date = SelectDate;
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }
    }
}
