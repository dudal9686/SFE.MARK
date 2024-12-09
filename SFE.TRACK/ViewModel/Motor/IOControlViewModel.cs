using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Motor
{
    public class IOControlViewModel : ViewModelBase
    {
        string IOInfo_ = string.Empty;
        List<IODataCls> DIList_ { get; set; }
        List<IODataCls> DOList_ { get; set; }
        List<AIODataCls> AIList_ { get; set; }

        public string IOInfo
        {
            get { return IOInfo_; }
            set { IOInfo_ = value; RaisePropertyChanged("IOInfo"); }
        }

        public List<IODataCls> DIList
        {
            get { return DIList_; }
            set { DIList_ = value; RaisePropertyChanged("DIList"); }
        }

        public List<IODataCls> DOList
        {
            get { return DOList_; }
            set { DOList_ = value; RaisePropertyChanged("DOList"); }
        }

        public List<AIODataCls> AIList
        {
            get { return AIList_; }
            set { AIList_ = value; RaisePropertyChanged("AIList"); }
        }

        public void SetDisplay()
        {
            IOInfo = Global.STMotorIODataMessage.Title;
            if (IOInfo == "ALL")
            {
                DIList = Global.STDIList;
                DOList = Global.STDOList;
                AIList = Global.STAIOList;
            }
            else
            {
                DIList = Global.STDIList.FindAll(x=>x.Alias == IOInfo);
                DOList = Global.STDOList.FindAll(x => x.Alias == IOInfo);
                AIList = Global.STAIOList.FindAll(x => x.Alias == IOInfo);
            }
        }
    }
}
