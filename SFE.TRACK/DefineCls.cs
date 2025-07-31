using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CoreCSRunSim;

namespace SFE.TRACK
{
    public enum EnumCustomProcess { None, Chamber, Developer, Coater };
    public enum enMachineStatus
    {
        STOP,
        STOPING, //장비가 멈추는 행동을 하고 있다. 취소시.
        RUN,
        MANUAL,
        HOME,
    };
    public enum enJogMode
    {
        LOW,
        HIGH,
        PITCH,
    }
    public enum enAlarmMessageType
    {
        WARNING = 0,
        ALARM,
    }
    public enum enCategory
    {
        PRA,
        CRA,
        CHAMBER,

    }
    public enum enAxisType
    {
        CRA_X = 0,
        CRA_Y = 1,
        CRA_Z = 2,

        PRA_X1 = 3,
        PRA_X2 = 4,
        PRA_X3 = 5,
        PRA_Y = 6,
        PRA_Z = 7,
        PRA_T = 8,

        C1_X1 = 30,
        C1_X2 = 31,
        C1_BT = 32, //Bath
        C1_PP1 = 33, //Pump

        D1_X1 = 34,
        C1_PP2 = 35,

        D2_X1 = 36,
        C1_PP3 = 37,

        C1_SPIN = 38,
        D1_SPIN = 39,
        D2_SPIN = 40,

        CB01_Pin = 100,
        CB02_Pin = 101,
        CB03_Pin = 102,
        CB04_Pin = 103,
        CB05_Pin = 104,
        CB06_Pin = 105,
        CB07_Pin = 106,
        CB08_Pin = 107,
        CB09_Pin = 108,
        CB10_Pin = 109,
        CB11_Pin = 110,
        CB12_Pin = 111,

        CHAMBER = 200,
        NONE = 1000,
    }    

    public enum enAxisDirection
    {
        X,
        Y,
        Z,
        T,
    }

    public enum enMaintDate
    {
        DAYS,
        WEEKS,
        MONTHS,
        YEARS,
    }

    public enum enMaintUnit
    {
        TIMES,
        VOLUME,

    }
    public enum enAuthLevel
    {
        GUEST,
        NOMAL_OP,
        PROCESS_OP,
        EQUIPMENT_OP,
        SERVICE_OP,
    }

    public enum enMaintenanceMode
    {
        NONE,
        MAINTMODE,
        MAINTMODE_USE,
    }

    public enum enDirection
    {
        CW = 1,
        CCW = 0,
    };

    public enum enMachineUintNo
    {
        COATER_L = 1,
        COATER_R = 5,
        DEVELOP_L = 10,
        DEVELOP_R = 15,

        ADH_01 = 30,
        ADH_02 = 31,
        ADH_03 = 32,
        ADH_04 = 33,
        ADH_05 = 34,
        ADH_06 = 35,
        ADH_07 = 36,
        ADH_08 = 37,

        TRS_01 = 40,
        TRS_02 = 41,
        TRS_03 = 42,
        TRS_04 = 43,
        TRS_05 = 44,
        TRS_06 = 45,
        TRS_07 = 46,
        TRS_08 = 47,

        CPL_01 = 50,
        CPL_02 = 51,
        CPL_03 = 52,
        CPL_04 = 53,
        CPL_05 = 54,
        CPL_06 = 55,
        CPL_07 = 56,
        CPL_08 = 57,

        LHP_01 = 60,
        LHP_02 = 61,
        LHP_03 = 62,
        LHP_04 = 63,
        LHP_05 = 64,
        LHP_06 = 65,
        LHP_07 = 66,
        LHP_08 = 67,

        HHP_01 = 70,
        HHP_02 = 71,
        HHP_03 = 72,
        HHP_04 = 73,
        HHP_05 = 74,
        HHP_06 = 75,
        HHP_07 = 76,
        HHP_08 = 77,

        CWH_01 = 80,
        CWH_02 = 81,
        CWH_03 = 82,
        CWH_04 = 83,
        CWH_05 = 84,
        CWH_06 = 85,
        CWH_07 = 86,
        CWH_08 = 87,

        TCP_01 = 100,
        TCP_02 = 101,
        TCP_03 = 102,
        TCP_04 = 103,
        TCP_05 = 104,
        TCP_06 = 105,
        TCP_07 = 106,
        TCP_08 = 107,
    }

    public enum enDispenseModule
    {
        ADH,
        COT,
        DEV,
    }

    public enum enMessageType
    {
        OKCANCEL,
        OK,
        NONE,
    }

    public enum enArmTpe
    {
        ARM1,
        ARM2,
    }

    public enum enRecipeMenu
    {
        ADH_DUMMY_COND,
        ADH_DUMMY_SEQ,
        ADH_PROCESS,
        CLEAN_COND,
        COT_CLEAN,
        COT_DUMMY_COND,
        COT_DUMMY_SEQ,
        COT_PROCESS,
        CPL_PROCESS,
        DEV_CLEAN,
        DEV_DUMMY_COND,
        DEV_DUMMY_SEQ,
        DEV_PROCESS,        
        DUMMY_COND_LINK,
        HHP_PROCESS,
        LHP_PROCESS,
        PUMP,
        SYSTEM,
        TCP_PROCESS,
        WAFER_FLOW,
        JOBINFO,
    };

    enum enMainMenu
    {
        AUTO = 0,
        RECIPE,
        MOTOR,
        PARAMETER,
        MOTIONMOVING,
        MAINT,
        UTILITY,
        GEM,
        LOG,
        ALARM,
    };

    public enum enDispenseNo : uint
    {
        DISPENSE_01 = 0x0001, DISPENSE_02 = 0x0002, DISPENSE_03 = 0x0004, DISPENSE_04 = 0x0008, DISPENSE_05 = 0x0010,
        DISPENSE_06 = 0x0020, DISPENSE_07 = 0x0040, DISPENSE_08 = 0x0080, DISPENSE_09 = 0x0100, DISPENSE_10 = 0x0200,
        DISPENSE_11 = 0x0400, DISPENSE_12 = 0x0800, DISPENSE_13 = 0x1000, DISPENSE_14 = 0x2000, DISPENSE_15 = 0x4000,
        DISPENSE_16 = 0x8000, DISPENSE_17 = 0x10000, DISPENSE_18 = 0x20000, DISPENSE_19 = 0x40000, DISPENSE_20 = 0x80000,
        DISPENSE_21 = 0x100000, DISPENSE_22 = 0x200000, DISPENSE_23 = 0x400000, DISPENSE_24 = 0x800000, DISPENSE_25 = 0x1000000,
        DISPENSE_26 = 0x2000000, DISPENSE_27 = 0x4000000, DISPENSE_28 = 0x8000000, DISPENSE_29 = 0x10000000, DISPENSE_30 = 0x20000000,
        DISPENSE_31 = 0x40000000, DISPENSE_32 = 0x80000000,
    };

    public enum enCotDispense : uint
    {
        RESIST1 = 0x0001, RESIST2 = 0x0002, RESIST3 = 0x0004, RESIST4 = 0x0008, RESIST5 = 0x0010,
        RESIST6 = 0x0020, RESIST7 = 0x0040, RESIST8 = 0x0080, RRC_SOLVENT = 0x0100, RRC_SOLVENT2 = 0x0200,
        /*DISPENSE_11		=0x0400		,DISPENSE_12			=0x0800	,*/
        EBR = 0x1000, EBR2 = 0x2000, R1_FILTER_AIR_VENT = 0x4000,
        R2_FILTER_AIR_VENT = 0x8000, R3_FILTER_AIR_VENT = 0x10000, R4_FILTER_AIR_VENT = 0x20000, TANK_PRESS = 0x40000,/*DISPENSE_20		=0x80000	,*/
        BACK_RINSE1 = 0x100000, BACK_RINSE2 = 0x200000, SOLVENT_BATH = 0x400000, LINE1_RINSE = 0x800000, AUTO_DAMPER = 0x1000000,
        BACK_RINSE1P2 = 0x2000000, DRAIN_CASE_CLEAN = 0x4000000 /*,DISPENSE_28		=0x8000000*/, FILTER1_AIR_VENT = 0x10000000, FILTER2_AIR_VENT = 0x20000000,
        FILTER3_AIR_VENT = 0x40000000, FILTER4_AIR_VENT = 0x80000000,
    }

