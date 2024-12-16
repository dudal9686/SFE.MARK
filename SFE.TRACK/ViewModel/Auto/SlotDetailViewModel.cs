using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SFE.TRACK.Model;

namespace SFE.TRACK.ViewModel.Auto
{
    public class SlotDetailViewModel : ViewModelBase
    {
        FoupCls foup { get; set; }
        WaferCls wafer { get; set; }
        public RelayCommand SetRecipeRelayCommand { get; set; }
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
         public RelayCommand<object> FoupDetailDoubleClickRelayCommand { get; set; }
        int selectedIndex = 0;
        public SlotDetailViewModel()
        {
            SetRecipeRelayCommand = new RelayCommand(SetRecipeCommand);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            FoupDetailDoubleClickRelayCommand = new RelayCommand<object>(FoupDetailDoubleClickCommand);
        }

        public void SetValue(FoupCls foup)
        {
            Foup = foup;
            if(Foup.FoupWaferList[0].Index == 1) Foup.FoupWaferList.Reverse();
            if (Foup != null) SelectedIndex = 0;
        }

        public FoupCls Foup
        {
            get { return foup; }
            set { foup = value; RaisePropertyChanged("Foup"); }
        }

        public WaferCls Wafer
        {
            get { return wafer; }
            set { wafer = value; RaisePropertyChanged("Wafer"); }
        }

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged("SelectedIndex"); }
        }

        private void SetRecipeCommand()
        {
            if(Wafer.WaferState != enWaferState.WAFER_EXIST)
            {
                Global.MessageOpen(enMessageType.OK, "Cannot specify recipe file.");
                return;
            }

            if(Global.RecipeOpen(enRecipeMenu.WAFER_FLOW, Wafer.Recipe.Name))
            {
                Wafer.Recipe.Name = Global.STRecipePopUp.SelectRecipeName;
            }
        }

        private void OKCommand(Window window)
        {
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void FoupDetailDoubleClickCommand(object o)
        {
            if (!Wafer.Use) return;
            if (Wafer.WaferState == enWaferState.WAFER_EXIST) { Wafer.WaferState = enWaferState.WAFER_NONE; Wafer.Recipe.Name = string.Empty; }
            else if (Wafer.WaferState == enWaferState.WAFER_NONE) Wafer.WaferState = enWaferState.WAFER_EXIST;
        }
    }
}
