using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Motor
{    
    public class MotorMainViewModel : ViewModelBase
    {
        List<MotorIODisplayCls> MainList_ = new List<MotorIODisplayCls>();
        MotorIODisplayCls mainData;
        MotorIODisplayDetailCls subData;
        List<string> titlelist = new List<string>();
        bool isScreenChange = true;
        public MotorMainViewModel()
        {
            Messenger.Default.Register<MotorIOMessageCls>(this, OnReceiveMessageAction);
            #region Data
            mainData = new MotorIODisplayCls();
            mainData.MainTitle = "MOTOR";

            foreach(AxisInfoCls axis in Global.STAxis)
            {
                subData = new MotorIODisplayDetailCls();
                subData.SubTitle = axis.AxisID.Replace("_", "-");
                subData.AxisType = (enAxisType)axis.AxisNo;
                mainData.MenuList.Add(subData);
            }

            MainList.Add(mainData);
            //----------------------------------------------------------------
            titlelist.Clear();
            mainData = new MotorIODisplayCls();
            mainData.MainTitle = "IO";

            subData = new MotorIODisplayDetailCls();
            subData.SubTitle = "ALL";
            subData.AxisType = enAxisType.NONE;
            mainData.MenuList.Add(subData);

            foreach (IODataCls io in Global.STDIList)
            {
                if(!CheckData(io.Alias))
                { 
                    subData = new MotorIODisplayDetailCls();
                    subData.SubTitle = io.Alias;
                    subData.AxisType = enAxisType.NONE;
                    titlelist.Add(io.Alias);
                    mainData.MenuList.Add(subData);
                }
            }

            titlelist.Clear();
            MainList.Add(mainData);
            #endregion

        }

        ~MotorMainViewModel()
        {
            Messenger.Default.Register<MotorIOMessageCls>(this, OnReceiveMessageAction);
        }

        public List<MotorIODisplayCls> MainList
        {
            get { return MainList_; }
            set { MainList_ = value; RaisePropertyChanged("MainList"); }
        }

        private bool CheckData(string title)
        {
            bool isExist = false;
            foreach(string t in titlelist)
            {
                if(t == title)
                {
                    isExist = true;
                    break;
                }
            }

            return isExist;
        }

        private void OnReceiveMessageAction(MotorIOMessageCls o)
        {
            if (o.AxisType != enAxisType.NONE)
            {
                IsScreenChange = true;
                CommonServiceLocator.ServiceLocator.Current.GetInstance<Motor.MotorControlViewModel>().SetDisplay();
            }
            else
            {
                IsScreenChange = false;
                CommonServiceLocator.ServiceLocator.Current.GetInstance<Motor.IOControlViewModel>().SetDisplay();
            }
        }

        public bool IsScreenChange
        {
            get { return isScreenChange; }
            set { isScreenChange = value; RaisePropertyChanged("IsScreenChange"); }
        }
    }

    public class MotorIODisplayCls : ViewModelBase
    {
        string title = string.Empty;
        List<MotorIODisplayDetailCls> list = new List<MotorIODisplayDetailCls>();

        public string MainTitle
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("MainTitle"); }
        }

        public List<MotorIODisplayDetailCls> MenuList
        {
            get { return list; }
            set { list = value; RaisePropertyChanged("MenuList"); }
        }
    }

    public class MotorIODisplayDetailCls : ViewModelBase
    {
        string subTitle = string.Empty;
        enAxisType axisType = enAxisType.NONE;
        public RelayCommand ClickRelayCommand { get; set; }
        
        public IODataCls ioInfo { get; set; }

        public MotorIODisplayDetailCls()
        {
            ClickRelayCommand = new RelayCommand(ClickCommand);
        }

        public string SubTitle
        {
            get { return subTitle; }
            set { subTitle = value; RaisePropertyChanged(""); }
        }

        public enAxisType AxisType
        {
            get { return axisType; }
            set { axisType = value; RaisePropertyChanged("AxisType"); }
        }

        private void ClickCommand()
        {
            Global.STMotorIODataMessage.AxisType = AxisType;
            Global.STMotorIODataMessage.Title = SubTitle;
            Messenger.Default.Send(Global.STMotorIODataMessage);
            Console.WriteLine(SubTitle);
        }
    }
}
