using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Maint
{
    public class UserRegistViewModel : ViewModelBase
    {
        List<LoginInfoCls> UserList_ { get; set; }
        public LoginInfoCls UserInfo { get; set; }
        public RelayCommand<object> LevelClickRelayCommand { get; set; }
        public RelayCommand CreateRelayCommand { get; set; }
        public RelayCommand ModifyRelayCommand { get; set; }
        public RelayCommand DeleteRelayCommand { get; set; }
        int SelectedIndex_ = -1;
        string level = "All";

        public UserRegistViewModel()
        {
            UserList = Global.STUserList;
            if (UserList.Count > 0) SelectedIndex = 0;

            LevelClickRelayCommand = new RelayCommand<object>(LevelClickCommand);
            CreateRelayCommand = new RelayCommand(CreateCommand);
            ModifyRelayCommand = new RelayCommand(ModifyCommand);
            DeleteRelayCommand = new RelayCommand(DeleteCommand);
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set { SelectedIndex_ = value; RaisePropertyChanged("SelectedIndex"); }
        }

        public List<LoginInfoCls> UserList
        {
            get { return UserList_; }
            set { UserList_ = value; RaisePropertyChanged("UserList"); }
        }

        private void LevelClickCommand(object o)
        {
            level = o.ToString();
            GetLevelList();
        }

        private void GetLevelList()
        {
            if (level == "All") UserList = Global.STUserList.ToList();
            else UserList = Global.STUserList.FindAll(x => x.AuthLevel == (enAuthLevel) Convert.ToInt32(level)).ToList();

            if (UserList.Count > 0) SelectedIndex = 0;
            else SelectedIndex = -1;
        }

        private void CreateCommand()
        {
            View.Account.AccountModify regist = new View.Account.AccountModify();
            Global.STUserRegistMessage.PassWordBox = null;
            Global.STUserRegistMessage.Type = 0;
            regist.Owner = System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STUserRegistMessage);
            regist.ShowDialog();

            if (regist.DialogResult.HasValue && regist.DialogResult.Value)
            {
                Global.STDataAccess.ReadUserInfo();
                GetLevelList();
            }
        }

        private void ModifyCommand()
        {
            if (UserInfo == null) return;

            View.Account.AccountModify regist = new View.Account.AccountModify();
            Global.STUserRegistMessage.ID = UserInfo.ID;
            Global.STUserRegistMessage.PassWord = UserInfo.PassWord;
            Global.STUserRegistMessage.Type = 1;
            Global.STUserRegistMessage.PassWordBox = regist.tbLogInPW;
            Global.STUserRegistMessage.AuthLevel = (int)UserInfo.AuthLevel;
            regist.Owner = System.Windows.Application.Current.MainWindow;
            Messenger.Default.Send(Global.STUserRegistMessage);
            regist.ShowDialog();

            if (regist.DialogResult.HasValue && regist.DialogResult.Value)
            {
                Global.STDataAccess.ReadUserInfo();
                GetLevelList();
            }
        }

        private void DeleteCommand()
        {
            if (UserInfo == null) return;

            if(Global.STLoginInfo.ID == UserInfo.ID)
            {
                Global.MessageOpen(enMessageType.OK, "This is the logged in ID.");
                return;
            }

            if(Global.MessageOpen(enMessageType.OKCANCEL, string.Format("[{0}] Are you sure you want to delete it?", UserInfo.ID)))
            {
                Global.STDataAccess.DeleteUserInfo(UserInfo.ID);
                Global.STDataAccess.ReadUserInfo();
                GetLevelList();
            }
        }
    }
}
