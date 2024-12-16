using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class SelectRecipeViewModel : ViewModelBase
    {
        public RelayCommand<Window> OKRelayCommand { get; set; }
        public RelayCommand<Window> CancelRelayCommand { get; set; }
        public RelayCommand<object> GridDoubleClickRelayCommand { get; set; }
        private ObservableCollection<DirFileListCls> list_ = null;// new List<DirFileListCls>();
        public RelayCommand<object> CheckClickRelayCommand { get; set; }
        DirFileListCls SelectedItem_ { get; set; }
        int SelectedIndex_ = -1;

        public SelectRecipeViewModel()
        {
            Messenger.Default.Register<PopUpRecipeCls>(this, OnReceiveMessageAction);
            OKRelayCommand = new RelayCommand<Window>(OKCommand);
            CancelRelayCommand = new RelayCommand<Window>(CancelCommand);
            GridDoubleClickRelayCommand = new RelayCommand<object>(GridDoubleClickCommand);
            CheckClickRelayCommand = new RelayCommand<object>(CheckClickCommand);
        }

        ~SelectRecipeViewModel()
        {
            Messenger.Default.Unregister<PopUpRecipeCls>(this, OnReceiveMessageAction);
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

        public DirFileListCls SelectedItem
        {
            get { return SelectedItem_; }
            set { 
                SelectedItem_ = value; 
                RaisePropertyChanged("SelectedItem"); }
        }

        public ObservableCollection<DirFileListCls> list
        {
            get { return list_; }
            set { list_ = value; RaisePropertyChanged("list"); }
        }

        private void OnReceiveMessageAction(PopUpRecipeCls obj)
        {
            list = null;
            switch (obj.RecipeMenu)
            {
                case enRecipeMenu.ADH_DUMMY_COND:
                    list = Global.ADHDummyCondRecipeFileList;
                    break;
                case enRecipeMenu.ADH_DUMMY_SEQ:
                    list = Global.ADHDummySeqRecipeFileList;
                    break;
                case enRecipeMenu.ADH_PROCESS:
                    list = Global.ADHProcessRecipeFileList;
                    break;
                case enRecipeMenu.CLEAN_COND:
                    list = Global.CleanCondRecipeFileList;
                    break;
                case enRecipeMenu.COT_CLEAN:
                    list = Global.CoaterCleanRecipeFileList;
                    break;
                case enRecipeMenu.COT_DUMMY_COND:
                    list = Global.CoaterDummyCondRecipeFileList;
                    break;
                case enRecipeMenu.COT_DUMMY_SEQ:
                    list = Global.CoaterDummySeqRecipeFileList;
                    break;
                case enRecipeMenu.COT_PROCESS:
                    list = Global.CoaterProcessRecipeFileList;
                    break;
                case enRecipeMenu.CPL_PROCESS:
                    list = Global.CPLProcessRecipeFileList;
                    break;
                case enRecipeMenu.DEV_CLEAN:
                    list = Global.DevCleanRecipeFileList;
                    break;
                case enRecipeMenu.DEV_DUMMY_COND:
                    list = Global.DevDummyCondRecipeFileList;
                    break;
                case enRecipeMenu.DEV_DUMMY_SEQ:
                    list = Global.DevDummySeqRecipeFileList;
                    break;
                case enRecipeMenu.DEV_PROCESS:
                    list = Global.DevProcessRecipeFileList;
                    break;
                case enRecipeMenu.DUMMY_COND_LINK:
                    list = Global.DummyCondLinkRecipeFileList;
                    break;
                case enRecipeMenu.HHP_PROCESS:
                    list = Global.HHPProcessRecipeFileList;
                    break;
                case enRecipeMenu.LHP_PROCESS:
                    list = Global.LHPProcessRecipeFileList;
                    break;
                case enRecipeMenu.PUMP:
                    list = Global.PumpRecipeFileList;
                    break;
                case enRecipeMenu.SYSTEM:
                    list = Global.SystemRecipeFileList;
                    break;
                case enRecipeMenu.TCP_PROCESS:
                    list = Global.TCPProcessRecipeFileList;
                    break;
                case enRecipeMenu.WAFER_FLOW:
                    list = Global.WaferFlowRecipeFileList;
                    break;
                case enRecipeMenu.JOBINFO:
                    list = Global.JobInfoFileList;
                    break;
            }

            if (list != null)
            {
                foreach (DirFileListCls file in list)
                {
                    if (file.FileName == obj.RecipeName) file.IsCheck = true;
                    else file.IsCheck = false;
                }
            }
        }

        private void OKCommand(Window window)
        {
            bool isCheck = false;
            foreach(DirFileListCls file in list)
            {
                if (file.IsCheck)
                {
                    Global.STRecipePopUp.SelectRecipeName = file.FileName;
                    isCheck = true;
                    break;
                }
            }

            if(!isCheck)
            {
                Global.MessageOpen(enMessageType.OK, "Please select a file.");
                return;
            }
            
            window.DialogResult = true;
        }

        private void CancelCommand(Window window)
        {
            window.DialogResult = false;
        }
        private void GridDoubleClickCommand(object o)
        {
            for(int i = 0; i < list.Count; i++)
            {
                DirFileListCls file = list[i] as DirFileListCls;
                if (SelectedIndex == i) file.IsCheck = true;
                else file.IsCheck = false;
            }
        }

        private void CheckClickCommand(object o)
        {
            DataGridRow row = o as DataGridRow;
            if (row == null) return;

            DirFileListCls file = row.DataContext as DirFileListCls;
            if (file == null) return;
            
            foreach(DirFileListCls item in list)
            {
                if (file != item) item.IsCheck = false;
            }

        }
    }
}
