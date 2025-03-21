using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using CoreCSRunSim;

namespace SFE.TRACK.ViewModel
{
    public class ViewModelMessage : MessageBase
    {
    }

    public class PopUpModuleTypeCls : MessageBase
    {
        public string ModuleType = string.Empty;
        public WaferStepCls waferStep = null; //WaferProcessRecipe를 위한 것
        public int BlockNo { get; set; }
        public int ModuleNo { get; set; }
        public string ControlTarget = string.Empty;

        public void Clear()
        {
            ModuleType = string.Empty;
            BlockNo = 0;
            ModuleNo = 0;
            waferStep = null;
            ControlTarget = string.Empty;
        }
    }

    public class PopUpRecipeCls : MessageBase
    {
        public enRecipeMenu RecipeMenu = enRecipeMenu.CLEAN_COND;
        public string RecipeName = string.Empty; //받은 레시피
        public string SelectRecipeName { get; set; }
    }

    public class PopUpMessageCls : MessageBase
    {
        public enMessageType MessageType = enMessageType.OK;
        public string Message = string.Empty;
    }

    public class PopUpArmPositionCls : MessageBase
    {
        public enArmTpe ArmType = enArmTpe.ARM1;
        public string ArmPosition = string.Empty;
        public string SelectArmPosition = string.Empty;
    }

    public class PopUpDispenseCls : MessageBase
    {
        public enDispenseModule ModuleType = enDispenseModule.ADH;
        public uint DispenseValue = 0;
        public uint SelectDispenseValue = 0;
        public string DummyOrRecipeUse = "DUMMY"; //DUMMYUSE, RECIPEUSE
        public bool IsMultiSelect = false;
    }

    public class SelectModuleCls : MessageBase
    {
        public int BlockNo { get; set; }
        public int ModuleNo { get; set; }
    }

    public class PopUpUserRegistCls : MessageBase
    {
        public string ID { get; set; }
        public string PassWord { get; set; }
        public int AuthLevel { get; set; }
        public int Type { get; set; }
        public PasswordBox PassWordBox { get; set; }
    }

    public class TeachModuleMessageCls : MessageBase
    {
        public int BlockNo { get; set; }
        public int ModuleNo { get; set; }
        public string Name { get; set; } 
    }

    public class MotorIOMessageCls : MessageBase
    {
        public string Title { get; set; }
        public enAxisType AxisType = enAxisType.C1_BT;
    }

    public class DateMessageCls : MessageBase
    {
        public DateTime Date { get; set; }
    }

    public class TeachingDataMessageCls : MessageBase
    {
        public double Position { get; set; }
        public Model.AxisInfoCls Axis { get; set; } 
    }
}