    public enum enDevDispense
    {
        RINSE = 0x0001, BACK_RINSE = 0x0002, DEVELOP = 0x0004, AUTO_DAMPER = 0x1000000,
    }

    public enum enModuleType
    {
        FOUP,
        WAFER,
        CRA,
        PRA,
        CHAMBER,
        SPINCHAMBER,
        NONE,
    };

    public enum enModuleState
    {
        NOTINITIAL,
        STANDBY,
        HOMMING,//OPERATING
        PROCESS,
        IDLE,
        PAUSE,
        MAINTENANCE,
        PROBLEM,
        NONE,
    }

    public enum enWaferState
    {
        WAFER_NONE = 0,
        WAFER_NORMAL,
        WAFER_ERROR,
        WAFER_EXTRA,
        WAFER_ABORT,
        WAFER_PILOT,
        WAFER_CUPWASH,
        WAFER_PROCESS_END,
        WAFER_PROCESS_NORMAL,
        WAFER_PROCESS,

        //Foup을 위한 색
        WAFER_EXIST,
        WAFER_EMPTY,
        WAFER_EXIST_PROCESS_END,
        WAFER_NO_USE,
    }

    public enum enHomeState 
    { 
        HOME_NONE, 
        HOMMING,
        HOME_OK, 
        HOME_ERROR 
    };

    public enum enMoveMode
    {
        JOG,
        RELATIVE,
        ABSOLUTE,
        VELOCITY
    }

    public enum enMoveWait
    {
        NOWAIT,
        WAIT,
    }

    public enum enIOType
    {
        DI,
        DO,
        AI,
        AO,
    }

    public enum enWorkingNeedStep 
    { 
        IsNot, 
        IsNeed, 
        IsDoing, 
        IsDone,
        IsDoneFail,
    };

    public enum enLamp
    {
        OFF,
        ON,
        TOGGLE,
    };
    public enum enBuzzer
    {
        OFF,
        ON,
    };

    public enum enLampDesc
    {
        NormalStop = 1,
        NormalStopping,
        ErrorStop,
        ErrorStopping,
        NomalRunning,
        Initializing,
        ManualRunning,
    };

    public class DefineCls
    {
    }

    public class ProcessWaferDataCls : ViewModelBase
    {
        string systemRecipe = string.Empty;
        private ObservableCollection<WaferStepCls> WaferStepList_ = new ObservableCollection<WaferStepCls>();

        public ObservableCollection<WaferStepCls> WaferStepList
        {
            get { return WaferStepList_; }
            set { WaferStepList_ = value; }
        }

        public void Clear()
        {
            SystemRecipeName = string.Empty;
        }

        public string SystemRecipeName 
        {
            get { return systemRecipe; }
            set { systemRecipe = value; RaisePropertyChanged("SystemRecipeName"); }
        }
    }

    public class RecipeInfoCls : ViewModelBase
    {
        int type = 0;
        string name = string.Empty;
        string dummyName = string.Empty;

        public int Type
        {
            get { return type; }
            set { type = value; RaisePropertyChanged("Type"); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }

        public string DummyName
        {
            get { return dummyName; }
            set { dummyName = value; RaisePropertyChanged("DummyName"); }
        }
    }

    public class WaferStepCls : ViewModelBase
    {
        string sName = string.Empty;
        int nBlokNo = 2;
        string sModuleNo = string.Empty;
        int nModuleFunc = 0;
        bool isExtraPin = false;
        string sRecipeName = "-";
        int[] nModuleNoList = new int[20];
        int nModuleCount = 0;
        bool isProcess = false;
        int nProcessModule = 0;
        int nIndex = 0;
        string sType = string.Empty;
        string moduleListDescription = string.Empty;

        public WaferStepCls()
        {
            for (int i = 0; i < 20; i++) nModuleNoList[i] = 0;
        }

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; RaisePropertyChanged("Name"); }
        }

        public int BlokNo
        {
            get { return nBlokNo; }
            set { nBlokNo = value; RaisePropertyChanged("BlokNo"); }
        }

        public string ModuleNo
        {
            get { return sModuleNo; }
            set { sModuleNo = value; RaisePropertyChanged("ModuleNo"); }
        }

        public string Type
        {
            get { return sType; }
            set { sType = value; }
        }

        public int ModuleFunc
        {
            get { return nModuleFunc; }
            set { nModuleFunc = value; RaisePropertyChanged("ModuleFunc"); }
        }

        public bool IsExtraPin
        {
            get { return isExtraPin; }
            set {
                if (!Global.GetRecipeEditMode()) return;
                isExtraPin = value; 
                RaisePropertyChanged("IsExtraPin"); 
            }
        }

        public string RecipeName
        {
            get { return sRecipeName; }
            set { sRecipeName = value; RaisePropertyChanged("RecipeName"); }
        }

        public int[] ModuleNoList
        {
            get { return nModuleNoList; }
            set { nModuleNoList = value; RaisePropertyChanged("ModuleNoList"); }
        }

        public string ModuleListDescription
        {
            get { return moduleListDescription; }
            set { moduleListDescription = value; RaisePropertyChanged("ModuleListDescription"); }
        }

        public int ModuleCount
        {
            get { return nModuleCount; }
            set { nModuleCount = value; RaisePropertyChanged("ModuleCount"); }
        }

        public bool IsProcess
        {
            get { return isProcess; }
            set { isProcess = value; RaisePropertyChanged("IsProcess"); }
        }

