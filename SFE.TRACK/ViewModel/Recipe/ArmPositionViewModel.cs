using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class ArmPositionViewModel : ViewModelBase
    {
        string[] ArmPositon1 = new string[] { "HOME", /*"DMY DISPN",*/ "BEGIN", "CENTER", /*"CENTER2",*/ "END", "DISPN 1", "WAFER EDGE" };
        string[] ArmPositon2 = new string[] { "HOME", "BEGIN", "CENTER", /*"CENTER2",*/ "END", "DISPN 1", "WAFER EDGE" }; 
        string[] DevArmPosition2 = new string[] { "IN", "OUT" }; //Arm2Position을 같이 쓰다가 Dev가 io로 바뀜
        public List<ObjectDisplayCls> PositionList { get; set; } = new List<ObjectDisplayCls>();
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> GridDoubleClickRelayCommand { get; set; }
        
        int SelectedIndex_ = -1;

        public ArmPositionViewModel()
        {
            Messenger.Default.Register<PopUpArmPositionCls>(this, OnReceiveMessageAction);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            GridDoubleClickRelayCommand = new RelayCommand<object>(GridDoubleClickCommand);
        }

        ~ArmPositionViewModel()
        {
            Messenger.Default.Unregister<PopUpArmPositionCls>(this, OnReceiveMessageAction);            
        }

        public int SelectedIndex
        {
            get { return SelectedIndex_; }
            set
            {
                SelectedIndex_ = value;
                RaisePropertyChanged("SelectedIndex");
            }
        }

        private void OKCommand(Window window)
        {
            foreach (ObjectDisplayCls display in PositionList)
            {
                if (display.IsCheck)
                {
                    Global.STArmPositionPopUp.SelectArmPosition = display.Display;
                    break;
                }
            }

            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }
        private void GridDoubleClickCommand(object o)
        {
            for (int i = 0; i < PositionList.Count; i++)
            {
                ObjectDisplayCls file = PositionList[i] as ObjectDisplayCls;
                if (SelectedIndex == i) file.IsCheck = true;
                else file.IsCheck = false;
            }
        }

        private void OnReceiveMessageAction(PopUpArmPositionCls o)
        {
            PositionList.Clear();
            if(o.ArmType == enArmTpe.ARM1)
            {
                for(int i = 0; i < ArmPositon1.Length; i++)
                {
                    ObjectDisplayCls displayCls = new ObjectDisplayCls();
                    displayCls.Index = i + 1;
                    displayCls.Display = ArmPositon1[i];
                    if (o.ArmPosition == ArmPositon1[i]) displayCls.IsCheck = true;
                    else displayCls.IsCheck = false;
                    PositionList.Add(displayCls);
                }
            }
            else
            {
                if (o.SelectModule == "DEV")
                {
                    for (int i = 0; i < DevArmPosition2.Length; i++)
                    {
                        ObjectDisplayCls displayCls = new ObjectDisplayCls();
                        displayCls.Index = i + 1;
                        displayCls.Display = DevArmPosition2[i];
                        if (o.ArmPosition == DevArmPosition2[i]) displayCls.IsCheck = true;
                        else displayCls.IsCheck = false;
                        PositionList.Add(displayCls);
                    }

                    return;
                }

                for (int i = 0; i < ArmPositon2.Length; i++)
                {
                    ObjectDisplayCls displayCls = new ObjectDisplayCls();
                    displayCls.Index = i + 1;
                    displayCls.Display = ArmPositon2[i];
                    if (o.ArmPosition == ArmPositon2[i]) displayCls.IsCheck = true;
                    else displayCls.IsCheck = false;
                    PositionList.Add(displayCls);
                }
            }
        }
    }

    public class ObjectDisplayCls : ViewModelBase
    {
        int index = 0;
        bool isCheck = false;
        string display = string.Empty;

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("IsCheck"); }
        }

        public string Display
        {
            get { return display; }
            set { display = value; RaisePropertyChanged("Display"); }
        }
    }
}
