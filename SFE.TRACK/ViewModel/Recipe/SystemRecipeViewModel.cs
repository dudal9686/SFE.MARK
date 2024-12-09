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
    public class SystemRecipeViewModel : ViewModelBase
    {
        private SystemRecipeCls SystemRecipeData_ = new SystemRecipeCls();
        public SystemRecipeStepCls SystemRecipeStep { get; set; } = new SystemRecipeStepCls();
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

        public SystemRecipeViewModel()
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

        public SystemRecipeCls SystemRecipeData
        {
            get { return SystemRecipeData_; }
            set { SystemRecipeData_ = value; RaisePropertyChanged("SystemRecipeData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[System Recipe] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"D:\SFE_RECIPE\SystemRecipe\" + newFileName + ".csv");

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
            SystemRecipeData.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadSystemRecipe(RecipeFileInfo.FileFullName, ref SystemRecipeData_);
                if(SystemRecipeData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[System Recipe] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[System Recipe] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[System Recipe] Change Process Name?"))
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
            SystemRecipeStepCls stepData = new SystemRecipeStepCls();
            if (RecipeDetailSelectedIndex < 0) SystemRecipeData.StepList.Add(stepData);
            else SystemRecipeData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < SystemRecipeData.StepList.Count; i++)
            {
                SystemRecipeStepCls step = SystemRecipeData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SaveSystemRecipe(RecipeFileInfo.FileFullName, SystemRecipeData);
        }

        private void DeleteDetailCommand()
        {
            if (SystemRecipeStep != null)
            {
                SystemRecipeData.StepList.Remove(SystemRecipeStep);

                for (int i = 0; i < SystemRecipeData.StepList.Count; i++)
                {
                    SystemRecipeStepCls step = SystemRecipeData.StepList[i];
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
                    //기존은 유닛 넘버로 가져 오지만 이제는 모듈 넘버로 가져온다.
                    if(Global.GetModuleOpen("SYSTEM", SystemRecipeStep.ModuleNo))
                    {
                        SystemRecipeStep.BlockNo = Global.STModulePopUp.BlockNo;
                        SystemRecipeStep.ModuleNo = Global.STModulePopUp.ModuleNo;
                    }
                    break;
                case 2:

                    if(SystemRecipeStep.ModuleNo == 0)
                    {
                        Global.MessageOpen(enMessageType.OK, "Please select a module.");
                        return;
                    }

                    if(Global.SystemControlTargetOpen("SYSTEMCONTROL", SystemRecipeStep.ControlTarget, SystemRecipeStep.ModuleNo))
                    {
                        SystemRecipeStep.ControlTarget = Global.STModulePopUp.ControlTarget;
                    }
                    break;
                case 3:
                    fGridValue = Global.KeyPad(SystemRecipeStep.SetValue);
                    SystemRecipeStep.SetValue = fGridValue;
                    break;
                case 4:
                    fGridValue = Global.KeyPad(SystemRecipeStep.AlarmMaxValue);
                    SystemRecipeStep.AlarmMaxValue = fGridValue;
                    break;
                case 5:
                    fGridValue = Global.KeyPad(SystemRecipeStep.AlarmMinValue);
                    SystemRecipeStep.AlarmMinValue = fGridValue;
                    break;
                case 6:
                    fGridValue = Global.KeyPad(SystemRecipeStep.StopMaxValue);
                    SystemRecipeStep.StopMaxValue = fGridValue;
                    break;
                case 7:
                    fGridValue = Global.KeyPad(SystemRecipeStep.StopMinValue);
                    SystemRecipeStep.StopMinValue = fGridValue;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"D:\SFE_RECIPE\SystemRecipe\", ref Global.SystemRecipeFileList);
            if (Global.SystemRecipeFileList.Count() > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.SystemRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
