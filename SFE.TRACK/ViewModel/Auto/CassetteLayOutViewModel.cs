using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Auto
{
    public class CassetteLayOutViewModel : ViewModelBase
    {
        bool isCheck = false;
        public RelayCommand<string> CassetteScanRelayCommand { get; set; }
        public CassetteLayOutViewModel()
        {
            CassetteScanRelayCommand = new RelayCommand<string>(CassetteScanCommand);
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; 
                
            foreach(WaferCls wafer in Global.STWaferList)
            {
                if (IsCheck) wafer.Diplay = wafer.Recipe.Name;
                else wafer.Diplay = string.Format("{0}-{1}", wafer.ModuleNo, wafer.Index);
            }

            RaisePropertyChanged("IsCheck"); }
        }

        public void CassetteScanCommand(string cst)
        {
            Console.WriteLine(cst);
        }
    }
}
