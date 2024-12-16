using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class SelectDispenseViewModel : ViewModelBase
    {
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> GridDoubleClickRelayCommand { get; set; }
        List<DispenseInfoCls> DispenseList_;// { get; set; }
        public DispenseInfoCls DispenseStep { get; set; } = null;
        int SelectedIndex_ = -1;
        public SelectDispenseViewModel()
        {
            Messenger.Default.Register<PopUpDispenseCls>(this, OnReceiveMessageAction);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            GridDoubleClickRelayCommand = new RelayCommand<object>(GridDoubleClickCommand);
        }        

        ~SelectDispenseViewModel()
        {

        }

        public List<DispenseInfoCls> DispenseList
        {
            get { return DispenseList_; }
            set { DispenseList_ = value; RaisePropertyChanged("DispenseList"); }
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

        private void OnReceiveMessageAction(PopUpDispenseCls o)
        {
            List<DispenseInfoCls> DispenseList_ = Global.STDispenseList.FindAll(x => x.Type == o.ModuleType.ToString());

            if (o.DummyOrRecipeUse == "RECIPEUSE") DispenseList = DispenseList_.Where(x => x.IsUse == true && x.IsUseRecipe == true).ToList();
            else DispenseList = DispenseList_.Where(x => x.IsUse == true && x.IsUseDummy == true).ToList();

            foreach (DispenseInfoCls info in DispenseList) info.IsCheck = false;

            for (int i = 0; i < Global.STDispenseIndex.Length; i++)
            {
                if (Convert.ToBoolean(o.DispenseValue & Global.STDispenseIndex[i])) CheckDispense(i + 1);
            }
        }

        private void CheckDispense(int dispNo)
        {
            foreach (DispenseInfoCls info in DispenseList)
            {
                if(dispNo == info.DispNo)
                {
                    info.IsCheck = true;
                }
            }
        }

        private Model.ModuleBaseCls GetModule(string mod)
        {
            Model.ModuleBaseCls o = null;
            foreach(Model.ModuleBaseCls module in Global.STModuleList)
            {
                if(module.MachineName.IndexOf(mod) != -1)
                {
                    o = module;
                    break;
                }
            }

            return o;
        }

        private void OKCommand(Window window)
        {
            uint dispValue = 0;
            foreach (DispenseInfoCls dispense in DispenseList)
            {
                if (dispense.IsCheck)
                {
                    dispValue += Global.STDispenseIndex[dispense.DispNo - 1];
                }
            }

            Global.STDispensePopUp.SelectDispenseValue = dispValue;
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }

        private void GridDoubleClickCommand(object o)
        {
            if(!DispenseStep.IsCheck) DispenseStep.IsCheck = true;
            else DispenseStep.IsCheck = false;

            if(Global.STDispensePopUp.DummyOrRecipeUse == "DUMMYUSE" && !Global.STDispensePopUp.IsMultiSelect)
            {
                foreach(DispenseInfoCls info in DispenseList)
                {
                    if (info != DispenseStep) info.IsCheck = false;
                }
            }
        }
    }
}
