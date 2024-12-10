using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.IO;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class PumpRecipeViewModel : ViewModelBase
    {
        public ObservableCollection<ProcessPumpDataCls> PumpList { get; set; } = new ObservableCollection<ProcessPumpDataCls>();
        public ProcessPumpDataCls PumpRecipeData_ = new ProcessPumpDataCls();
        public DirFileListCls RecipeFileInfo { get; set; } = null;
        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;
        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }

        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }

        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private float fGridValue = 0;
        public PumpRecipeViewModel()
        {
            GetRecipe();
            AddListRelayCommand = new RelayCommand(AddListCommand);
            LoadListRelayCommand = new RelayCommand(LoadListCommand);
            SaveAsListRelayCommand = new RelayCommand(SaveAsListCommand);
            DeleteListRelayCommand = new RelayCommand(DeleteListCommand);
            ReNameListRelayCommand = new RelayCommand(ReNameListCommand);
            SaveDetailRelayCommand = new RelayCommand(SaveDetailCommand);
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
        public ProcessPumpDataCls PumpRecipeData
        {
            get {
                if (PumpRecipeData_ == null) PumpRecipeData_ = new ProcessPumpDataCls();
                return PumpRecipeData_; 
            }
            set { PumpRecipeData_ = value;}
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Pump Recipe] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\PumpRecipe\" + newFileName + ".csv");

                    if (!fi.Exists)
                    {
                        fi.Create();
                        GetRecipe();
                        RecipeDetailSelectedIndex = -1;

                        for(int i = 0; i < Global.PumpRecipeFileList.Count; i++)
                        {
                            DirFileListCls file = Global.PumpRecipeFileList[i] as DirFileListCls;
                            if(file.FileName == newFileName)
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
            PumpList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadPumpRecipe(RecipeFileInfo.FileFullName, ref PumpRecipeData_);
                PumpList.Add(PumpRecipeData);
                RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Pump Recipe] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Pump Recipe] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Pump Recipe] Change Process Name?"))
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

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SavePumpRecipe(RecipeFileInfo.FileFullName, PumpRecipeData);
        }

        private void RecipeDetailDoubleClickCommand(object o)
        {
            DataGrid grid = o as DataGrid;
            int index = grid.CurrentCell.Column.DisplayIndex;

            switch (index)
            {
                case 0:
                    fGridValue = Global.KeyPad(PumpRecipeData.DisAmount);
                    PumpRecipeData.DisAmount = fGridValue;
                    break;
                case 1:
                    fGridValue = Global.KeyPad(PumpRecipeData.DistRate);
                    PumpRecipeData.DistRate = fGridValue;
                    break;
                case 2:
                    fGridValue = Global.KeyPad(PumpRecipeData.Acc);
                    PumpRecipeData.Acc = Convert.ToInt32(fGridValue);
                    break;
                case 3:
                    fGridValue = Global.KeyPad(PumpRecipeData.Dec);
                    PumpRecipeData.Dec = Convert.ToInt32(fGridValue);
                    break;
                case 4:
                    fGridValue = Global.KeyPad(PumpRecipeData.ReloadRate);
                    PumpRecipeData.ReloadRate = fGridValue;
                    break;
                case 5:
                    fGridValue = Global.KeyPad(PumpRecipeData.Cal);
                    PumpRecipeData.Cal = fGridValue;
                    break;
                case 6:
                    fGridValue = Global.KeyPad(PumpRecipeData.AvCloseDelayTime);
                    PumpRecipeData.AvCloseDelayTime = fGridValue;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\PumpRecipe\", ref Global.PumpRecipeFileList);
            if (Global.PumpRecipeFileList.Count() > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.PumpRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
