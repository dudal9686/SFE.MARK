using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Account
{
    public class UserAccountViewModel : ViewModelBase
    {
        public RelayCommand<object> PassWordClickRelayCommand { get; set; }
        public RelayCommand IDClickRelayCommand { get; set; }
        public RelayCommand<object> LoginRelayCommand { get; set; }
        public RelayCommand<object> CancelRelayCommand { get; set; }

        string ID_ = string.Empty;

        public UserAccountViewModel()
        {
            PassWordClickRelayCommand = new RelayCommand<object>(PassWordClickCommand);
            IDClickRelayCommand = new RelayCommand(IDClickCommand);
            LoginRelayCommand = new RelayCommand<object>(LoginCommand);
            CancelRelayCommand = new RelayCommand<object>(CancelCommand);
        }

        public string ID
        {
            get { return ID_; }
            set { ID_ = value; RaisePropertyChanged("ID"); }
        }

        private void PassWordClickCommand(object o)
        {
            PasswordBox password = o as PasswordBox;

            string pass = string.Empty;
            if(Global.KeyBoard(ref pass))
            {
                password.Password = pass;
            }
        }

        private void IDClickCommand()
        {
            string id = ID;
            if (Global.KeyBoard(ref id))
            {
                ID = id;
            }
        }

        private void LoginCommand(object o)
        {
            var values = (object[])o;
            PasswordBox password = values[0] as PasswordBox;
            Window window = values[1] as Window;

            bool isMatch = false;

            foreach(LoginInfoCls info in Global.STUserList)
            {
                if(ID == info.ID && password.Password == info.PassWord)
                {
                    Global.STLoginInfo.ID = info.ID;
                    Global.STLoginInfo.PassWord = info.PassWord;
                    Global.STLoginInfo.AuthLevel = info.AuthLevel;
                    isMatch = true;
                    break;
                }
            }

            if(!isMatch)
            {
                Global.MessageOpen(enMessageType.OK, "Invalid.");
            }

            ID = string.Empty;
            password.Password = string.Empty;
            window.DialogResult = true;
        }

        private void CancelCommand(object o)
        {
            var values = (object[])o;
            PasswordBox password = values[0] as PasswordBox;
            Window window = values[1] as Window;

            ID = string.Empty;
            password.Password = string.Empty;
            window.DialogResult = false;
        }
    }
}
