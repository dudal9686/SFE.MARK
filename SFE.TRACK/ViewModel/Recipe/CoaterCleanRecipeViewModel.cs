using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class CoaterCleanRecipeViewModel : ViewModelBase
    {
        private CleanDataCls CotData_ = new CleanDataCls();
        public CleanStepCls CotStepData { get; set; } = null;
        public DirFileListCls RecipeFileInfo { get; set; } = null;

        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }

        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }

        public RelayCommand PumpRecipeRelayCommand { get; set; }
        public RelayCommand StopRangeRelayCommand { get; set; }
        public RelayCommand AlarmRangeRelayCommand { get; set; }

        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;
        public CoaterCleanRecipeViewModel()
        {
            GetRecipe();

            AddListRelayCommand = new RelayCommand(AddListCommand);
            LoadListRelayCommand = new RelayCommand(LoadListCommand);
            SaveAsListRelayCommand = new RelayCommand(SaveAsListCommand);
            DeleteListRelayCommand = new RelayCommand(DeleteListCommand);
            ReNameListRelayCommand = new RelayCommand(ReNameListCommand);

            AddDetailRelayCommand = new RelayCommand(AddDetailCommand);
            SaveDetailRelayCommand = new RelayCommand(SaveDetailCommand);
            DeleteDetailRelayCommand = new RelayCommand(DeleteDetailCommand);

            PumpRecipeRelayCommand = new RelayCommand(PumpRecipeCommand);
            StopRangeRelayCommand = new RelayCommand(StopRangeCommand);
            AlarmRangeRelayCommand = new RelayCommand(AlarmRangeCommand);
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

        public CleanDataCls CotData
        {
            get { return CotData_; }
            set { CotData_ = value; RaisePropertyChanged("CotData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater Clean] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\CleanCOTRecipe\" + newFileName + ".csv");

                    if (!fi.Exists)
                    {
                        fi.Create();
                        GetRecipe();
                        RecipeDetailSelectedIndex = -1;
                    }
                }
            }
        }

        private void LoadListCommand()
        {
            CotData.Clear();
            CotData_.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadCleanCOTRecipe(RecipeFileInfo.FileFullName, ref CotData_);
                RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater Clean] Do you Make This file?"))
                {
                    string saveAsfile = RecipeFileInfo.FileName;
                    if (Global.KeyBoard(ref saveAsfile))
                    {
                        if (File.Exists(RecipeFileInfo.FilePath + saveAsfile + ".csv"))
                        {
                            Global.MessageOpen(enMessageType.OKCANCEL, string.Format("[{0}] Exsit file.", saveAsfile));
                            return;
                        }

                        File.Exists(saveAsfile);
                        File.Copy(RecipeFileInfo.FileFullName, RecipeFileInfo.FilePath + saveAsfile + ".csv");
                        GetRecipe();
                    }
                }
            }
        }

        private void DeleteListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater Clean] Do you want to delete the file?"))
                {
                    File.Delete(RecipeFileInfo.FileFullName);
                    GetRecipe();
                }
            }
        }

        private void ReNameListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater Clean] Change Process Name?"))
                {
                    string reNamefile = RecipeFileInfo.FileName;
                    if (Global.KeyBoard(ref reNamefile))
                    {
                        if (File.Exists(RecipeFileInfo.FilePath + reNamefile + ".csv"))
                        {
                            Global.MessageOpen(enMessageType.OKCANCEL, string.Format("[{0}] Exsit file.", reNamefile));
                            return;
                        }

                        File.Move(RecipeFileInfo.FileFullName, RecipeFileInfo.FilePath + reNamefile + ".csv");
                        GetRecipe();
                    }
                }
            }
        }

        private void AddDetailCommand()
        {
            CleanStepCls stepData = new CleanStepCls();
            if (RecipeDetailSelectedIndex < 0) CotData.StepList.Add(stepData);
            else CotData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < CotData.StepList.Count; i++)
            {
                CleanStepCls step = CotData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SaveCleanCOTRecipe(RecipeFileInfo.FileFullName, CotData);
        }

        private void DeleteDetailCommand()
        {
            if (CotStepData != null)
            {
                CotData.StepList.Remove(CotStepData);

                for (int i = 0; i < CotData.StepList.Count; i++)
                {
                    CleanStepCls step = CotData.StepList[i];
                    step.Index = i + 1;
                }
            }
        }

        private void StopRangeCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(CotData.StopRange);
                CotData.StopRange = Convert.ToInt32(value);
            }
        }

        private void AlarmRangeCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(CotData.AlarmRange);
                CotData.AlarmRange = Convert.ToInt32(value);
            }
        }

        private void PumpRecipeCommand()
        {
            if (Global.RecipeOpen(enRecipeMenu.PUMP, CotData.PumpRecipe))
            {
                CotData.PumpRecipe = Global.STRecipePopUp.SelectRecipeName;
            }
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
        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\CleanCOTRecipe\", ref Global.CoaterCleanRecipeFileList);
            if (Global.CoaterCleanRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.CoaterCleanRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
