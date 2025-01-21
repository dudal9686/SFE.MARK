using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFE.TRACK
{
    public class JobDataCheckCls
    {
        public string FindDuplicates(UInt32 tagetA, UInt32 targetB)
        {
            //          UInt32 a, b;
            //         for(int i=0; i<32; i++)
            //          {
            //              targetA & Global.STDispenseIndex[i];
            //               targetB &= Global.STDispenseIndex[i]; 
            //          }
            //       }
            return "";
        }
        public bool WaferFlowCheckCls(ProcessWaferDataCls waferdata)
        {
            foreach (WaferStepCls stepData in waferdata.WaferStepList)
            {
                stepData.ModuleNo = string.Empty;
                for (int i = 0; i < stepData.ModuleCount; i++)
                {
                    stepData.ModuleNo += stepData.ModuleNoList[i].ToString();
                    if (stepData.ModuleCount - 1 != i) stepData.ModuleNo += ":";
                }

                Model.ModuleBaseCls module = Global.GetModule(stepData.BlokNo, stepData.ModuleNoList[0]);

                if (module == null || module.ModuleNo == 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of module is empty, cannot save.", stepData.Index));
                    return false;
                }

                if (stepData.RecipeName == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of recipe name is empty, cannot save.", stepData.Index));
                    return false;
                }
            }

            return true;
        }

        public bool COTProcessCheckCls(ProcessCotDataCls cotData)
        {
            uint nUnusableDisp = new uint();
            enCotDispense nArm1Disp = enCotDispense.RESIST1 | enCotDispense.RESIST2 | enCotDispense.RESIST3 | enCotDispense.RESIST4 | enCotDispense.RESIST5 | enCotDispense.RESIST6 | enCotDispense.RESIST7 | enCotDispense.RESIST8 | enCotDispense.RRC_SOLVENT | enCotDispense.RRC_SOLVENT2;
            enCotDispense nArm2Disp = enCotDispense.EBR | enCotDispense.EBR2;
            int nCnt = 0;

            if (cotData.PumpRecipe == "")
            {
                Global.MessageOpen(enMessageType.OK, string.Format("In case of pumprecipe is invalid, cannot save."));
                return false;
            }

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "COT")
                {
                    if (!disp.IsUse || !disp.IsUseRecipe) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }

            foreach (SpinChamberStepCls step in cotData.StepList)
            {
               
                if (step.DispNo > 0)
                {
                    if ((nUnusableDisp & step.DispNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }

                    if ((step.Arm1Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm1Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 1 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    if ((step.Arm2Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm2Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 2 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                }

                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm1Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm1 speed is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm2Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm2 speed is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool DEVProcessCheckCls(ProcessDevDataCls devData)
        {
            uint nUnusableDisp = new uint();
            enDevDispense nArm1Disp = enDevDispense.DEVELOP;
            enDevDispense nArm2Disp = enDevDispense.RINSE;
            int nCnt = 0;

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "DEV")
                {
                    if (!disp.IsUse || !disp.IsUseRecipe) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }

            foreach (SpinChamberStepCls step in devData.StepList)
            {
                if (step.DispNo > 0)
                {
                    if ((nUnusableDisp & step.DispNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }

                    if ((step.Arm1Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm1Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 1 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    if ((step.Arm2Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm2Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 2 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                }

                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm1Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm1 speed is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm2Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm2 speed is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool ChamberProcessCheckCls(ProcessChamberDataCls ChamberData)
        {
            foreach (ChamberStepCls step in ChamberData.StepList)
            {
                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool ADHProcessCheckCls(ProcessADHDataCls ChamberData)
        {
            uint nUnusableDisp = new uint();
            int nCnt = 0;

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "ADH")
                {
                    if (!disp.IsUse || !disp.IsUseRecipe) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }
            foreach (ADHStepCls step in ChamberData.StepList)
            {
                if (step.DispenseNo > 0)
                {
                    if ((nUnusableDisp & step.DispenseNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }
                }
                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }
       

        public bool SystemRecipeCheckCls(SystemRecipeCls systemData)
        {
            foreach (SystemRecipeStepCls step in systemData.StepList)
            {
                if (step.ModuleDisplay == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of module is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.ControlTarget == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of controltarget is empty, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool DmyCondLinkRecipeCheckCls(DummyConditionLinkCls DmyCondLinkData)
        {
            foreach (DummyConditionLinkStepCls step in DmyCondLinkData.StepList)
            {
                if (step.ModuleDisplay == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("No[{0}] : In case of module is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.RecipeName == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("No[{0}] : In case of recipename is empty, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool DmyCondRecipeCheckCls(DummyConditionCls DmyCondData)
        {
            foreach (DummyConditionStepCls step in DmyCondData.StepList)
            {
                if (step.DipsenseDisplay == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("No[{0}] : In case of dispense name is empty, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool COTDmySeqRecipeCheckCls(ProcessCotDataCls cotData)
        {
            uint nUnusableDisp = new uint();
            enCotDispense nArm1Disp = enCotDispense.RESIST1 | enCotDispense.RESIST2 | enCotDispense.RESIST3 | enCotDispense.RESIST4 | enCotDispense.RESIST5 | enCotDispense.RESIST6 | enCotDispense.RESIST7 | enCotDispense.RESIST8 | enCotDispense.RRC_SOLVENT | enCotDispense.RRC_SOLVENT2;
            enCotDispense nArm2Disp = enCotDispense.EBR | enCotDispense.EBR2;
            int nCnt = 0;

            if (cotData.PumpRecipe == "")
            {
                Global.MessageOpen(enMessageType.OK, string.Format("In case of pumprecipe is invalid, cannot save."));
                return false;
            }

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "COT")
                {
                    if (!disp.IsUse || !disp.IsUseDummy) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }

            foreach (SpinChamberStepCls step in cotData.StepList)
            {

                if (step.DispNo > 0)
                {
                    if ((nUnusableDisp & step.DispNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }
                    /*
                    if ((step.Arm1Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm1Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 1 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    if ((step.Arm2Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm2Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 2 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    */
                }

                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm1Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm1 speed is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm2Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm2 speed is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }

        public bool DEVDmySeqRecipeCheckCls(ProcessDevDataCls devData)
        {
            uint nUnusableDisp = new uint();
            enDevDispense nArm1Disp = enDevDispense.DEVELOP;
            enDevDispense nArm2Disp = enDevDispense.RINSE;
            int nCnt = 0;

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "DEV")
                {
                    if (!disp.IsUse || !disp.IsUseDummy) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }

            foreach (SpinChamberStepCls step in devData.StepList)
            {
                if (step.DispNo > 0)
                {
                    if ((nUnusableDisp & step.DispNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }
                    /*
                    if ((step.Arm1Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm1Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 1 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    if ((step.Arm2Pos == "HOME") && Convert.ToBoolean(step.DispNo & ((uint)nArm2Disp)))
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of Arm 2 moving out of cup, cannot dispense.", step.Index));
                        return false;
                    }
                    */
                }

                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm1Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm1 speed is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Arm2Speed <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of arm2 speed is invalid, cannot save.", step.Index));
                    return false;
                }
            }

            return true;
        }
        public bool ADHDmySeqRecipeCheckCls(ProcessADHDataCls ChamberData)
        {
            uint nUnusableDisp = new uint();
            int nCnt = 0;

            foreach (DispenseInfoCls disp in Global.STDispenseList)
            {
                if (disp.Type == "ADH")
                {
                    if (!disp.IsUse || !disp.IsUseDummy) nUnusableDisp += Global.STDispenseIndex[disp.DispNo - 1];
                    nCnt++;
                }
            }

            nCnt = 0;

            foreach (ADHStepCls step in ChamberData.StepList)
            {
                if (step.DispenseNo > 0)
                {
                    if ((nUnusableDisp & step.DispenseNo) != 0)
                    {
                        Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of dispense is invalid, cannot save.", step.Index));
                        return false;
                    }
                }
                if (step.Name == "")
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of name is empty, cannot save.", step.Index));
                    return false;
                }

                if (step.StepTime <= 0)
                {
                    Global.MessageOpen(enMessageType.OK, string.Format("Step[{0}] : In case of steptime is invalid, cannot save.", step.Index));
                    return false;
                }

                if (step.Loop != 0) nCnt++;
            }

            if (nCnt%2!=0)
            {
                Global.MessageOpen(enMessageType.OK, string.Format("In case of no end to the loop, cannot save."));
                return false;
            }

            return true;
        }

    }
}