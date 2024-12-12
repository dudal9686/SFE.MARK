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
    public class DevDummySeqRecipeViewModel : ViewModelBase
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
        public RelayCommand<object> RecipeDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;

        private float fGridValue = 0;

        public DevDummySeqRecipeViewModel()
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Seq] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\Dummy\Seq\DEV\" + newFileName + ".csv");

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
                Global.STDataAccess.ReadDummySeqDEVRecipe(RecipeFileInfo.FileFullName, ref DevData_);
                if(DevData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Seq] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Seq] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Dev Dummy Seq] Change Process Name?"))
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
            if(Global.STDataAccess.SaveDummySeqDEVRecipe(RecipeFileInfo.FileFullName, DevData)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
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
                        SpinChamberStepCls step = DevData.StepList[i];
                        step.Index = i + 1;
                    }
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
                    string name = DevStepData.Name;
                    if (Global.KeyBoard(ref name)) DevStepData.Name = name;
                    break;
                case 2:
                    string loopTarget = string.Empty;
                    int loop = 0;
                    if (DevStepData.Loop == 0) loopTarget = "NO";
                    else if (DevStepData.Loop == 2) loopTarget = "END";
                    else
                    {
                        loop = DevStepData.Loop - 0x10000;
                        loopTarget = "START";
                    }

                    if (Global.SystemControlTargetOpen("DUMMYLOOP", loopTarget))
                    {
                        if (Global.STModulePopUp.ControlTarget == "NO") DevStepData.Loop = 0;
                        if (Global.STModulePopUp.ControlTarget == "START")
                        {
                            DevStepData.Loop = 0x10000 + loop;

                            fGridValue = Global.KeyPad(loop);
                            DevStepData.Loop = 0x10000 + Convert.ToInt32(fGridValue);
                        }
                        if (Global.STModulePopUp.ControlTarget == "END") DevStepData.Loop = 2;
                    }
                    break;
                case 3:
                    fGridValue = Global.KeyPad(DevStepData.StepTime);
                    DevStepData.StepTime = fGridValue;
                    break;
                case 4:
                    if (Global.DispenseInfoOpen(enDispenseModule.DEV, DevStepData.DispNo, "DUMMYUSE"))
                    {
                        DevStepData.DispNo = Global.STDispensePopUp.SelectDispenseValue;
                    }
                    break;
                case 5:
                    if (Global.ArmPositionOpen(enArmTpe.ARM1, DevStepData.Arm1Pos)) DevStepData.Arm1Pos = Global.STArmPositionPopUp.SelectArmPosition;
                    break;
                case 6:
                    fGridValue = Global.KeyPad(DevStepData.Arm1Speed);
                    DevStepData.Arm1Speed = Convert.ToInt32(fGridValue);
                    break;
                case 7:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm1 Wait Change?")) DevStepData.IsArm1MoveWait = DevStepData.IsArm1MoveWait.Equals(true) ? false : true;
                    break;
                case 8:
                    if (Global.ArmPositionOpen(enArmTpe.ARM2, DevStepData.Arm2Pos)) DevStepData.Arm2Pos = Global.STArmPositionPopUp.SelectArmPosition;
                    break;
                case 9:
                    fGridValue = Global.KeyPad(DevStepData.Arm2Speed);
                    DevStepData.Arm2Speed = Convert.ToInt32(fGridValue);
                    break;
                case 10:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm2 Wait Change?")) DevStepData.IsArm2MoveWait = DevStepData.IsArm2MoveWait.Equals(true) ? false : true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Dummy\Seq\DEV\", ref Global.DevDummySeqRecipeFileList);
            if (Global.DevDummySeqRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.DevDummySeqRecipeFileList[0];
                LoadListCommand();
            }
        }
    }
}
