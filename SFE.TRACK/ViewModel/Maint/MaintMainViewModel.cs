using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Maint
{
    public class MaintMainViewModel : ViewModelBase
    {
        bool IsSupport_ = true;
        bool IsChamber_ = false;
        bool IsMaint_ = false;
        bool IsRegist_ = false;

        public bool IsSupport
        {
            get { return IsSupport_; }
            set { IsSupport_ = value; RaisePropertyChanged("IsSupport"); }
        }

        public bool IsChamber
        {
            get { return IsChamber_; }
            set { IsChamber_ = value; RaisePropertyChanged("IsChamber"); }
        }

        public bool IsMaint
        {
            get { return IsMaint_; }
            set { IsMaint_ = value; RaisePropertyChanged("IsMaint"); }
        }

        public bool IsRegist
        {
            get { return IsRegist_; }
            set { IsRegist_ = value; RaisePropertyChanged("IsRegist"); }
        }
    }
}
