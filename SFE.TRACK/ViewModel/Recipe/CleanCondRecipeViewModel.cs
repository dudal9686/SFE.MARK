using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Controls;
using System.IO;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class CleanCondRecipeViewModel : ViewModelBase
    {
        private List<DirFileListCls> FileList_ = new List<DirFileListCls>();
        private CleanCondDataCls RecipeData_ = new CleanCondDataCls();
        public CleanCondStepCls RecipeStep { get; set; } = new CleanCondStepCls();
        public DirFileListCls RecipeFileInfo { get; set; } = null;

        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }
        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;

        private float fGridValue = 0;
        private string sGridValue = string.Empty;

        public CleanCondRecipeViewModel()
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

        public CleanCondDataCls RecipeData
        {
            get { return RecipeData_; }
            set { RecipeData_ = value; RaisePropertyChanged("RecipeData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Clean Condition] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"D:\SFE_RECIPE\CleanCondRecipe\" + newFileName + ".csv");

                    if (!fi.Exists)
                    {
                        fi.Create();
                        GetRecipe();
                        RecipeDetailSelectedIndex = -1;

                        for (int i = 0; i < Global.CleanCondRecipeFileList.Count; i++)
                        {
                            DirFileListCls file = Global.CleanCondRecipeFileList[i] as DirFileListCls;
                            if (file.FileName == newFileName)
                            {
                                RecipeFileInfo = file;
                                RecipeListSelectedIndex = i;
                                LoadListCommand();
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void LoadListCommand()
        {
            RecipeData.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadCleanCondRecipe(RecipeFileInfo.FileFullName, ref RecipeData_);
                if (RecipeData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Clean Condition] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Clean Condition] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Clean Condition] Change Process Name?"))
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
            CleanCondStepCls stepData = new CleanCondStepCls();
            if (RecipeDetailSelectedIndex < 0) RecipeData.StepList.Add(stepData);
            else RecipeData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < RecipeData.StepList.Count; i++)
            {
                CleanCondStepCls step = RecipeData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SaveCleanCondRecipe(RecipeFileInfo.FileFullName, RecipeData);
        }

        private void DeleteDetailCommand()
        {
            if (RecipeStep != null)
            {
                RecipeData.StepList.Remove(RecipeStep);

                for (int i = 0; i < RecipeData.StepList.Count; i++)
                {
                    CleanCondStepCls step = RecipeData.StepList[i];
                    step.Index = i + 1;
                }
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
                    if(Global.GetModuleOpen("SYSTEM", RecipeStep.BlockNo, RecipeStep.ModuleNo))
                    {
                        RecipeStep.BlockNo = Global.STModulePopUp.BlockNo;
                        RecipeStep.ModuleNo = Global.STModulePopUp.ModuleNo;
                    }
                    break;
                case 2:
                    break;
                case 3:
                    fGridValue = Global.KeyPad(RecipeStep.Cnt);
                    RecipeStep.Cnt = Convert.ToInt32(fGridValue);
                    break;
                case 4:
                    fGridValue = Global.KeyPad(RecipeStep.Interval);
                    RecipeStep.Interval = Convert.ToInt32(fGridValue);
                    break;
                case 5:
                    break;
                case 6:
                    if(Global.RecipeOpen(enRecipeMenu.CLEAN_COND, RecipeStep.RecipeName))
                    {
                        RecipeStep.RecipeName = Global.STRecipePopUp.SelectRecipeName;
                    }
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"D:\SFE_RECIPE\CleanCondRecipe\", ref Global.CleanCondRecipeFileList);
            if (Global.CleanCondRecipeFileList.Count() > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.CleanCondRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
