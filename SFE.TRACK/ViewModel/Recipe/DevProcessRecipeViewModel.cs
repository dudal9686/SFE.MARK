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
    public class DevProcessRecipeViewModel : ViewModelBase
    {
        private ProcessDevDataCls DevData_ = new ProcessDevDataCls();
        public SpinChamberStepCls DevStepData { get; set; } = null;
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
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;

        private string sGridValue = string.Empty;
        private float fGridValue = 0;

        public DevProcessRecipeViewModel()
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

        public ProcessDevDataCls DevData
        {
            get { return DevData_; }
            set { DevData_ = value; RaisePropertyChanged("DevData"); }
        }
        #endregion

        #region Command
        private void AddListCommand()
        {
            string newFileName = string.Empty;

            if (Global.KeyBoard(ref newFileName))
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Developer] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"D:\SFE_RECIPE\ProcessDEVRecipe\" + newFileName + ".csv");

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
            DevData.Clear();
            DevData.StepList.Clear();
            if (RecipeFileInfo != null)
            {
                Global.STDataAccess.ReadProcessDevRecipe(RecipeFileInfo.FileFullName, ref DevData_);
                if (DevData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Developer] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Developer] Change Process Name?"))
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
            SpinChamberStepCls stepData = new SpinChamberStepCls();
            if (RecipeDetailSelectedIndex < 0) DevData.StepList.Add(stepData);
            else DevData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < DevData.StepList.Count; i++)
            {
                SpinChamberStepCls step = DevData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;
            Global.STDataAccess.SaveProcessDevRecipe(RecipeFileInfo.FileFullName, DevData);
        }

        private void DeleteDetailCommand()
        {
            if (DevStepData != null)
            {
                DevData.StepList.Remove(DevStepData);

                for (int i = 0; i < DevData.StepList.Count; i++)
                {
                    SpinChamberStepCls step = DevData.StepList[i];
                    step.Index = i + 1;
                }
            }
        }

        private void StopRangeCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(DevData.StopRange);
                DevData.StopRange = Convert.ToInt32(value);
            }
        }

        private void AlarmRangeCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                float value = Global.KeyPad(DevData.AlarmRange);
                DevData.AlarmRange = Convert.ToInt32(value);
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
                    sGridValue = DevStepData.Name;
                    if (Global.KeyBoard(ref sGridValue)) DevStepData.Name = sGridValue;
                    break;
                case 2:
                    fGridValue = Global.KeyPad(DevStepData.StepTime);
                    DevStepData.StepTime = fGridValue;
                    break;
                case 3:
                    fGridValue = Global.KeyPad(DevStepData.SpinSpeed);
                    DevStepData.SpinSpeed = Convert.ToInt32(fGridValue);
                    break;
                case 4:
                    fGridValue = Global.KeyPad(DevStepData.SpinAcc);
                    DevStepData.SpinAcc = Convert.ToInt32(fGridValue);
                    break;
                case 5:
                    if (Global.DispenseInfoOpen(enDispenseModule.DEV, DevStepData.DispNo)) DevStepData.DispNo = Global.STDispensePopUp.SelectDispenseValue;
                    break;
                case 6:
                    if (Global.ArmPositionOpen(enArmTpe.ARM1, DevStepData.Arm1Pos)) DevStepData.Arm1Pos = Global.STArmPositionPopUp.SelectArmPosition;
                    break;
                case 7:
                    fGridValue = Global.KeyPad(DevStepData.Arm1Speed);
                    DevStepData.Arm1Speed = Convert.ToInt32(fGridValue);
                    break;
                case 8:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm1 Wait Change?")) DevStepData.IsArm1MoveWait = DevStepData.IsArm1MoveWait.Equals(true) ? false : true;
                    break;
                case 9:
                    if (Global.ArmPositionOpen(enArmTpe.ARM2, DevStepData.Arm2Pos)) DevStepData.Arm2Pos = Global.STArmPositionPopUp.SelectArmPosition;
                    break;
                case 10:
                    fGridValue = Global.KeyPad(DevStepData.Arm2Speed);
                    DevStepData.Arm2Speed = Convert.ToInt32(fGridValue);
                    break;
                case 11:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm2 Wait Change?")) DevStepData.IsArm2MoveWait = DevStepData.IsArm2MoveWait.Equals(true) ? false : true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"D:\SFE_RECIPE\ProcessDEVRecipe\", ref Global.DevProcessRecipeFileList);
            if (Global.DevProcessRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.DevProcessRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
