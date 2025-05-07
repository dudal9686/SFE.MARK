using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class WaferFlowRecipeViewModel : ViewModelBase
    {
        private ProcessWaferDataCls Waferdata_ = new ProcessWaferDataCls();
        public WaferStepCls WaferStep { get; set; } = new WaferStepCls();
        public DirFileListCls RecipeFileInfo { get; set; } = null;
        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SystemRecipeRelayCommand { get; set; }
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
        
        public WaferFlowRecipeViewModel()
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
            SystemRecipeRelayCommand = new RelayCommand(SystemRecipeCommand);
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

        public ProcessWaferDataCls Waferdata
        {
            get { return Waferdata_; }
            set { Waferdata_ = value; }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            if (!Global.GetRecipeEditMode()) return;

            string newFileName = string.Empty;

            if(Global.KeyBoard(ref newFileName))
            {
                if(Global.MessageOpen(enMessageType.OKCANCEL, "[Process_Wafer] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\WaferFlow\" + newFileName + ".csv");

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
            Waferdata.Clear();
            Waferdata.WaferStepList.Clear();
            if(RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadPrcessWaferRecipe(RecipeFileInfo.FileFullName, ref Waferdata_);
                if(Waferdata.WaferStepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SystemRecipeCommand()
        {
            if (!Global.GetRecipeEditMode()) return;
            string systemRecipe = string.Empty;
            if(Global.RecipeOpen(enRecipeMenu.SYSTEM, Waferdata.SystemRecipeName))
            {
                Waferdata.SystemRecipeName = Global.STRecipePopUp.SelectRecipeName;
            }
        }

        private void SaveAsListCommand()
        {
            if (!Global.GetRecipeEditMode()) return;
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Process_Wafer] Do you Make This file?"))
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
            if (!Global.GetRecipeEditMode()) return;
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Process_Wafer] Do you want to delete the file?"))
                { 
                    File.Delete(RecipeFileInfo.FileFullName);
                    GetRecipe();
                }
            }
        }

        private void ReNameListCommand()
        {
            if (!Global.GetRecipeEditMode()) return;
            if (RecipeListSelectedIndex != -1)
            {
                if(Global.MessageOpen(enMessageType.OKCANCEL, "[Process_Wafer] Change Process Name?"))
                {
                    string reNamefile = RecipeFileInfo.FileName;
                    if (Global.KeyBoard(ref reNamefile))
                    {
                        if(File.Exists(RecipeFileInfo.FilePath + reNamefile + ".csv"))
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
            if (!Global.GetRecipeEditMode()) return;
            WaferStepCls waferstep = new WaferStepCls();
            if(RecipeDetailSelectedIndex < 0) Waferdata.WaferStepList.Add(waferstep);
            else Waferdata.WaferStepList.Insert(RecipeDetailSelectedIndex+1, waferstep);

            for (int i = 0; i < Waferdata.WaferStepList.Count; i++)
            {
                WaferStepCls wafer = Waferdata.WaferStepList[i];
                wafer.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (!Global.GetRecipeEditMode()) return;
            if (RecipeFileInfo == null) return;
            JobDataCheckCls jobCheck = new JobDataCheckCls();
            if (!jobCheck.WaferFlowCheckCls(Waferdata)) return;

            if(Global.STDataAccess.SaveProcessWaferRecipe(RecipeFileInfo.FileFullName, Waferdata))
            {
                Global.MessageOpen(enMessageType.OK, "It has been saved.");
            }
        }

        private void DeleteDetailCommand()
        {
            if (!Global.GetRecipeEditMode()) return;
            if (WaferStep != null)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to delete it?"))
                {
                    Waferdata.WaferStepList.Remove(WaferStep);

                    for (int i = 0; i < Waferdata.WaferStepList.Count; i++)
                    {
                        WaferStepCls wafer = Waferdata.WaferStepList[i];
                        wafer.Index = i + 1;
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
            if (!Global.GetRecipeEditMode()) return;
            System.Windows.Controls.DataGrid grid = o as System.Windows.Controls.DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;
            string machineDesc = string.Empty;

            switch(index)
            {
                case 0:
                    break;
                case 1:
                    machineDesc = Waferdata.WaferStepList[RecipeDetailSelectedIndex].Name;
                    if (Global.GetModuleOpen("ALL", WaferStep))
                    {
                        if (machineDesc != Waferdata.WaferStepList[RecipeDetailSelectedIndex].Name)
                        {
                            WaferStep.RecipeName = string.Empty;
                            Global.GetModuleOpen(Waferdata.WaferStepList[RecipeDetailSelectedIndex].Name, WaferStep);
                        }
                    }

                    break;
                case 2:
                    Global.GetModuleOpen(Waferdata.WaferStepList[RecipeDetailSelectedIndex].Name, WaferStep);
                    break;
                case 3:
                    if (WaferStep == null) break;

                    enRecipeMenu recipeMenu = enRecipeMenu.CLEAN_COND;
                    string moduleName = Global.GetModuleFullNameToName(WaferStep.Name);

                    if (moduleName.IndexOf("COT") != -1) recipeMenu = enRecipeMenu.COT_PROCESS;
                    else if (moduleName.IndexOf("DEV") != -1) recipeMenu = enRecipeMenu.DEV_PROCESS;
                    else if (moduleName.IndexOf("ADH") != -1) recipeMenu = enRecipeMenu.ADH_PROCESS;
                    else if (moduleName.IndexOf("HHP") != -1) recipeMenu = enRecipeMenu.HHP_PROCESS;
                    else if (moduleName.IndexOf("LHP") != -1) recipeMenu = enRecipeMenu.LHP_PROCESS;
                    else if (moduleName.IndexOf("CPL") != -1) recipeMenu = enRecipeMenu.CPL_PROCESS;
                    else break;

                    if(Global.RecipeOpen(recipeMenu, WaferStep.RecipeName))WaferStep.RecipeName = Global.STRecipePopUp.SelectRecipeName;
                    break;
                case 4:
                    //if (Global.MessageOpen(enMessageType.OKCANCEL, "Extra Pin Change?")) WaferStep.IsExtraPin = WaferStep.IsExtraPin.Equals(false) ? true : false;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\WaferFlow\", ref Global.WaferFlowRecipeFileList);
            if (Global.WaferFlowRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.WaferFlowRecipeFileList[0];
                LoadListCommand();
            }
            else RecipeListSelectedIndex = -1;
        }
    }
}
