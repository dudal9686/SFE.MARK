using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Account
{
    public class AccountModifyViewModel : ViewModelBase
    {
        public List<string> AuthList_ { get; set; } = new List<string>();
        public RelayCommand IDClickRelayCommand { get; set; }
        public RelayCommand<object> PassWordClickRelayCommand { get; set; }
        public RelayCommand<object> OKRelayCommand { get; set; }
        public RelayCommand<object> CancelRelayCommand { get; set; }
        string ID_ = string.Empty;
        int AuthSelectedIndex_ = 0;
        bool IsIDEnable_ = true;
        public AccountModifyViewModel()
        {
            Messenger.Default.Register<PopUpUserRegistCls>(this, OnReceiveMessageAction);
            Init();
            IDClickRelayCommand = new RelayCommand(IDClickCommand);
            PassWordClickRelayCommand = new RelayCommand<object>(PassWordClickCommand);
            OKRelayCommand = new RelayCommand<object>(OKCommand);
            CancelRelayCommand = new RelayCommand<object>(CancelCommand);
        }

        ~AccountModifyViewModel()
        {
            Messenger.Default.Unregister<PopUpUserRegistCls>(this, OnReceiveMessageAction);
        }

        private void Init()
        {
            AuthList.Clear();
            AuthList.Add("-SELECT-");
            AuthList.Add(enAuthLevel.NOMAL_OP.ToString());
            AuthList.Add(enAuthLevel.PROCESS_OP.ToString());
            AuthList.Add(enAuthLevel.EQUIPMENT_OP.ToString());
            AuthList.Add(enAuthLevel.SERVICE_OP.ToString());
        }

        public List<string> AuthList
        {
            get { return AuthList_; }
            set { AuthList_ = value; RaisePropertyChanged("AuthList"); }
        }

        public bool IsIDEnable
        {
            get { return IsIDEnable_; }
            set { IsIDEnable_ = value; RaisePropertyChanged("IsIDEnable"); }
        }

        public int AuthSelectedIndex
        {
            get { return AuthSelectedIndex_; }
            set { AuthSelectedIndex_ = value; RaisePropertyChanged("AuthSelectedIndex"); }
        }

        public string ID
        {
            get { return ID_; }
            set { ID_ = value; RaisePropertyChanged("ID"); }
        }

        private void IDClickCommand()
        {
            string id = ID;
            if (Global.KeyBoard(ref id))
            {
                ID = id;
            }
        }

        public void OnReceiveMessageAction(PopUpUserRegistCls o)
        {
            if (o.Type == 0) IsIDEnable = true;
            else if (o.Type == 1) //Modify
            {
                ID = Global.STUserRegistMessage.ID;
                Global.STUserRegistMessage.PassWordBox.Password = Global.STUserRegistMessage.PassWord;
                AuthSelectedIndex = Global.STUserRegistMessage.AuthLevel;

                Global.STUserRegistMessage.PassWordBox = null;
                IsIDEnable = false;
            }
        }

        private void PassWordClickCommand(object o)
        {
            PasswordBox password = o as PasswordBox;

            string pass = password.Password;
            if (Global.KeyBoard(ref pass))
            {
                password.Password = pass;
            }
        }

        private void OKCommand(object o)
        {
            var values = (object[])o;
            PasswordBox password = values[0] as PasswordBox;
            Window window = values[1] as Window;
            bool isSuccess = false;

            if(AuthSelectedIndex == 0)
            {
                Global.MessageOpen(enMessageType.OK, "Please select your permissions.");
                return;
            }

            if(ID == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please enter your ID.");
                return;
            }

            if(password.Password == string.Empty)
            {
                Global.MessageOpen(enMessageType.OK, "Please enter your Password.");
                return;
            }

            if (Global.STUserRegistMessage.Type == 0)
            {
                if(GetIDDuplication())
                {
                    Global.MessageOpen(enMessageType.OK, "The ID is duplicated.");
                    return;
                }

                isSuccess = Global.STDataAccess.CreateUserInfo(ID, password.Password, AuthSelectedIndex);
            }
            else
            {
                isSuccess = Global.STDataAccess.ModifyUserInfo(ID, password.Password, AuthSelectedIndex);
            }

            if(isSuccess)
            {
                Global.MessageOpen(enMessageType.OK, "Saved");
            }
            else
            {
                Global.MessageOpen(enMessageType.OK, "Not Saved.");
                return;
            }

            AuthSelectedIndex = 0;
            ID = string.Empty;
            password.Password = string.Empty;
            window.DialogResult = true;
        }

        private void CancelCommand(object o)
        {
            var values = (object[])o;
            PasswordBox password = values[0] as PasswordBox;
            Window window = values[1] as Window;

            AuthSelectedIndex = 0;
            ID = string.Empty;
            password.Password = string.Empty;
            window.DialogResult = false;
        }

        private bool GetIDDuplication()
        {
            bool isDuplication = false;
            foreach (LoginInfoCls info in Global.STUserList)
            {
                if (info.ID == ID)
                {
                    isDuplication = true;
                    break;
                }
            }
            return isDuplication;
        }
    }
}
