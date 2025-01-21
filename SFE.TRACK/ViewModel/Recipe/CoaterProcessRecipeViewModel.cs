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
    public class CoaterProcessRecipeViewModel : ViewModelBase
    {
        private ProcessCotDataCls CotData_ = new ProcessCotDataCls();
        public SpinChamberStepCls CotStepData { get; set; } = null;
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
        public RelayCommand<object> RecipeDoubleClickRelayCommand { get; set; }
        public RelayCommand<object> RecipeDetailDoubleClickRelayCommand { get; set; }

        private int RecipeListSelectedIndex_ = -1;
        private int RecipeDetailSelectedIndex_ = -1;
        private string sGridValue = string.Empty;
        private float fGridValue = 0;
        public CoaterProcessRecipeViewModel()
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

        public ProcessCotDataCls CotData
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater] Would you like to create a file ?"))
                {
                    FileInfo fi = new FileInfo(@"C:\MachineSet\SFETrack\Recipe\Process\COT\" + newFileName + ".csv");

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
                Global.STDataAccess.ReadProcessCotRecipe(RecipeFileInfo.FileFullName, ref CotData_);
                if (CotData.StepList.Count == 0) RecipeDetailSelectedIndex = -1;
                else RecipeDetailSelectedIndex = 0;
            }
        }       

        private void SaveAsListCommand()
        {
            if (RecipeListSelectedIndex != -1)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater] Do you Make This file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater] Do you want to delete the file?"))
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
                if (Global.MessageOpen(enMessageType.OKCANCEL, "[Coater] Change Process Name?"))
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
            stepData.Arm1Pos = "HOME";
            stepData.Arm2Pos = "HOME";
            stepData.Arm1Speed = 100;
            stepData.Arm2Speed = 100;
            if (RecipeDetailSelectedIndex < 0) CotData.StepList.Add(stepData);
            else CotData.StepList.Insert(RecipeDetailSelectedIndex + 1, stepData);

            for (int i = 0; i < CotData.StepList.Count; i++)
            {
                SpinChamberStepCls step = CotData.StepList[i];
                step.Index = i + 1;
            }
        }

        private void SaveDetailCommand()
        {
            if (RecipeFileInfo == null) return;

            JobDataCheckCls jobCheck = new JobDataCheckCls();
            if (!jobCheck.COTProcessCheckCls(CotData)) return;

            if (Global.STDataAccess.SaveProcessCotRecipe(RecipeFileInfo.FileFullName, CotData)) Global.MessageOpen(enMessageType.OK, "It has been saved.");
        }

        private void DeleteDetailCommand()
        {
            if (CotStepData != null)
            {
                if (Global.MessageOpen(enMessageType.OKCANCEL, "Are you sure you want to delete it?"))
                {
                    CotData.StepList.Remove(CotStepData);
                    for (int i = 0; i < CotData.StepList.Count; i++)
                    {
                        SpinChamberStepCls step = CotData.StepList[i];
                        step.Index = i + 1;
                    }
                }
            }
        }

        private void StopRangeCommand()


        {
            if(RecipeListSelectedIndex != -1)
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
                    sGridValue = CotStepData.Name;
                    if(Global.KeyBoard(ref sGridValue)) CotStepData.Name = sGridValue;
                    break;
                case 2:
                    fGridValue = Global.KeyPad(CotStepData.StepTime);
                    CotStepData.StepTime = fGridValue;
                    break;
                case 3:
                    fGridValue = Global.KeyPad(CotStepData.SpinSpeed);
                    CotStepData.SpinSpeed = Convert.ToInt32(fGridValue);
                    break;
                case 4:
                    fGridValue = Global.KeyPad(CotStepData.SpinAcc);
                    CotStepData.SpinAcc = Convert.ToInt32(fGridValue);
                    break;
                case 5:
                    if(Global.DispenseInfoOpen(enDispenseModule.COT, CotStepData.DispNo)) CotStepData.DispNo = Global.STDispensePopUp.SelectDispenseValue;
                    break;
                case 6:
                    if(Global.ArmPositionOpen(enArmTpe.ARM1, CotStepData.Arm1Pos)) CotStepData.Arm1Pos = Global.STArmPositionPopUp.SelectArmPosition;                    
                    break;
                case 7:
                    fGridValue = Global.KeyPad(CotStepData.Arm1Speed);
                    CotStepData.Arm1Speed = Convert.ToInt32(fGridValue);
                    break;
                case 8:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm1 Wait Change?")) CotStepData.IsArm1MoveWait = CotStepData.IsArm1MoveWait.Equals(true) ? false : true;
                    break;
                case 9:
                    if (Global.ArmPositionOpen(enArmTpe.ARM2, CotStepData.Arm2Pos)) CotStepData.Arm2Pos = Global.STArmPositionPopUp.SelectArmPosition;
                    break;
                case 10:
                    fGridValue = Global.KeyPad(CotStepData.Arm2Speed);
                    CotStepData.Arm2Speed = Convert.ToInt32(fGridValue);
                    break;
                case 11:
                    if (Global.MessageOpen(enMessageType.OKCANCEL, "Arm2 Wait Change?")) CotStepData.IsArm2MoveWait = CotStepData.IsArm2MoveWait.Equals(true) ? false : true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        private void GetRecipe()
        {
            Global.GetDirectoryFile(@"C:\MachineSet\SFETrack\Recipe\Process\COT\", ref Global.CoaterProcessRecipeFileList);
            if (Global.CoaterProcessRecipeFileList.Count > 0)
            {
                RecipeListSelectedIndex = 0;
                RecipeFileInfo = Global.CoaterProcessRecipeFileList[0];
                LoadListCommand();
            }
            else RecipeListSelectedIndex = -1;
        }
    }
}
