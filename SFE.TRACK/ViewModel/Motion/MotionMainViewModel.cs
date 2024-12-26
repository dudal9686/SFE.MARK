using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Motion
{
    public class MotionMainViewModel : ViewModelBase
    {
        bool isCRA = true;
        bool isPRA = false;
        bool isIRA = false;

        public MotionMainViewModel()
        {

        }

        public bool IsCRA
        {
            get { return isCRA; }
            set { isCRA = value; RaisePropertyChanged("IsCRA"); }
        }

        public bool IsPRA
        {
            get { return isPRA; }
            set { isPRA = value; RaisePropertyChanged("IsPRA"); }
        }

        public bool IsIRA
        {
            get { return isIRA; }
            set { isIRA = value; RaisePropertyChanged("IsIRA"); }
        }
    }
}
