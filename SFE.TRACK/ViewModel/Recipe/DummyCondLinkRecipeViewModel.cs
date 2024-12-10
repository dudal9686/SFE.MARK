using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using System.Windows.Controls;
using System.IO;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class DummyCondLinkRecipeViewModel : ViewModelBase
    {
        private DummyConditionLinkCls RecipeData_ = new DummyConditionLinkCls();
        public DummyConditionLinkStepCls RecipeStep { get; set; } = new DummyConditionLinkStepCls();
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

        public DummyCondLinkRecipeViewModel()
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

        public DummyConditionLinkCls RecipeData
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dummy Condition Link]\n Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\DummyCondLinkRecipe\" + newFileName + ".csv");

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
            RecipeData.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadDummyCondLinkRecipe(RecipeFileInfo.FileFullName, ref RecipeData_);
                if(RecipeData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dummy Condition Link]\n Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dummy Condition Link]\n Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dummy Condition Link]\n Change Process Name?"))
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
            DummyConditionLinkStepCls stepData = new DummyConditionLinkStepCls();
            if (RecipeDetailSelectedIndex < 0) RecipeData.StepList.Add(stepData);
            else RecipeData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < RecipeData.StepList.Count; i++)
            {
                DummyConditionLinkStepCls step = RecipeData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SaveDummyCondLinkRecipe(RecipeFileInfo.FileFullName, RecipeData);
        }

        private void DeleteDetailCommand()
        {
            if (RecipeStep != null)
            {
                RecipeData.StepList.Remove(RecipeStep);

                for (int i = 0; i < RecipeData.StepList.Count; i++)
                {
                    DummyConditionLinkStepCls step = RecipeData.StepList[i];
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
                    if(Global.GetModuleOpen("DUMMY", RecipeStep.BlockNo, RecipeStep.ModuleNo))
                    {
                        RecipeStep.BlockNo = Global.STModulePopUp.BlockNo;
                        RecipeStep.ModuleNo = Global.STModulePopUp.ModuleNo;
                    }
                    break;
                case 2:

                    if(RecipeStep.ModuleNo == 0)
                    {
                        Global.MessageOpen(enMessageType.OK, "Please select a module.");
                        return;
                    }

                    enRecipeMenu menu = enRecipeMenu.ADH_DUMMY_COND;
                    Model.ModuleBaseCls mod = Global.GetModule(RecipeStep.BlockNo, RecipeStep.ModuleNo);
                    if (mod.MachineName.IndexOf("DEV") != -1) menu = enRecipeMenu.DEV_DUMMY_COND; 
                    else if (mod.MachineName.IndexOf("COT") != -1) menu = enRecipeMenu.COT_DUMMY_COND;
                    else if (mod.MachineName.IndexOf("ADH") != -1) menu = enRecipeMenu.ADH_DUMMY_COND;

                    if (Global.RecipeOpen(menu, RecipeStep.RecipeName))
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
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\DummyCondLinkRecipe\", ref Global.DummyCondLinkRecipeFileList);
            if (Global.DummyCondLinkRecipeFileList.Count() > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.DummyCondLinkRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
