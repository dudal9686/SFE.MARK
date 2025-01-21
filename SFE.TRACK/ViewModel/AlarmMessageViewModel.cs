using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;


namespace SFE.TRACK.ViewModel
{
    public class AlarmMessageViewModel : ViewModelBase
    {
        Visibility OKCANCELVisible_ = Visibility.Hidden;
        Visibility OKVisible_ = Visibility.Hidden;
        string Message_ = string.Empty;
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; } 
        public AlarmMessageViewModel()
        {
            Messenger.Default.Register<PopUpMessageCls>(this, OnReceiveMessageAction);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancleCommand);
        }

        public Visibility OKCANCELVisible
        {
            get { return OKCANCELVisible_; }
            set { OKCANCELVisible_ = value; RaisePropertyChanged("OKCANCELVisible"); }
        }

        public Visibility OKVisible
        {
            get { return OKVisible_; }
            set { OKVisible_ = value; RaisePropertyChanged("OKVisible"); }
        }

        public string Message
        {
            get { return Message_; }
            set { Message_ = value; RaisePropertyChanged("Message"); }
        }

        public void OKCommand(Window window)
        {
            window.DialogResult = true;
        }

        private void CancleCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void OnReceiveMessageAction(PopUpMessageCls o)
        {
            if (o.MessageType == enMessageType.OK) { OKCANCELVisible = Visibility.Hidden; OKVisible = Visibility.Visible; }
            else if(o.MessageType == enMessageType.OKCANCEL){ OKCANCELVisible = Visibility.Visible; OKVisible = Visibility.Hidden; }
            else if(o.MessageType == enMessageType.NONE) { OKCANCELVisible = Visibility.Hidden; OKVisible = Visibility.Hidden; }
            Message = o.Message;
        }
    }
}
