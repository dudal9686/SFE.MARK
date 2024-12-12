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
    public class DevDummyCondRecipeViewModel : ViewModelBase
    {
        private DummyConditionCls Recipe_ = new DummyConditionCls();
        public DummyConditionStepCls StepRecipe { get; set; } = null;
        public DirFileListCls RecipeFileInfo { get; set; } = null;

        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }

        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }

        public RelayCommand<object> RecipeDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;

        private float fGridValue = 0;

        public DevDummyCondRecipeViewModel()
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

        public DummyConditionCls Recipe
        {
            get { return Recipe_; }
            set { Recipe_ = value; RaisePropertyChanged("Recipe"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Condition]\n Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\Dummy\Cond\DEV\" + newFileName + ".csv");

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
            Recipe.Clear();
            Recipe.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadDummyCondRecipe(RecipeFileInfo.FileFullName, ref Recipe_);
                if (Recipe.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;

                int dispIndex = 0;

                Model.ModuleBaseCls module = Global.STModuleList.Find(x => x.MachineName.IndexOf("DEV") != -1);

                if (module != null)
                {
                    foreach (DummyConditionStepCls step in Recipe.StepList)
                    {
                        for (int i = 0; i < module.DispenseList.Count; i++)
                        {
                            dispIndex = Global.GetDispenseIndex(step.DispenseNo);

                            if (dispIndex == -1) continue;

                            DispenseInfoCls dispense = module.DispenseList[i];

                            if (dispIndex == dispense.DispNo) { step.DipsenseDisplay = dispense.DispName; break; }
                        }
                    }
                }
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Condition]\n Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Condition]\n Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Condition] Change Process Name?"))
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
            DummyConditionStepCls stepData = new DummyConditionStepCls();
            if (RecipeDetailSelectedIndex < 0) Recipe.StepList.Add(stepData);
            else Recipe.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < Recipe.StepList.Count; i++)
            {
                DummyConditionStepCls step = Recipe.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            if(Global.STDataAccess.SaveDummyCondRecipe(RecipeFileInfo.FileFullName, Recipe)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
        }

        private void DeleteDetailCommand()
        {
            if (StepRecipe != null)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to delete it?"))
                {
                    Recipe.StepList.Remove(StepRecipe);
                    for (int i = 0; i < Recipe.StepList.Count; i++)
                    {
                        DummyConditionStepCls step = Recipe.StepList[i];
                        step.Index = i + 1;
                    }
                }
            }
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
                    if (Global.DispenseInfoOpen(enDispenseModule.DEV, StepRecipe.DispenseNo, "DUMMYUSE", false))
                    {
                        StepRecipe.DispenseNo = Global.STDispensePopUp.SelectDispenseValue;
                        SetRecipeDispense();
                    }
                    break;
                case 2:
                    fGridValue = Global.KeyPad(StepRecipe.WaferCnt);
                    StepRecipe.WaferCnt = Convert.ToInt32(fGridValue);
                    break;
                case 3:
                    fGridValue = Global.KeyPad(StepRecipe.Interval);
                    StepRecipe.Interval = Convert.ToInt32(fGridValue);
                    break;
                case 4:
                    StepRecipe.Lotspec = StepRecipe.Lotspec.Equals(0) ? 1 : 0;
                    break;
                case 5:
                    StepRecipe.IsCond = StepRecipe.IsCond.Equals(false) ? true : false;
                    break;
                case 6:
                    StepRecipe.Timing++;
                    if (StepRecipe.Timing > 3) StepRecipe.Timing = 0;
                    break;
                case 7:
                    if (Global.RecipeOpen(enRecipeMenu.DEV_DUMMY_COND, StepRecipe.Recipe)) StepRecipe.Recipe = Global.STRecipePopUp.SelectRecipeName;
                    break;
                case 8:
                    StepRecipe.IsRecipeUse = StepRecipe.IsRecipeUse.Equals(true) ? false : true;
                    break;
                case 9:
                    fGridValue = Global.KeyPad(StepRecipe.RecipeCnt);
                    StepRecipe.RecipeCnt = Convert.ToInt32(fGridValue);
                    break;
                case 10:
                    fGridValue = Global.KeyPad(StepRecipe.RecipeInterval);
                    StepRecipe.RecipeInterval = Convert.ToInt32(fGridValue);
                    break;
                case 11:
                    fGridValue = Global.KeyPad(StepRecipe.RecipeTime);
                    StepRecipe.RecipeTime = Convert.ToInt32(fGridValue);
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Cond\DEV\", ref Global.DevDummyCondRecipeFileList);
            if (Global.DevDummyCondRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.DevDummyCondRecipeFileList[0];
                LoadListCommand();
            }
        }

        private void SetRecipeDispense()
        {
            Model.ModuleBaseCls module = Global.STModuleList.Find(x => x.MachineName.IndexOf("DEV") != -1);

            if (module != null)
            {
                int dispIndex = -1;
                dispIndex = Global.GetDispenseIndex(StepRecipe.DispenseNo);

                if (dispIndex != -1)
                {
                    for (int i = 0; i < module.DispenseList.Count; i++)
                    {
                        DispenseInfoCls dispense = module.DispenseList[i];

                        if (dispIndex == dispense.DispNo) { StepRecipe.DipsenseDisplay = dispense.DispName; break; }
                    }
                }
            }
        }
    }
}
