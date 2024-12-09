using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Util
{
    public class CassetteConfigViewModel : ViewModelBase
    {
        public List<WaferCls> FoupList1 { get; set; }
        public List<WaferCls> FoupList2 { get; set; }
        public List<WaferCls> FoupList3 { get; set; }
        public List<WaferCls> FoupList4 { get; set; }
        WaferCls WaferInfo_ { get; set; }
        public RelayCommand SaveRelayCommand { get; set; }

        int SelectedIndex1_ = 0;
        int SelectedIndex2_ = -1;
        int SelectedIndex3_ = -1;
        int SelectedIndex4_ = -1;
        public CassetteConfigViewModel()
        {
            CassetteInit();
            SaveRelayCommand = new RelayCommand(SaveCommand);
        }

        public WaferCls WaferInfo
        {
            get { return WaferInfo_; }
            set { WaferInfo_ = value; RaisePropertyChanged("WaferInfo"); }
        }

        public int SelectedIndex1
        {
            get { return SelectedIndex1_; }
            set { SelectedIndex1_ = value;
                if (SelectedIndex1 != -1) { SelectedIndex2 = -1; SelectedIndex3 = -1; SelectedIndex4 = -1; }
                RaisePropertyChanged("SelectedIndex1"); }
        }

        public int SelectedIndex2
        {
            get { return SelectedIndex2_; }
            set { SelectedIndex2_ = value; 
                if(SelectedIndex2 != -1) { SelectedIndex1 = -1; SelectedIndex3 = -1; SelectedIndex4 = -1; }
                RaisePropertyChanged("SelectedIndex2"); }
        }

        public int SelectedIndex3
        {
            get { return SelectedIndex3_; }
            set { SelectedIndex3_ = value;
                if (SelectedIndex3 != -1) { SelectedIndex1 = -1; SelectedIndex2 = -1; SelectedIndex4 = -1; }
                RaisePropertyChanged("SelectedIndex3"); }
        }

        public int SelectedIndex4
        {
            get { return SelectedIndex4_; }
            set { SelectedIndex4_ = value;
                if (SelectedIndex4 != -1) { SelectedIndex1 = -1; SelectedIndex2 = -1; SelectedIndex3 = -1; }
                RaisePropertyChanged("SelectedIndex4"); }
        }

        public void CassetteInit()
        {
            FoupList1 = Global.STWaferList.FindAll(x => x.BlockNo == 1 && x.ModuleNo == 1);
            FoupList1.Reverse();
            FoupList2 = Global.STWaferList.FindAll(x => x.BlockNo == 1 && x.ModuleNo == 2);
            FoupList2.Reverse();
            FoupList3 = Global.STWaferList.FindAll(x => x.BlockNo == 1 && x.ModuleNo == 3);
            FoupList3.Reverse();
            FoupList4 = Global.STWaferList.FindAll(x => x.BlockNo == 1 && x.ModuleNo == 4);
            FoupList4.Reverse();
        }

        private void SaveCommand()
        {
            if (Global.STDataAccess.SetWafer()) Global.MessageOpen(enMessageType.OK, "Saved");
            else Global.MessageOpen(enMessageType.OK, "Not Saved");
        }
    }

    public class UseValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           if(System.Convert.ToBoolean(value) == true)  return new SolidColorBrush(Colors.White);

            return new SolidColorBrush(Colors.DarkGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
