using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class DevCleanRecipeViewModel : ViewModelBase
    {
        private CleanDataCls DevData_ = new CleanDataCls();
        public CleanStepCls DevStepData { get; set; } = null;
        public DirFileListCls RecipeFileInfo { get; set; } = null;

        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }

        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }

        public RelayCommand StopRangeRelayCommand { get; set; }
        public RelayCommand AlarmRangeRelayCommand { get; set; }
        public RelayCommand<object> RecipeDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;
        public DevCleanRecipeViewModel()
        {   
            if (Global.DevCleanRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.DevCleanRecipeFileList[0];
                LoadListCommand();
            }

            AddListRelayCommand = new RelayCommand(AddListCommand);
            LoadListRelayCommand = new RelayCommand(LoadListCommand);
            SaveAsListRelayCommand = new RelayCommand(SaveAsListCommand);
            DeleteListRelayCommand = new RelayCommand(DeleteListCommand);
            ReNameListRelayCommand = new RelayCommand(ReNameListCommand);

            AddDetailRelayCommand = new RelayCommand(AddDetailCommand);
            SaveDetailRelayCommand = new RelayCommand(SaveDetailCommand);
            DeleteDetailRelayCommand = new RelayCommand(DeleteDetailCommand);

            StopRangeRelayCommand = new RelayCommand(StopRangeCommand);
            AlarmRangeRelayCommand = new RelayCommand(AlarmRangeCommand);
            RecipeDoubleClickRelayCommand = new RelayCommand<object>(RecipeDoubleClickCommand);
            RecipeDetailDoubleClickRelayCommand = new RelayCommand<object>(RecipeDetailDoubleClickCommand);
        }

        #region Get Set
        public int RecipeListSelectedIndex
        {
            get { return RecipeListSelectedIndex_; }
            set { RecipeListSelectedIndex_ = value; RaisePropertyChanged("RecipeListSelectedIndex"); }
        }

        public int RecipeDetailSelectedIndex
        {
            get { return RecipeDetailSelectedIndex_; }
            set { RecipeDetailSelectedIndex_ = value; RaisePropertyChanged("RecipeDetailSelectedIndex"); }
        }

        public CleanDataCls DevData
        {
            get { return DevData_; }
            set { DevData_ = value; RaisePropertyChanged("DevData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {

        }

        private void LoadListCommand()
        {
            DevData.Clear();
            DevData.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadCleanCOTRecipe(RecipeFileInfo.FileFullName, ref DevData_);
                RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {

        }

        private void DeleteListCommand()
        {

        }

        private void ReNameListCommand()
        {

        }

        private void AddDetailCommand()
        {
            CleanStepCls stepData = new CleanStepCls();
            if (RecipeDetailSelectedIndex < 0) DevData.StepList.Add(stepData);
            else DevData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < DevData.StepList.Count; i++)
            {
                CleanStepCls step = DevData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            if(Global.STDataAccess.SaveCleanCOTRecipe(RecipeFileInfo.FileFullName, DevData)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
        }

        private void DeleteDetailCommand()
        {
            if (DevStepData != null)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to delete it?"))
                {
                    DevData.StepList.Remove(DevStepData);
                    for (int i = 0; i < DevData.StepList.Count; i++)
                    {
                        CleanStepCls step = DevData.StepList[i];
                        step.Index = i + 1;
                    }
                }
            }
        }

        private void StopRangeCommand()
        {

        }

        private void AlarmRangeCommand()
        {

        }
        private void RecipeDoubleClickCommand(object o)
        {
            if (RecipeFileInfo != null) LoadListCommand();
        }
        private void RecipeDetailDoubleClickCommand(object o)
        {
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch (index)
            {
                case 0:
                    break;
                case 1:
                    View.Recipe.SelectModule selectModule = new View.Recipe.SelectModule();
                    selectModule.ShowDialog();
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                default:
                    break;
            }

        }
        #endregion
    }
}
