using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SFE.TRACK.ViewModel.Recipe
{
    public class HHPProcessRecipeViewModel : ViewModelBase
    {
        private ProcessChamberDataCls HhpData_ = new ProcessChamberDataCls();
        public ChamberStepCls ChamberStepData { get; set; } = null;
        public DirFileListCls RecipeFileInfo { get; set; } = null;

        public RelayCommand AddListRelayCommand { get; set; }
        public RelayCommand LoadListRelayCommand { get; set; }
        public RelayCommand SaveAsListRelayCommand { get; set; }
        public RelayCommand DeleteListRelayCommand { get; set; }
        public RelayCommand ReNameListRelayCommand { get; set; }

        public RelayCommand AddDetailRelayCommand { get; set; }
        public RelayCommand SaveDetailRelayCommand { get; set; }
        public RelayCommand DeleteDetailRelayCommand { get; set; }

        public RelayCommand SetValueRelayCommand { get; set; }
        public RelayCommand AlarmMaxRelayCommand { get; set; }
        public RelayCommand AlarmMinRelayCommand { get; set; }
        public RelayCommand StopMaxRelayCommand { get; set; }
        public RelayCommand StopMinRelayCommand { get; set; }
        public RelayCommand<object> RecipeDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;

        private string sGridValue = string.Empty;
        private float fGridValue = 0;

        public HHPProcessRecipeViewModel()
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

            SetValueRelayCommand = new RelayCommand(SetValueCommand);
            AlarmMaxRelayCommand = new RelayCommand(AlarmMaxCommand);
            AlarmMinRelayCommand = new RelayCommand(AlarmMinCommand);
            StopMaxRelayCommand = new RelayCommand(StopMaxCommand);
            StopMinRelayCommand = new RelayCommand(StopMinCommand);

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

        public ProcessChamberDataCls HhpData
        {
            get { return HhpData_; }
            set { HhpData_ = value; RaisePropertyChanged("HhpData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[HHP] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\Process\HHP\" + newFileName + ".csv");

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
            HhpData.Clear();
            HhpData.StepList.Clear();

            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadProcessHHPRecipe(RecipeFileInfo.FileFullName, ref HhpData_);
                if(HhpData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[HHP] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[HHP] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[HHP] Change Process Name?"))
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
            ChamberStepCls stepData = new ChamberStepCls();
            stepData.PinDesc = "Down";
            if (RecipeDetailSelectedIndex < 0) HhpData.StepList.Add(stepData);
            else HhpData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < HhpData.StepList.Count; i++)
            {
                ChamberStepCls step = HhpData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            JobDataCheckCls jobCheck = new JobDataCheckCls();
            if (!jobCheck.ChamberProcessCheckCls(HhpData)) return;
            if (Global.STDataAccess.SaveProcessHHPRecipe(RecipeFileInfo.FileFullName, HhpData)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
        }

        private void DeleteDetailCommand()
        {
            if (ChamberStepData != null)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to delete it?"))
                {
                    HhpData.StepList.Remove(ChamberStepData);
                    for (int i = 0; i < HhpData.StepList.Count; i++)
                    {
                        ChamberStepCls step = HhpData.StepList[i];
                        step.Index = i + 1;
                    }
                }
            }
        }

        private void SetValueCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(HhpData.SetValue);
                HhpData.SetValue = value;
            }
        }
        private void AlarmMaxCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(HhpData.AlarmMaxValue);
                HhpData.AlarmMaxValue = value;
            }
        }
        private void AlarmMinCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(HhpData.AlarmMinValue);
                HhpData.AlarmMinValue = value;
            }
        }
        private void StopMaxCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(HhpData.StopMaxValue);
                HhpData.StopMaxValue = value;
            }
        }
        private void StopMinCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(HhpData.StopMinValue);
                HhpData.StopMinValue = value;
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
                    sGridValue = ChamberStepData.Name;
                    if (Global.KeyBoard(ref sGridValue)) ChamberStepData.Name = sGridValue;
                    break;
                case 2:
                    fGridValue = Global.KeyPad(ChamberStepData.StepTime);
                    ChamberStepData.StepTime = fGridValue;
                    break;
                case 3:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Pin Position Change?")) ChamberStepData.IsPinPos = ChamberStepData.IsPinPos.Equals(true) ? false : true;
                    break;
                case 4:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Shutter Position Change?")) ChamberStepData.IsShutter = ChamberStepData.IsShutter.Equals(true) ? false : true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\HHP\", ref Global.HHPProcessRecipeFileList);
            if (Global.HHPProcessRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.HHPProcessRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