        public int ProcessModule
        {
            get { return nProcessModule; }
            set { nProcessModule = value; RaisePropertyChanged("ProcessModule"); }
        }
    }

    public class ProcessADHDataCls : ViewModelBase
    {
        public ObservableCollection<ADHStepCls> StepList { get; set; } = new ObservableCollection<ADHStepCls>();
        float fSetValue = 0;
        float fAlarmMax = 0;
        float fAlarmMin = 0;
        float fStopMax = 0;
        float fStopMin = 0;

        int nDispBlock = 0;
        int nDispModule = 0;        

        public ProcessADHDataCls()
        {
            
        }
        public void Clear()
        {
            SetValue = 0;
            AlarmMaxValue = 0;
            AlarmMinValue = 0;
            StopMaxValue = 0;
            StopMinValue = 0;

            DispBlock = 0;
            DispModule = 0;
        }
        public float SetValue
        {
            get { return fSetValue; }
            set { fSetValue = value; RaisePropertyChanged("SetValue"); }
        }

        public float AlarmMaxValue
        {
            get { return fAlarmMax; }
            set { fAlarmMax = value; RaisePropertyChanged("AlarmMaxValue"); }
        }

        public float AlarmMinValue
        {
            get { return fAlarmMin; }
            set { fAlarmMin = value; RaisePropertyChanged("AlarmMinValue"); }
        }

        public float StopMaxValue
        {
            get { return fStopMax; }
            set { fStopMax = value; RaisePropertyChanged("StopMaxValue"); }
        }

        public float StopMinValue
        {
            get { return fStopMin; }
            set { fStopMin = value; RaisePropertyChanged("StopMinValue"); }
        }

        public int DispBlock
        {
            get { return nDispBlock; }
            set { nDispBlock = value; RaisePropertyChanged("DispBlock"); }
        }

        public int DispModule
        {
            get { return nDispModule; }
            set { nDispModule = value; RaisePropertyChanged("DispModule"); }
        }
    }

    public class ADHStepCls : ViewModelBase
    {
        int nIndex = 0;
        string sName = string.Empty;
        float fStepTime = 0;
        int nLoop = 0;
        string sLoopDisplaly = string.Empty;
        uint nDispenseNo = 0;
        string sDispenseDisplay = string.Empty;
        bool isVapor = false;
        bool isExhaust = false;
        bool isPress = false;
        bool isPinPos = false;
        string sPinDesc = string.Empty;
        
        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; RaisePropertyChanged("Name"); }
        }

        public float StepTime
        {
            get { return fStepTime; }
            set { fStepTime = value; RaisePropertyChanged("StepTime"); }
        }

        public int Loop
        {
            get { return nLoop; }
            set
            {
                nLoop = value;

                if (Loop == 0) LoopDisplay = string.Empty;
                else if (Loop == 2) LoopDisplay = "END";
                else
                {
                    LoopDisplay = "START / " + (Loop - 1).ToString();
                }
                RaisePropertyChanged("Loop");
            }
        }

        public string LoopDisplay
        {
            get { return sLoopDisplaly; }
            set { sLoopDisplaly = value; RaisePropertyChanged("LoopDisplay"); }
        }

        public uint DispenseNo
        {
            get { return nDispenseNo; }
            set { 
                nDispenseNo = value;
                string disp = string.Empty;
                for (int i = 0; i < Global.STDispenseIndex.Length; i++)
                {
                    if (Convert.ToBoolean(Global.STDispenseIndex[i] & DispenseNo))
                    {
                        disp += (i + 1).ToString() + ",";
                    }
                }

                if (disp != string.Empty)
                    DispenseDisplay = disp.Substring(0, disp.Length - 1);
                RaisePropertyChanged("DispenseNo"); }
        }

        public string DispenseDisplay
        {
            get { return sDispenseDisplay; }
            set { sDispenseDisplay = value; RaisePropertyChanged("DispenseDisplay"); }
        }

        public bool IsVapor
        {
            get { return isVapor; }
            set { isVapor = value; RaisePropertyChanged("IsVapor"); }
        }

        public bool IsExhaust
        {
            get { return isExhaust; }
            set { isExhaust = value; RaisePropertyChanged("IsExhaust"); }
        }

        public bool IsPress
        {
            get { return isPress; }
            set { isPress = value; RaisePropertyChanged("IsPress"); }
        }

        public bool IsPinPos
        {
            get { return isPinPos; }
            set { isPinPos = value;
                if (IsPinPos) PinDesc = "Up";
                else PinDesc = "Down";
                RaisePropertyChanged("IsPinPos"); 
            }
        }

        public string PinDesc
        {
            get { return sPinDesc; }
            set { sPinDesc = value; RaisePropertyChanged("PinDesc"); }
        }
    }

    public class ProcessCotDataCls : ViewModelBase
    {
        public ObservableCollection<SpinChamberStepCls> StepList { get; set; } = new ObservableCollection<SpinChamberStepCls>();
        int nPosEnd = 0;
        int nPos3 = 0;
        int nPos2 = 0;
        int nPos1 = 0;
        int nPosBegin = 0;
        int nDispBlock = 0;
        int nDispModule = 0;
        int nStopRange = 0;
        int nAlarmRange = 0;
        string sPumpRecipe = string.Empty;

        public ProcessCotDataCls()
        {   
        }

        public void Clear()
        {
            PosEnd = 0;
            Pos3 = 0;
            Pos2 = 0;
            Pos1 = 0;
            PosBegin = 0;
            DispBlock = 0;
            DispModule = 0;
            StopRange = 0;
            AlarmRange = 0;
            sPumpRecipe = string.Empty;
        }

        public string PumpRecipe
        {
            get { return sPumpRecipe; }
            set { sPumpRecipe = value; RaisePropertyChanged("PumpRecipe"); }
        }

        public int PosEnd
        {
            get { return nPosEnd; }
            set { nPosEnd = value; RaisePropertyChanged("PosEnd"); }
        }

        public int Pos1
        {
            get { return nPos1; }
            set { nPos1 = value; RaisePropertyChanged("Pos1"); }
        }

        public int Pos2
        {
            get { return nPos2; }
            set { nPos2 = value; RaisePropertyChanged("Pos2"); }
        }

        public int Pos3
        {
            get { return nPos3; }
            set { nPos3 = value; RaisePropertyChanged("Pos3"); }
        }

        public int PosBegin
        {
            get { return nPosBegin; }
            set { nPosBegin = value; RaisePropertyChanged("PosBegin"); }
        }

        public int DispBlock
        {
            get { return nDispBlock; }
            set { nDispBlock = value; RaisePropertyChanged("DispBlock"); }
        }

        public int DispModule
        {
            get { return nDispModule; }
            set { nDispModule = value; RaisePropertyChanged("DispModule"); }
        }

        public int StopRange
        {
            get { return nStopRange; }
            set { nStopRange = value; RaisePropertyChanged("StopRange"); }
        }

        public int AlarmRange
        {
            get { return nAlarmRange; }
            set { nAlarmRange = value; RaisePropertyChanged("AlarmRange"); }
        }
    }

    //Cot Dev
    public class SpinChamberStepCls : ViewModelBase
    {
        int nIndex = 0;
        string sName = string.Empty;
        float fStepTime = 0;
        int nSpinSpeed = 0;
        int nSpinAcc = 0;
        int nLoop = 0;
        string sLoopDisplay = string.Empty;
        bool isRRC = false;
        bool isDispenseLiquid = false; //Rinse, Develop
        bool isBackRinse = false;
        bool isExhaust = false;
        uint nDispNo = 0;
        string sDispDisplay = string.Empty;
        string sArm1Pos = string.Empty;
        int nArm1Speed = 0;
        bool isArm1MoveWait = false;
        string sArm2Pos = string.Empty;
        int nArm2Speed = 0;
        bool isArm2MoveWait = false;
        string sPumpRecipe = string.Empty;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; RaisePropertyChanged("Name"); }
        }

        public float StepTime
        {
            get { return fStepTime; }
            set { fStepTime = value; RaisePropertyChanged("StepTime"); }
        }

        public int SpinSpeed
        {
            get { return nSpinSpeed; }
            set { nSpinSpeed = value; RaisePropertyChanged("SpinSpeed"); }
        }

        public int SpinAcc
        {
            get { return nSpinAcc; }
            set { nSpinAcc = value; RaisePropertyChanged("SpinAcc"); }
        }

        public int Loop
        {
            get { return nLoop; }
            set { nLoop = value;

                if (Loop == 0) LoopDisplay = string.Empty;
                else if (Loop == 2) LoopDisplay = "END";
                else
                {
                    if (Loop - 0x10000 < 0) LoopDisplay = "START / " + Loop.ToString();
                    else LoopDisplay = "START / " + (Loop - 0x10000).ToString();
                }
                RaisePropertyChanged("Loop"); 
            }
        }

        public string LoopDisplay
        {
            get { return sLoopDisplay; }
            set
            {
                sLoopDisplay = value;
                RaisePropertyChanged("LoopDisplay");
            }
        }

        public bool IsRRC
        {
            get { return isRRC; }
            set { isRRC = value; RaisePropertyChanged("IsRRC"); }
        }

        public bool IsDispenseLiquid
        {
            get { return isDispenseLiquid; }
            set { isDispenseLiquid = value; RaisePropertyChanged("IsDispenseLiquid"); }
        }

        public bool IsBackRinse
        {
            get { return isBackRinse; }
            set { isBackRinse = value; RaisePropertyChanged("IsBackRinse"); }
        }

        public bool IsExhaust
        {
            get { return isExhaust; }
            set { isExhaust = value; RaisePropertyChanged("IsExhaust"); }
        }

        public uint DispNo
        {
            get { return nDispNo; }
            set { nDispNo = value;
                string disp = string.Empty;
                for (int i = 0; i < Global.STDispenseIndex.Length; i++)
                {                    
                    if(Convert.ToBoolean(Global.STDispenseIndex[i] & DispNo))
                    {
                        disp += (i + 1).ToString() + ",";
                    }
                }

                if(disp != string.Empty) 
                    DispDisplay = disp.Substring(0, disp.Length - 1);
                RaisePropertyChanged("DispNo"); }
        }

        public string DispDisplay
        {
            get { return sDispDisplay; }
            set { sDispDisplay = value; RaisePropertyChanged("DispDisplay"); }
        }

        public string Arm1Pos
        {
            get { return sArm1Pos; }
            set { sArm1Pos = value; RaisePropertyChanged("Arm1Pos"); }
        }

        public int Arm1Speed
        {
            get { return nArm1Speed; }
            set { nArm1Speed = value; RaisePropertyChanged("Arm1Speed"); }
        }

        public bool IsArm1MoveWait
        {
            get { return isArm1MoveWait; }
            set {
                if (!Global.GetRecipeEditMode()) return;
                isArm1MoveWait = value; RaisePropertyChanged("IsArm1MoveWait"); }
        }

        public string Arm2Pos
        {
            get { return sArm2Pos; }
            set { sArm2Pos = value; RaisePropertyChanged("Arm2Pos"); }
        }

        public int Arm2Speed
        {
            get { return nArm2Speed; }
            set { nArm2Speed = value; RaisePropertyChanged("Arm2Speed"); }
        }

        public bool IsArm2MoveWait
        {
            get { return isArm2MoveWait; }
            set {
                if (!Global.GetRecipeEditMode()) return;
                isArm2MoveWait = value; RaisePropertyChanged("IsArm2MoveWait"); }
        }

        public string PumpRecipe
        {
            get { return sPumpRecipe; }
            set { sPumpRecipe = value; RaisePropertyChanged("PumpRecipe"); }
        }
    } 

    public class ProcessDevDataCls : ViewModelBase
    {
        public ObservableCollection<SpinChamberStepCls> StepList { get; set; }
        int nPosEnd = 0;
        int nPos3 = 0;
        int nPos2 = 0;
        int nPos1 = 0;
        int nPosBegin = 0;
        int nDispBlock = 0;
        int nDispModule = 0;
        int nStopRange = 0;
        int nAlarmRange = 0;

        public ProcessDevDataCls()
        {
            StepList = new ObservableCollection<SpinChamberStepCls>();
        }

        public void Clear()
        {
            PosEnd = 0;
            Pos3 = 0;
            Pos2 = 0;
            Pos1 = 0;
            PosBegin = 0;
            DispBlock = 0;
            DispModule = 0;
            StopRange = 0;
            AlarmRange = 0;
        }

        public int PosEnd
        {
            get { return nPosEnd; }
            set { nPosEnd = value; RaisePropertyChanged("PosEnd"); }
        }

        public int Pos1
        {
            get { return nPos1; }
            set { nPos1 = value; RaisePropertyChanged("Pos1"); }
        }

        public int Pos2
        {
            get { return nPos2; }
            set { nPos2 = value; RaisePropertyChanged("Pos2"); }
        }

        public int Pos3
        {
            get { return nPos3; }
            set { nPos3 = value; RaisePropertyChanged("Pos3"); }
        }

        public int PosBegin
        {
            get { return nPosBegin; }
            set { nPosBegin = value; RaisePropertyChanged("PosBegin"); }
        }

        public int DispBlock
        {
            get { return nDispBlock; }
            set { nDispBlock = value; RaisePropertyChanged("DispBlock"); }
        }

        public int DispModule
        {
            get { return nDispModule; }
            set { nDispModule = value; RaisePropertyChanged("DispModule"); }
        }

        public int StopRange
        {
            get { return nStopRange; }
            set { nStopRange = value; RaisePropertyChanged("StopRange"); }
        }

        public int AlarmRange
        {
            get { return nAlarmRange; }
            set { nAlarmRange = value; RaisePropertyChanged("AlarmRange"); }
        }
    }

    //HHP LHP CPL TCP
    public class ProcessChamberDataCls : ViewModelBase
    {
        public ObservableCollection<ChamberStepCls> StepList { get; set; } = new ObservableCollection<ChamberStepCls>();
        float fSetValue = 0;
        float fAlarmMax = 0;
        float fAlarmMin = 0;
        float fStopMax = 0;
        float fStopMin = 0;
        public ProcessChamberDataCls()
        {
            
        }
        public void Clear()
        {
            SetValue = 0;
            AlarmMaxValue = 0;
            AlarmMinValue = 0;
            StopMaxValue = 0;
            StopMinValue = 0;
        }
        public float SetValue
        {
            get { return fSetValue; }
            set { fSetValue = value; RaisePropertyChanged("SetValue"); }
        }

        public float AlarmMaxValue
        {
            get { return fAlarmMax; }
            set { fAlarmMax = value; RaisePropertyChanged("AlarmMaxValue"); }
        }

        public float AlarmMinValue
        {
            get { return fAlarmMin; }
            set { fAlarmMin = value; RaisePropertyChanged("AlarmMinValue"); }
        }

        public float StopMaxValue
        {
            get { return fStopMax; }
            set { fStopMax = value; RaisePropertyChanged("StopMaxValue"); }
        }

        public float StopMinValue
        {
            get { return fStopMin; }
            set { fStopMin = value; RaisePropertyChanged("StopMinValue"); }
        }
    }

    public class ChamberStepCls : ViewModelBase
    {
        string sName = string.Empty;
        float fStepTime = 0;
        bool isPinPos = false;
        bool isShutter = false;
        string sPinDesc = string.Empty;
        string sShutterDesc = string.Empty;
        int nIndex = 0;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; RaisePropertyChanged("Name"); }
        }

        public float StepTime
        {
            get { return fStepTime; }
            set { fStepTime = value; RaisePropertyChanged("StepTime"); }
        }

        public bool IsPinPos
        {
            get { return isPinPos; }
            set { 
                isPinPos = value;
                if (IsPinPos) PinDesc = "Up";
                else PinDesc = "Down";
                RaisePropertyChanged("IsPinPos"); 
            }
        }

        public string PinDesc
        {
            get { return sPinDesc; }
            set { sPinDesc = value; RaisePropertyChanged("PinDesc"); }
        }

        public bool IsShutter
        {
            get { return isShutter; }
            set { 
                isShutter = value;
                if (IsShutter) ShutterDesc = "Open";
                else ShutterDesc = "Close";
                RaisePropertyChanged("IsShutter"); 
            }
        }

        public string ShutterDesc
        {
            get { return sShutterDesc; }
            set { sShutterDesc = value; RaisePropertyChanged("ShutterDesc"); }
        }
    }

    public class ProcessPumpDataCls : ViewModelBase
    {
        float fDisAmount = 0;
        float fDistRate = 0;
        int nAcc = 0;
        int nDec = 0;
        float fReloadRate = 0;
        float fCal = 0;
        float fAvCloseDelayTime = 0;

        public void Clear()
        {
            DisAmount = 0;
            DistRate = 0;
            Acc = 0;
            Dec = 0;
            ReloadRate = 0;
            Cal = 0;
            AvCloseDelayTime = 0;
        }

        public float DisAmount
        {
            get { return fDisAmount; }
            set { fDisAmount = value; RaisePropertyChanged("DisAmount"); }
        }

        public float DistRate
        {
            get { return fDistRate; }
            set { fDistRate = value; RaisePropertyChanged("DistRate"); }
        }

        public int Acc
        {
            get { return nAcc; }
            set { nAcc = value; RaisePropertyChanged("Acc"); }
        }

        public int Dec
        {
            get { return nDec; }
            set { nDec = value; RaisePropertyChanged("Dec"); }
        }

        public float ReloadRate
        {
            get { return fReloadRate; }
            set { fReloadRate = value; RaisePropertyChanged("ReloadRate"); }
        }

        public float Cal
        {
            get { return fCal; }
            set { fCal = value; RaisePropertyChanged("Cal"); }
        }

        public float AvCloseDelayTime
        {
            get { return fAvCloseDelayTime; }
            set { fAvCloseDelayTime = value; RaisePropertyChanged("AvCloseDelayTime"); }
        }        
    }

    public class SystemRecipeStepCls : ViewModelBase
    {
        int nIndex = 0;
        int nBlockNo = 2;
        int nModuleNo = 0;        
        string sControlTarget = string.Empty;
        string sModuleDisplay = string.Empty;
        float fSetValue = 0;
        float fAlarmMax = 0;
        float fAlarmMin = 0;
        float fStopMax = 0;
        float fStopMin = 0;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return nModuleNo; }
            set { nModuleNo = value; 
                if(ModuleNo != 0)
                {
                    Model.ModuleBaseCls mod = Global.GetModule(BlockNo, ModuleNo);
                    if (mod != null) ModuleDisplay = string.Format("{0} {1}-{2}", mod.MachineFullName, mod.BlockNo, mod.ModuleNo);

                }
                RaisePropertyChanged("ModuleNo"); 
            }
        }
        public string ModuleDisplay
        {
            get { return sModuleDisplay; }
            set { sModuleDisplay = value; RaisePropertyChanged("ModuleDisplay"); }
        }
        public string ControlTarget
        {
            get { return sControlTarget; }
            set { sControlTarget = value; RaisePropertyChanged("ControlTarget"); }
        }
        public float SetValue
        {
            get { return fSetValue; }
            set { fSetValue = value; RaisePropertyChanged("SetValue"); }
        }
        public float AlarmMaxValue
        {
            get { return fAlarmMax; }
            set { fAlarmMax = value; RaisePropertyChanged("AlarmMaxValue"); }
        }

        public float AlarmMinValue
        {
            get { return fAlarmMin; }
            set { fAlarmMin = value; RaisePropertyChanged("AlarmMinValue"); }
        }

        public float StopMaxValue
        {
            get { return fStopMax; }
            set { fStopMax = value; RaisePropertyChanged("StopMaxValue"); }
        }

        public float StopMinValue
        {
            get { return fStopMin; }
            set { fStopMin = value; RaisePropertyChanged("StopMinValue"); }
        }
    }

    public class DummyConditionStepCls : ViewModelBase
    {
        int nIndex = 0;
        uint nDispenseNo = 0;
        string sDipsenseDisplay = string.Empty;
        int nWaferCnt = 0;
        int nInterval = 0;
        int nLotspec = 0;
        string sLotspecDisplay = string.Empty;
        bool isCond = false;
        string sCondDisplay = string.Empty;
        int nTiming = 0;
        string sTimingDisplay = string.Empty;
        string sRecipe = string.Empty;
        bool isRecipeUse = false;
        int nRecipeCnt = 0;
        int nRecipeInterval = 0;
        int nRecipeTime = 0;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public uint DispenseNo
        {
            get { return nDispenseNo; }
            set 
            {   
                nDispenseNo = value;
                RaisePropertyChanged("DispenseNo"); 
            }
        }

        public string DipsenseDisplay
        {
            get { return sDipsenseDisplay; }
            set { sDipsenseDisplay = value; RaisePropertyChanged("DipsenseDisplay"); }
        }

        public int WaferCnt
        {
            get { return nWaferCnt; }
            set { nWaferCnt = value; RaisePropertyChanged("WaferCnt"); }
        }

        public int Interval
        {
            get { return nInterval; }
            set { nInterval = value; RaisePropertyChanged("Interval"); }
        }

        public int Lotspec
        {
            get { return nLotspec; }
            set { 
                nLotspec = value;
                if (Lotspec == 0) LotspecDisplay = "Unspecified";
                else LotspecDisplay = "Specified";
                RaisePropertyChanged("Lotspec"); 
            }
        }

        public string LotspecDisplay
        {
            get { return sLotspecDisplay; }
            set { sLotspecDisplay = value; RaisePropertyChanged("LotspecDisplay"); }
        }

        public bool IsCond
        {
            get { return isCond; }
            set { 
                isCond = value;
                if (IsCond) CondDisplay = "AND";
                else CondDisplay = "OR";
                RaisePropertyChanged("IsCond"); 
            }
        }

        public string CondDisplay
        {
            get { return sCondDisplay; }
            set { sCondDisplay = value; RaisePropertyChanged("CondDisplay"); }
        }

        public int Timing
        {
            get { return nTiming; }
            set { 
                nTiming = value;
                if (Timing == 0) TimingDisplay = "Pre-recv"; 
                else if (Timing == 1) TimingDisplay = "Pre-proc";
                else if (Timing == 2) TimingDisplay = "Post-proc";
                else TimingDisplay = "Processing";
                RaisePropertyChanged("Timing"); 
            }
        }

        public string TimingDisplay
        {
            get { return sTimingDisplay; }
            set { sTimingDisplay = value; RaisePropertyChanged("TimingDisplay"); }
        }

        public string Recipe
        {
            get { return sRecipe; }
            set { sRecipe = value; RaisePropertyChanged("Recipe"); }
        }

        public bool IsRecipeUse
        {
            get { return isRecipeUse; }
            set { isRecipeUse = value; RaisePropertyChanged("IsRecipeUse"); }
        }

        public int RecipeCnt
        {
            get { return nRecipeCnt; }
            set { nRecipeCnt = value; RaisePropertyChanged("RecipeCnt"); }
        }

        public int RecipeInterval
        {
            get { return nRecipeInterval; }
            set { nRecipeInterval = value; RaisePropertyChanged("RecipeInterval"); }
        }

        public int RecipeTime
        {
            get { return nRecipeTime; }
            set { nRecipeTime = value; RaisePropertyChanged("RecipeTime"); }
        }
    }

    public class DummyConditionLinkStepCls : ViewModelBase
    {
        int nBlockNo = 2;
        int nModuleNo = 0;
        int nIndex = 0;
        string sRecipeName = string.Empty;
        string sModuleDiplay = string.Empty;
        
        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return nModuleNo; }
            set { 
                nModuleNo = value;
                if (ModuleNo == 0) ModuleDisplay = "";
                else
                {
                    Model.ModuleBaseCls mod = Global.GetModule(BlockNo, ModuleNo);
                    ModuleDisplay = string.Format("{0} {1}-{2}", mod.MachineFullName, mod.BlockNo, mod.ModuleNo);
                }
                
                RaisePropertyChanged("ModuleNo"); }
        }

        public string ModuleDisplay
        {
            get { return sModuleDiplay; }
            set { sModuleDiplay = value; RaisePropertyChanged("ModuleDisplay"); }
        }

        public string RecipeName
        {
            get { return sRecipeName; }
            set { sRecipeName = value; RaisePropertyChanged("RecipeName"); }
        }
    }

    public class DummyConditionCls : ViewModelBase
    {
        int nStep = 0;
        int nDispBlock = 0;
        int nDispModule = 0;
        string sPumpRecipe = string.Empty;
        public ObservableCollection<DummyConditionStepCls> StepList { get; set; }

        public DummyConditionCls()
        {
            StepList = new ObservableCollection<DummyConditionStepCls>();
        }

        public void Clear()
        {
            Step = 0;
            DispBlock = 0;
            DispModule = 0;
            PumpRecipe = string.Empty;
        }

        public int Step
        {
            get { return nStep; }
            set { nStep = value; RaisePropertyChanged("Step"); }
        }

        public int DispBlock
        {
            get { return nDispBlock; }
            set { nDispBlock = value; RaisePropertyChanged("DispBlock"); }
        }

        public int DispModule
        {
            get { return nDispModule; }
            set { nDispModule = value; RaisePropertyChanged("DispModule"); }
        }

        public string PumpRecipe
        {
            get { return sPumpRecipe; }
            set { sPumpRecipe = value; RaisePropertyChanged("PumpRecipe"); }
        }
    }

    public class DummyConditionLinkCls : ViewModelBase
    {
        public ObservableCollection<DummyConditionLinkStepCls> StepList { get; set; }
        public DummyConditionLinkCls()
        {
            StepList = new ObservableCollection<DummyConditionLinkStepCls>();
        }
    }

    public class CleanStepCls : ViewModelBase
    {
        int nIndex = 0;
        string sName = string.Empty;
        int nLoop = 0;
        float fStepTime = 0;
        int nSpinSpeed = 0;
        int nSpinAcc = 0;
        uint nDispenseNo = 0;
        string sDispenseDisplay = string.Empty;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public string Name
        {
            get { return sName; }
            set { sName = value; RaisePropertyChanged("Name"); }
        }

        public int Loop
        {
            get { return nLoop; }
            set { nLoop = value; RaisePropertyChanged("Loop"); }
        }

        public float StepTime
        {
            get { return fStepTime; }
            set { fStepTime = value; RaisePropertyChanged("StepTime"); }
        }

        public int SpinSpeed
        {
            get { return nSpinSpeed; }
            set { nSpinSpeed = value; RaisePropertyChanged("SpinSpeed"); }
        }

        public int SpinAcc
        {
            get { return nSpinAcc; }
            set { nSpinAcc = value; RaisePropertyChanged("SpinAcc"); }
        }

        public uint DispenseNo
        {
            get { return nDispenseNo; }
            set { 
                nDispenseNo = value;
                string disp = string.Empty;
                for (int i = 0; i < Global.STDispenseIndex.Length; i++)
                {
                    if (Convert.ToBoolean(Global.STDispenseIndex[i] & DispenseNo))
                    {
                        disp += (i + 1).ToString() + ",";
                    }
                }

                if (disp != string.Empty)
                    DispenseDisplay = disp.Substring(0, disp.Length - 1);
                RaisePropertyChanged("nDispenseNo"); }
        }

        public string DispenseDisplay
        {
            get { return sDispenseDisplay; }
            set { sDispenseDisplay = value; RaisePropertyChanged("DispenseDisplay"); }
        }
    }

    public class CleanCondStepCls : ViewModelBase
    {
        int nIndex = 0;
        int nBlockNo = 2;
        int nModuleNo = 0;        
        int nJigModuleNo = 0;
        int nCnt = 0;
        int nInterval = 0;
        bool isCond = false;
        string sRecipeName = string.Empty;
        string sModuleDisplay = string.Empty;

        public int Index
        {
            get { return nIndex; }
            set { nIndex = value; RaisePropertyChanged("Index"); }
        }

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return nModuleNo; }
            set {
                if (ModuleNo == 0) ModuleDisplay = string.Empty;
                else
                {
                    Model.ModuleBaseCls mod = Global.GetModule(BlockNo, ModuleNo);
                    if(mod != null)
                    {
                        ModuleDisplay = string.Format("{0} {1}-{2}", mod.MachineFullName, mod.BlockNo, mod.ModuleNo);
                    }
                }
                nModuleNo = value; 
                RaisePropertyChanged("ModuleNo"); 
            }
        }

        public string ModuleDisplay
        {
            get { return sModuleDisplay; }
            set { sModuleDisplay = value; RaisePropertyChanged("ModuleDisplay"); }
        }

        public int JigModuleNo
        {
            get { return nJigModuleNo; }
            set { nJigModuleNo = value; RaisePropertyChanged("JigModuleNo"); }
        }

        public int Cnt
        {
            get { return nCnt; }
            set { nCnt = value; RaisePropertyChanged("Cnt"); }
        }

        public int Interval
        {
            get { return nInterval; }
            set { nInterval = value; RaisePropertyChanged("Interval"); }
        }

        public bool IsCond
        {
            get { return isCond; }
            set { isCond = value; RaisePropertyChanged("IsCond"); }
        }

        public string RecipeName
        {
            get { return sRecipeName; }
            set { sRecipeName = value; RaisePropertyChanged("RecipeName"); }
        }
    }

    public class CleanDataCls : ViewModelBase
    {
        int nStopRange = 0;
        int nAlarmRange = 0;
        int nDispBlock = 0;
        int nDispModule = 0;
        string sPumpRecipe = string.Empty;
        public ObservableCollection<CleanStepCls> StepList { get; set; }
        public CleanDataCls()
        {
            StepList = new ObservableCollection<CleanStepCls>();
        }

        public void Clear()
        {
            StopRange = 0;
            AlarmRange = 0;
            DispBlock = 0;
            DispModule = 0;
            PumpRecipe = string.Empty;
        }

        public int StopRange
        {
            get { return nStopRange; }
            set { nStopRange = value; RaisePropertyChanged("StopRange"); }
        }

        public int AlarmRange
        {
            get { return nAlarmRange; }
            set { nAlarmRange = value; RaisePropertyChanged("AlarmRange"); }
        }

        public int DispBlock
        {
            get { return nDispBlock; }
            set { nDispBlock = value; RaisePropertyChanged("DispBlock"); }
        }

        public int DispModule
        {
            get { return nDispModule; }
            set { nDispModule = value; RaisePropertyChanged("DispModule"); }
        }

        public string PumpRecipe
        {
            get { return sPumpRecipe; }
            set { sPumpRecipe = value; RaisePropertyChanged("PumpRecipe"); }
        }
    }

    public class CleanCondDataCls : ViewModelBase
    {
        public ObservableCollection<CleanCondStepCls> StepList { get; set; }
        public CleanCondDataCls()
        {
            StepList = new ObservableCollection<CleanCondStepCls>();
        }
    }

    public class SystemRecipeCls : ViewModelBase
    {
        public ObservableCollection<SystemRecipeStepCls> StepList { get; set; }
        public SystemRecipeCls()
        {
            StepList = new ObservableCollection<SystemRecipeStepCls>();
        }
    }

    public class AlarmLogCls : ViewModelBase
    {
        string title = string.Empty;
        string code = string.Empty;
        string message = string.Empty;
        string help = string.Empty;
        string owner = string.Empty;
        string maker = string.Empty;
        string time = string.Empty;
        string param = string.Empty;
        int sendID = -1;
        AlarmRecord alramRecod = null;
        enAlarmMessageType messageType = enAlarmMessageType.ALARM; 

        public int SendID
        {
            get { return sendID; }
            set { sendID = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }

        public string Code
        {
            get { return code; }
            set { code = value; RaisePropertyChanged("Code"); }
        }

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged("Message"); }
        }

        public string Owner
        {
            get { return owner; }
            set { owner = value; RaisePropertyChanged("Owner"); }
        }

        public string Param
        {
            get { return param; }
            set { param = value; }
        }

        public string Maker
        {
            get { return maker; }
            set { maker = value; RaisePropertyChanged("Maker"); }
        }

        public string Help
        {
            get { return help; }
            set { help = value; RaisePropertyChanged("Help"); }
        }

        public string Time
        {
            get { return time; }
            set { time = value; RaisePropertyChanged("Time"); }
        }

        public enAlarmMessageType MessageType
        {
            get { return messageType; }
            set { messageType = value; RaisePropertyChanged("MessageType"); }
        }

        public AlarmRecord AlarmRecord
        {
            get { return alramRecod; }
            set { alramRecod = value; RaisePropertyChanged("AlarmRecord"); }
        }
    }

    public class DirFileListCls : ViewModelBase
    {
        int index = 0;
        string fileName = string.Empty;
        string fileExtension = string.Empty;
        string fileFullName = string.Empty;
        string filePath = string.Empty;
        bool isCheck = false; //나중에 리스트 체크해야 함.

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; RaisePropertyChanged("FileName"); }
        }

        public string FileExtension
        {
            get { return fileExtension; }
            set { fileExtension = value; RaisePropertyChanged("FileExtension"); }
        }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public string FileFullName
        {
            get { return fileFullName; }
            set { fileFullName = value; RaisePropertyChanged("FileFullName"); }
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("IsCheck"); }
        }
    }

    public class FlowControlDataCls : ViewModelBase
    {
        int nBlockNo = 0;
        int nModuleNo = 0;
        int nDispIndex = 0;
        bool bCheckTiming = false;
        bool bFlowMonitoring = false;

        string sDispName = string.Empty;
        string sSensorName = string.Empty;

        float fPulseRate = 0;
        float fSamplingDelayTime = 0;
        float fReferenceValue = 0;
        float fCalibration = 0;
        float fAlarmUpper = 0;
        float fAlarmLower = 0;
        float fStopUpper = 0;
        float fStopLower = 0;

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return nModuleNo; }
            set { nModuleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public int DispIndex
        {
            get { return nDispIndex; }
            set { nDispIndex = value; RaisePropertyChanged("DispIndex"); }
        }
        public bool CheckTiming
        {
            get { return bCheckTiming; }
            set { bCheckTiming = value; RaisePropertyChanged("CheckTiming"); }
        }
        public bool FlowMonitoring
        {
            get { return bFlowMonitoring; }
            set { bFlowMonitoring = value; RaisePropertyChanged("FlowMonitoring"); }
        }
        public string DispName
        {
            get { return sDispName; }
            set { sDispName = value; RaisePropertyChanged("DispName"); }
        }
        public string SensorName
        {
            get { return sSensorName; }
            set { sSensorName = value; RaisePropertyChanged("SensorName"); }
        }
        public float PulseRate
        {
            get { return fPulseRate; }
            set { fPulseRate = value; RaisePropertyChanged("PulseRate"); }
        }
        public float SamplingDelayTime
        {
            get { return fSamplingDelayTime; }
            set { fSamplingDelayTime = value; RaisePropertyChanged("SamplingDelayTime"); }
        }
        public float ReferenceValue
        {
            get { return fReferenceValue; }
            set { fReferenceValue = value; RaisePropertyChanged("ReferenceValue"); }
        }
        public float Calibration
        {
            get { return fCalibration; }
            set { fCalibration = value; RaisePropertyChanged("Calibration"); }
        }
        public float AlarmUpper
        {
            get { return fAlarmUpper; }
            set { fAlarmUpper = value; RaisePropertyChanged("AlarmUpper"); }
        }
        public float AlarmLower
        {
            get { return fAlarmLower; }
            set { fAlarmLower = value; RaisePropertyChanged("AlarmLower"); }
        }
        public float StopUpper
        {
            get { return fStopUpper; }
            set { fStopUpper = value; RaisePropertyChanged("StopUpper"); }
        }
        public float StopLower
        {
            get { return fStopLower; }
            set { fStopLower = value; RaisePropertyChanged("StopLower"); }
        }
    }

    public class PumpControlDataCls : ViewModelBase
    {
        int nBlockNo = 0;
        int nModuleNo = 0;
        int nDispIndex = 0;
        bool bPassOper = false;
        int nPulseCount = 0;
        int nSpareReload = 0;
        int nTotalDispCountAlarm = 0;
        int nTotalDispCountStop = 0;

        string sDispName = string.Empty;
        string sPumpName = string.Empty;

        float fPumpCapa = 0;
        float fCalibration = 0;

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return nModuleNo; }
            set { nModuleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public int DispIndex
        {
            get { return nDispIndex; }
            set { nDispIndex = value; RaisePropertyChanged("DispIndex"); }
        }
        public bool PassOper
        {
            get { return bPassOper; }
            set { bPassOper = value; RaisePropertyChanged("PassOper"); }
        }
        public int PulseCount
        {
            get { return nPulseCount; }
            set { nPulseCount = value; RaisePropertyChanged("PulseCount"); }
        }
        public int SpareReload
        {
            get { return nSpareReload; }
            set { nSpareReload = value; RaisePropertyChanged("SpareReload"); }
        }
        public int TotalDispCountAlarm
        {
            get { return nTotalDispCountAlarm; }
            set { nTotalDispCountAlarm = value; RaisePropertyChanged("TotalDispCountAlarm"); }
        }
        public int TotalDispCountStop
        {
            get { return nTotalDispCountStop; }
            set { nTotalDispCountStop = value; RaisePropertyChanged("TotalDispCountStop"); }
        }
        public string DispName
        {
            get { return sDispName; }
            set { sDispName = value; RaisePropertyChanged("DispName"); }
        }
        public string PumpName
        {
            get { return sPumpName; }
            set { sPumpName = value; RaisePropertyChanged("PumpName"); }
        }
        public float PumpCapa
        {
            get { return fPumpCapa; }
            set { fPumpCapa = value; RaisePropertyChanged("PumpCapa"); }
        }
        public float Calibration
        {
            get { return fCalibration; }
            set { fCalibration = value; RaisePropertyChanged("Calibration"); }
        }
    }

    public class AutoSupplyControlDataCls : ViewModelBase
    {
        int nBlockNo = 0;
        int nModuleNo = 0;
        int nDispIndex = 0;
        bool bAutoMode = false;
        int nLiquidSource = 0;
        int nSupplyTime = 0;
        int nVacuumeTime = 0;
        int nWaitTime = 0;
        int nSupplyDelayTime = 0;
        int nPurgeTime = 0;
        string sDispName = string.Empty;
        string sAutoSupplyName = string.Empty;

        public int BlockNo
        {
            get { return nBlockNo; }
            set { nBlockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return nModuleNo; }
            set { nModuleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public int DispIndex
        {
            get { return nDispIndex; }
            set { nDispIndex = value; RaisePropertyChanged("DispIndex"); }
        }
        public bool AutoMode
        {
            get { return bAutoMode; }
            set { bAutoMode = value; RaisePropertyChanged("AutoMode"); }
        }
        public int LiquidSource
        {
            get { return nLiquidSource; }
            set { nLiquidSource = value; RaisePropertyChanged("LiquidSource"); }
        }
        public int SupplyTime
        {
            get { return nSupplyTime; }
            set { nSupplyTime = value; RaisePropertyChanged("SupplyTime"); }
        }
        public int VacuumeTime
        {
            get { return nVacuumeTime; }
            set { nVacuumeTime = value; RaisePropertyChanged("VacuumeTime"); }
        }
        public int WaitTime
        {
            get { return nWaitTime; }
            set { nWaitTime = value; RaisePropertyChanged("WaitTime"); }
        }
        public int SupplyDelayTime
        {
            get { return nSupplyDelayTime; }
            set { nSupplyDelayTime = value; RaisePropertyChanged("SupplyDelayTime"); }
        }
        public int PurgeTime
        {
            get { return nPurgeTime; }
            set { nPurgeTime = value; RaisePropertyChanged("PurgeTime"); }
        }
        public string DispName
        {
            get { return sDispName; }
            set { sDispName = value; RaisePropertyChanged("DispName"); }
        }
        public string AutoSupplyName
        {
            get { return sAutoSupplyName; }
            set { sAutoSupplyName = value; RaisePropertyChanged("AutoSupplyName"); }
        }
    }

    public class DispenseInfoCls : ViewModelBase
    {
        int index = 0;
        uint dispNo = 0;
        string type = string.Empty;
        string dispName = string.Empty;
        bool isUse = false;
        bool isUseDummy = false;
        bool isUseRecipe = false;
        bool isCheck = false;

        public FlowControlDataCls FlowControlData { get; set; } = new FlowControlDataCls();
        public PumpControlDataCls PumpControlData { get; set; }  = new PumpControlDataCls();
        public AutoSupplyControlDataCls AutoSupplyControlData { get; set; }  = new AutoSupplyControlDataCls();

        public int Index
        {
            get { return index; }
            set { index = value; RaisePropertyChanged("Index"); }
        }
        public uint DispNo
        {
            get { return dispNo; }
            set { dispNo = value; RaisePropertyChanged("DispNo"); }
        }
        public string Type
        {
            get { return type; }
            set { type = value; RaisePropertyChanged("Type"); }
        }
        public string DispName
        {
            get { return dispName; }
            set { dispName = value; RaisePropertyChanged("DispName"); }
        }
        public bool IsUse
        {
            get { return isUse; }
            set { isUse = value; RaisePropertyChanged("IsUse"); }
        }
        public bool IsUseDummy
        {
            get { return isUseDummy; }
            set { isUseDummy = value; RaisePropertyChanged("IsUseDummy"); }
        }
        public bool IsUseRecipe
        {
            get { return isUseRecipe; }
            set { isUseRecipe = value; RaisePropertyChanged("IsUseRecipe"); }
        }
        public bool IsCheck
        {
            get { return isCheck; }
            set { isCheck = value; RaisePropertyChanged("IsCheck"); }
        }

        public void Clone()
        {
            
        }
    }

    public class LoginInfoCls : ViewModelBase
    {
        string id = string.Empty;
        string pass = string.Empty;
        string loginDisplay = "Login";
        enAuthLevel authLevel = enAuthLevel.GUEST;

        public string ID
        {
            get { return id; }
            set 
            { 
                id = value;
                if (id == string.Empty) LoginDisplay = "Login";
                else LoginDisplay = "Logout";
                RaisePropertyChanged("ID"); 
            }
        }

        public string PassWord
        {
            get { return pass; }
            set { pass = value; RaisePropertyChanged("PassWord"); }
        }

        public enAuthLevel AuthLevel
        {
            get { return authLevel; }
            set { authLevel = value; RaisePropertyChanged("AuthLevel"); }
        }

        public string LoginDisplay
        {
            get { return loginDisplay; }
            set { loginDisplay = value; RaisePropertyChanged("LoginDisplay"); }
        }
    }

    public class MonitoringDataCls : ViewModelBase
    {
        int blockNo = 0;
        int moduleNo = 0;
        float initTemp = 0;
        float overTemp = 0;
        int settlingDetermTime = 0;
        int settlingTimeOut = 0;
        float rangeMin = 0;
        float rangeMax = 0;
        float pv = 0;
        float sv = 0;
        string moduleName = string.Empty;
        string measDataName = string.Empty;
        string controllerName = string.Empty;
        bool isUse = false;        

        public int BlockNo
        {
            get { return blockNo; }
            set { blockNo = value; RaisePropertyChanged("BlockNo"); }
        }
        public int ModuleNo
        {
            get { return moduleNo; }
            set { moduleNo = value; RaisePropertyChanged("ModuleNo"); }
        }
        public string ModuleName
        {
            get { return moduleName; }
            set { moduleName = value; RaisePropertyChanged("ModuleName"); }
        }
        public string MeasDataName
        {
            get { return measDataName; }
            set { measDataName = value; RaisePropertyChanged("MeasDataName"); }
        }
        public string ControllerName
        {
            get { return controllerName; }
            set { controllerName = value; RaisePropertyChanged("ControllerName"); }
        }
        public bool IsUse
        {
            get { return isUse; }
            set { isUse = value; RaisePropertyChanged("IsUse"); }
        }
        public float PV
        {
            get { return pv; }
            set { pv = value; RaisePropertyChanged("PV"); }
        }
        public float SV
        {
            get { return sv; }
            set { sv = value; RaisePropertyChanged("SV"); }
        }
        public float InitTemp
        {
            get { return initTemp; }
            set { initTemp = value; RaisePropertyChanged("InitTemp"); }
        }
        public float OverTemp
        {
            get { return overTemp; }
            set { overTemp = value; RaisePropertyChanged("OverTemp"); }
        }
        public int SettlingDetermTime
        {
            get { return settlingDetermTime; }
            set { settlingDetermTime = value; RaisePropertyChanged("SettlingDetermTime"); }
        }
        public int SettlingTimeOut
        {
            get { return settlingTimeOut; }
            set { settlingTimeOut = value; RaisePropertyChanged("SettlingTimeOut"); }
        }
        public float RangeMax
        {
            get { return rangeMax; }
            set { rangeMax = value; RaisePropertyChanged("RangeMax"); }
        }
        public float RangeMin
        {
            get { return rangeMin; }
            set { rangeMin = value; RaisePropertyChanged("RangeMin"); }
        }
    }

    public class MaintSupportCls : ViewModelBase
    {
        int BlockNo_ = 0;
        int ModuleNo_ = 0;
        int WarnLevel_ = 0;
        int LimitValue_ = 0;
        int Amount_ = 0;
        int Unit_ = 0;
        int Type_ = 0;
        bool IsAlarm_ = false;
        bool IsWatch_ = false;
        string Item_ = string.Empty;
        string Time_ = string.Empty;
        string AmountDisplay_ = string.Empty;
        string UnitDisplay_ = string.Empty;

        public int BlockNo
        {
            get { return BlockNo_; }
            set { BlockNo_ = value; RaisePropertyChanged("BlockNo"); }
        }

        public int ModuleNo
        {
            get { return ModuleNo_; }
            set { ModuleNo_ = value; RaisePropertyChanged("ModuleNo"); }
        }

        public int WarnLevel
        {
            get { return WarnLevel_; }
            set { WarnLevel_ = value; RaisePropertyChanged("WarnLevel"); }
        }

        public int LimitValue
        {
            get { return LimitValue_; }
            set { LimitValue_ = value; RaisePropertyChanged("LimitValue"); }
        }

        public int Amount
        {
            get { return Amount_; }
            set { Amount_ = value; RaisePropertyChanged("Amount"); }
        }

        public int Unit
        {
            get { return Unit_; }
            set { Unit_ = value;
                if (Type == 0) UnitDisplay = ((enMaintDate)Unit).ToString();
                else UnitDisplay = ((enMaintUnit)Unit).ToString();
                RaisePropertyChanged("Unit"); }
        }

        public int Type
        {
            get { return Type_; }
            set { Type_ = value; RaisePropertyChanged("Type"); }
        }

        public string UnitDisplay
        {
            get { return UnitDisplay_; }
            set { UnitDisplay_ = value; RaisePropertyChanged("UnitDisplay"); }
        }

        public bool IsAlarm
        {
            get { return IsAlarm_; }
            set { IsAlarm_ = value; RaisePropertyChanged("IsAlarm"); }
        }

        public bool IsWatch
        {
            get { return IsWatch_; }
            set { IsWatch_ = value; RaisePropertyChanged("IsWatch"); }
        }

        public string Item
        {
            get { return Item_; }
            set { Item_ = value; RaisePropertyChanged("Item"); }
        }

        public string Time
        {
            get { return Time_; }
            set { Time_ = value; RaisePropertyChanged("Time"); }
        }

        public string AmountDisplay
        {
            get { return AmountDisplay_; }
            set { AmountDisplay_ = value; RaisePropertyChanged("AmountDisplay"); }
        }
    }

    public class LotInfoCls : ViewModelBase
    {
        string lotID = string.Empty;
        string recipeName = string.Empty;
        string comment = string.Empty;
        int sModuleCount = 0;
        int eModuleCount = 0;
        string startDisplay = string.Empty;
        string endDisplay = string.Empty;
        ObservableCollection<int> startModuleList { get; set; } = new ObservableCollection<int>();
        ObservableCollection<int> endModuleList { get; set; } = new ObservableCollection<int>();
        bool isPilot = false;
        public LotInfoCls()
        {

        }

        public void Clear()
        {
            StartModuleList.Clear();
            EndModuleList.Clear();
            LotID = string.Empty;
            RecipeName = string.Empty;
            Comment = string.Empty;
            SModuleCount = 0;
            EModuleCount = 0;
            StartDisplay = string.Empty;
            EndDisplay = string.Empty;
            IsPilot = false;
        }

        public ObservableCollection<int> StartModuleList
        {
            get { return startModuleList; }
            set { startModuleList = value;
                StartDisplay = string.Empty;
                for(int i = 0; i < StartModuleList.Count; i++)
                {
                    StartDisplay += StartModuleList[i].ToString();
                    if (i != StartModuleList.Count - 1) StartDisplay += ",";
                }
                RaisePropertyChanged("StartModuleList"); }
        }

        public ObservableCollection<int> EndModuleList
        {
            get { return endModuleList; }
            set { endModuleList = value;
                EndDisplay = string.Empty;
                for (int i = 0; i < EndModuleList.Count; i++)
                {
                    EndDisplay += EndModuleList[i].ToString();
                    if (i != EndModuleList.Count - 1) EndDisplay += ",";
                }
                RaisePropertyChanged("EndModuleList"); }
        }

        public string StartDisplay
        {
            get { return startDisplay; }
            set { startDisplay = value; RaisePropertyChanged("StartDisplay"); }
        }

        public string EndDisplay
        {
            get { return endDisplay; }
            set { endDisplay = value; RaisePropertyChanged("EndDisplay"); }
        }

        public string LotID
        {
            get { return lotID; }
            set { lotID = value; RaisePropertyChanged("LotID"); }
        }

        public string RecipeName
        {
            get { return recipeName; }
            set { recipeName = value; RaisePropertyChanged("RecipeName"); }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; RaisePropertyChanged("Comment"); }
        }

        public int SModuleCount
        {
            get { return sModuleCount; }
            set { sModuleCount = value; RaisePropertyChanged("SModuleCount"); }
        }

        public int EModuleCount
        {
            get { return eModuleCount; }
            set { eModuleCount = value; RaisePropertyChanged("EModuleCount"); }
        }

        public bool IsPilot
        {
            get { return isPilot; }
            set { isPilot = value; RaisePropertyChanged("IsPilot"); }
        }
    }

    public class JobInfoCls : ViewModelBase
    {
        string jobName = string.Empty;
        List<LotInfoCls> lotInfoList = new List<LotInfoCls>();
        public JobInfoCls()
        {

        }

        public void Clear()
        {
            JobName = string.Empty;
            foreach(LotInfoCls lot in LotInfoList)
            {
                lot.Clear();
            }
        }

        public string JobName
        {
            get { return jobName; }
            set { jobName = value; RaisePropertyChanged("JobName"); }
        }

        public List<LotInfoCls> LotInfoList
        {
            get { return lotInfoList; }
            set { lotInfoList = value; RaisePropertyChanged("LotInfoList"); }
        }
    }

    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }    
}
