using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.Model
{
    public class SystemCfgCls : ViewModelBase
    {
        int blockNo = 0;
        int moduleNo = 0;
        string title = string.Empty;
        string name = string.Empty;
        string sValue = string.Empty;

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }
        public string Value
        {
            get { return sValue; }
            set { sValue = value; RaisePropertyChanged("Value"); }
        }
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }
    }
}
