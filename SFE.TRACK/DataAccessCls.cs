﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SFE.TRACK.Model;
using System.IO;

namespace SFE.TRACK
{
    public class DataAccessCls
    {
        public bool ReadModuleData()
        {
            bool isDone = true;
            DataTable dt = new DataTable();
            ModuleBaseCls moduleBase = null;
            if (Global.STAccessDB.GetModuleInfo(ref dt))
            {
                Global.STModuleList.Clear();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    enModuleType type = (enModuleType)Convert.ToInt32(dt.Rows[i]["MachineType"]);
                    string name = dt.Rows[i]["Name"].ToString();
                    if (type == enModuleType.CHAMBER)
                    {
                        if (name.IndexOf("COT") != -1 || name.IndexOf("DEV") != -1) moduleBase = new SpinChamberCls();
                        else moduleBase = new ChamberCls();
                    }
                    else if (type == enModuleType.FOUP) moduleBase = new FoupCls();
                    else if (type == enModuleType.PRA) moduleBase = new PRARobotCls();
                    else moduleBase = new CRARobotCls();

                    moduleBase.BlockNo = Convert.ToInt32(dt.Rows[i]["BlockNo"]);
                    moduleBase.ModuleNo = Convert.ToInt32(dt.Rows[i]["ModuleNo"]);
                    moduleBase.ScreenX = Convert.ToInt32(dt.Rows[i]["ScreenX"]);
                    moduleBase.ScreenY = Convert.ToInt32(dt.Rows[i]["ScreenY"]);
                    moduleBase.SizeX = Convert.ToInt32(dt.Rows[i]["SizeX"]);
                    moduleBase.SizeY = Convert.ToInt32(dt.Rows[i]["SizeY"]);
                    moduleBase.MachineName = dt.Rows[i]["Name"].ToString();
                    moduleBase.Use = Convert.ToInt32(dt.Rows[i]["Usage"]).Equals(1) ? true : false;
                    moduleBase.ModuleType = (enModuleType)Convert.ToInt32(dt.Rows[i]["MachineType"]);
                    moduleBase.MachineTitle = string.Format("{0}-{1} {2}", moduleBase.BlockNo, moduleBase.ModuleNo, moduleBase.MachineName);
                    moduleBase.MachineDesc = dt.Rows[i]["Description"].ToString();
                    moduleBase.MachineFullName = dt.Rows[i]["FullName"].ToString();
                    moduleBase.IsUsePRA = Convert.ToInt32(dt.Rows[i]["UsePRA"]).Equals(1) ? true : false;
                    moduleBase.IsUseCRA = Convert.ToInt32(dt.Rows[i]["UseCRA"]).Equals(1) ? true : false;
                    moduleBase.IsUseIRA = Convert.ToInt32(dt.Rows[i]["UseIRA"]).Equals(1) ? true : false;
                    moduleBase.MaintMode = (enMaintenanceMode)Convert.ToInt32(dt.Rows[i]["MaintMode"]);
                    moduleBase.MachineMemo = moduleBase.MachineName;
                    if (!moduleBase.Use) moduleBase.ModuleState = enModuleState.NONE;
                    else
                    {
                        if (moduleBase.ModuleState == enModuleState.NONE) moduleBase.ModuleState = enModuleState.NOTINITIAL;
                    }

                    if(moduleBase.MachineName.IndexOf("DEV") != -1 || moduleBase.MachineName.IndexOf("COT") != -1 || moduleBase.MachineName.IndexOf("ADH") != -1)
                    {
                        GetDispenseInfo(ref moduleBase);
                    }

                    Global.STModuleList.Add(moduleBase);
                }
            }
            else isDone = false;
            GetWafer();

            dt.Clear();
            dt.Dispose();

            return isDone;
        }

        private bool GetDispenseInfo(ref ModuleBaseCls module)
        {
            bool isDone = true;
            DataTable dt = new DataTable();

            if (Global.STAccessDB.GetDispenseInfo(module.BlockNo, module.ModuleNo, ref dt))
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DispenseInfoCls dispInfo = new DispenseInfoCls();
                        dispInfo.Index = i + 1;
                        dispInfo.DispNo = Convert.ToUInt32(dt.Rows[i]["DispNo"]);
                        dispInfo.DispName = dt.Rows[i]["DispName"].ToString();
                        dispInfo.IsUse = Convert.ToInt32(dt.Rows[i]["Use"]).Equals(0) ? false : true;
                        dispInfo.IsUseDummy = Convert.ToInt32(dt.Rows[i]["DummyUse"]).Equals(0) ? false : true;
                        dispInfo.IsUseRecipe = Convert.ToInt32(dt.Rows[i]["RecipeUse"]).Equals(0) ? false : true;
                        module.DispenseList.Add(dispInfo);
                    }
                }
            }
            else return false;

            return isDone;
        }

        public bool SaveDispenseInfo(int blockNo, int moduleNo)
        {
            return Global.STAccessDB.SaveDispenseInfo(blockNo, moduleNo);
        }

        private void GetWafer()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    WaferCls wafer = new WaferCls();
                    wafer.Index = j + 1;
                    wafer.BlockNo = 1;
                    wafer.ModuleNo = i + 1;
                    wafer.SizeX = 130;
                    wafer.SizeY = 15;
                    wafer.ScreenX = 40 + (i * 200);
                    wafer.ScreenY = 500 - (j * 20);
                    wafer.Diplay = string.Format("{0}-{1}", wafer.ModuleNo, wafer.Index);
                    wafer.WaferState = enWaferState.WAFER_EMPTY;

                    if(i == 0 || i == 1) wafer.WaferState = enWaferState.WAFER_EXIST;
                    Global.STWaferList.Add(wafer);
                }
            }

            DataTable dt = new DataTable();
            if(Global.STAccessDB.GetCassetteInfo(ref dt))
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    WaferCls wafer = Global.STWaferList.Find(x => x.ModuleNo == Convert.ToInt32(dt.Rows[i]["CstNo"]) && x.Index == Convert.ToInt32(dt.Rows[i]["Index"]));
                    if(wafer != null) wafer.Use = Convert.ToInt32(dt.Rows[i]["Usage"]).Equals(1) ? true : false;

                    FoupCls foup = Global.STModuleList.Find(x => x.ModuleType == enModuleType.FOUP && x.ModuleNo == wafer.ModuleNo) as FoupCls;

                    if (!foup.Use || !wafer.Use) wafer.WaferState = enWaferState.WAFER_NO_USE;
                }
            }

            List<ModuleBaseCls> moduleList = Global.STModuleList.FindAll(x => x.ModuleType == enModuleType.FOUP);            
            foreach(ModuleBaseCls module in moduleList)
            {
                FoupCls foup = module as FoupCls;
                foup.FoupWaferList = Global.STWaferList.FindAll(x => x.BlockNo == foup.BlockNo && x.ModuleNo == foup.ModuleNo);
            }
        }

        public bool SetWafer()
        {
            bool isDone = true;
            int use = 0;
            foreach(WaferCls wafer in Global.STWaferList)
            {
                use = wafer.Use.Equals(true) ? 1 : 0;
                if (!wafer.Use) wafer.WaferState = enWaferState.WAFER_NO_USE;
                else
                {
                    if (wafer.WaferState == enWaferState.WAFER_NO_USE) wafer.WaferState = enWaferState.WAFER_NONE;
                }
                if (!Global.STAccessDB.SetCassetteInfo(wafer.ModuleNo, wafer.Index, use))
                {
                    isDone = false;
                    break;
                }
            }

            return isDone;
        }

        public bool SaveModuleData()
        {
            bool isDone = true;
            foreach (ModuleBaseCls moduleBase in Global.STModuleList)
            {
                isDone = Global.STAccessDB.SetModuleInfo(moduleBase);

                if(moduleBase.ModuleType == enModuleType.FOUP)
                {
                    if(!moduleBase.Use)
                    {
                        List<WaferCls> list = Global.STWaferList.FindAll(x => x.ModuleNo == moduleBase.ModuleNo);
                        foreach (WaferCls wafer in list) wafer.WaferState = enWaferState.WAFER_NO_USE;
                    }
                    else
                    {
                        List<WaferCls> list = Global.STWaferList.FindAll(x => x.ModuleNo == moduleBase.ModuleNo);
                        foreach (WaferCls wafer in list)
                        {
                            if (wafer.WaferState == enWaferState.WAFER_NO_USE) wafer.WaferState = enWaferState.WAFER_NONE;
                        }
                    }
                }

                if (moduleBase.Use)
                {
                    if (moduleBase.ModuleState == enModuleState.NONE) moduleBase.ModuleState = enModuleState.NOTINITIAL;
                }
                else moduleBase.ModuleState = enModuleState.NONE;

                if (!isDone) break;
            }

            return isDone;
        }

        public bool ReadPrcessWaferRecipe(string filename, ref ProcessWaferDataCls waferdata)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            int index = 0;
            string line = string.Empty;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (arr[0].Trim() == "System Recipe")
                    {
                        waferdata.SystemRecipeName = arr[1].Trim();
                        continue;
                    }

                    if (arr[0].Trim() == "Process")
                    {
                        isRead = true;
                        continue;
                    }

                    if (isRead)
                    {
                        if (arr[0].Trim() == string.Empty) continue;
                        WaferStepCls stepData = new WaferStepCls();
                        stepData.Index = index + 1;
                        stepData.Name = arr[1].Trim();
                        stepData.BlokNo = Convert.ToInt32(arr[2].Trim());
                        stepData.ModuleNo = Convert.ToInt32(arr[3].Trim());
                        stepData.ModuleFunc = Convert.ToInt32(arr[4].Trim());
                        stepData.RecipeName = arr[5].Trim();
                        //stepData.RecipeNameList[1] = arr[6].Trim();
                        //stepData.RecipeNameList[2] = arr[7].Trim();
                        //stepData.RecipeNameList[3] = arr[8].Trim();
                        //stepData.RecipeNameList[4] = arr[9].Trim();
                        //stepData.RecipeNameList[5] = arr[10].Trim();
                        stepData.ModuleNoList[0] = Convert.ToInt32(arr[11].Trim());
                        stepData.ModuleNoList[1] = Convert.ToInt32(arr[12].Trim());
                        stepData.ModuleNoList[2] = Convert.ToInt32(arr[13].Trim());
                        stepData.ModuleNoList[3] = Convert.ToInt32(arr[14].Trim());
                        stepData.ModuleNoList[4] = Convert.ToInt32(arr[15].Trim());
                        stepData.ModuleNoList[5] = Convert.ToInt32(arr[16].Trim());
                        stepData.ModuleCount = Convert.ToInt32(arr[17].Trim());
                        stepData.ExtraPin = Convert.ToInt32(arr[18].Trim());

                        stepData.ModuleListDescription = string.Empty;
                        for(int i = 0; i < stepData.ModuleCount; i++)
                        {
                            stepData.ModuleListDescription += "2-" + stepData.ModuleNoList[i].ToString();
                            if (i < stepData.ModuleCount - 1) stepData.ModuleListDescription += ", ";
                        }

                        index++;

                        waferdata.WaferStepList.Add(stepData);
                    }
                }
            }
            catch (Exception ex)
            {
                isDone = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SavePrcessWaferRecipe(string filename, ProcessWaferDataCls waferdata)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("<Wafer Flow>"); sw.WriteLine();
                sw.WriteLine(string.Format("System Recipe,{0}", waferdata.SystemRecipeName));
                sw.WriteLine("Process,Name,Block No,Module No,Module Function,Rcp Name1,Rcp Name2,Rcp Name3,Rcp Name4,Rcp Name5,Rcp Name6,Module_1,Module_2,Module_3,Module_4,Module_5,Module_6,Count,ExtraPin");
                foreach (WaferStepCls stepData in waferdata.WaferStepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}",
                    stepData.Index - 1,
                    stepData.Name,
                    stepData.BlokNo,
                    stepData.ModuleNo,
                    stepData.ModuleFunc,
                    stepData.RecipeName,
                    "-",
                    "-",
                    "-",
                    "-",
                    "-",
                    stepData.ModuleNoList[0],
                    stepData.ModuleNoList[1],
                    stepData.ModuleNoList[2],
                    stepData.ModuleNoList[3],
                    stepData.ModuleNoList[4],
                    stepData.ModuleNoList[5],
                    stepData.ModuleCount,
                    stepData.ExtraPin));
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return isDone;
        }

        public bool ReadSystemRecipe(string filename, ref SystemRecipeCls systemRecipe)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            int index = 1;
            string line = string.Empty;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (arr[0].Trim().ToUpper() == "NO")
                    {
                        isRead = true;
                        continue;
                    }

                    if (isRead)
                    {
                        SystemRecipeStepCls stepData = new SystemRecipeStepCls();
                        stepData.Index = index;
                        stepData.ModuleNo = Convert.ToInt32(arr[1]);
                        stepData.ControlTarget = arr[2];
                        stepData.SetValue = Convert.ToSingle(arr[3]);
                        stepData.AlarmMaxValue = Convert.ToSingle(arr[4]);
                        stepData.AlarmMinValue = Convert.ToSingle(arr[5]);
                        stepData.StopMaxValue = Convert.ToSingle(arr[6]);
                        stepData.StopMinValue = Convert.ToSingle(arr[7]);

                        systemRecipe.StepList.Add(stepData);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveSystemRecipe(string filename, SystemRecipeCls systemRecipe)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("<System Recipe>"); sw.WriteLine();
                sw.WriteLine("No,Module,Control Target,Setting,Alarm Max,Alarm min,Stop Max,Stop Min");

                foreach (SystemRecipeStepCls stepData in systemRecipe.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                        stepData.Index,
                        stepData.ModuleNo,
                        stepData.ControlTarget,
                        stepData.SetValue,
                        stepData.AlarmMaxValue,
                        stepData.AlarmMinValue,
                        stepData.StopMaxValue,
                        stepData.StopMinValue));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadPumpRecipe(string filename, ref ProcessPumpDataCls pumpData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (arr[0].Trim().ToUpper().IndexOf("AMOUNT") != -1)
                    {
                        isRead = true;
                        continue;
                    }

                    if (isRead)
                    {
                        pumpData.DisAmount = Convert.ToSingle(arr[0]);
                        pumpData.DistRate = Convert.ToSingle(arr[1]);
                        pumpData.Acc = Convert.ToInt32(arr[2]);
                        pumpData.Dec = Convert.ToInt32(arr[3]);
                        pumpData.ReloadRate = Convert.ToSingle(arr[4]);
                        pumpData.Cal = Convert.ToSingle(arr[5]);
                        pumpData.AvCloseDelayTime = Convert.ToSingle(arr[6]);
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SavePumpRecipe(string filename, ProcessPumpDataCls pumpData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("<Pump Recipe>"); sw.WriteLine();
                sw.WriteLine("[Disp.Amount],[Disp.Rate],[Disp.Acc],[Disp.Dec],[Reload rate],[Calibration],[AV close]");
                sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    pumpData.DisAmount,
                    pumpData.DistRate,
                    pumpData.Acc,
                    pumpData.Dec,
                    pumpData.ReloadRate,
                    pumpData.Cal,
                    pumpData.AvCloseDelayTime));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return isDone;
        }

        public bool ReadProcessCotRecipe(string filename, ref ProcessCotDataCls cotData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (isRead == false)
                    {
                        if (arr[0].Trim().ToUpper().IndexOf("GAP") != -1)
                        {
                            cotData.PosEnd = Convert.ToInt32(arr[1]);
                            cotData.Pos3 = Convert.ToInt32(arr[3]);
                            cotData.Pos2 = Convert.ToInt32(arr[4]);
                            cotData.Pos1 = Convert.ToInt32(arr[5]);
                            cotData.PosBegin = Convert.ToInt32(arr[6]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("PUMP RECIPE") != -1)
                        {
                            cotData.PumpRecipe = arr[1];
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("STOP RANGE") != -1)
                        {
                            cotData.StopRange = Convert.ToInt32(arr[1]);
                            cotData.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("DISPN BLOCK") != -1)
                        {
                            cotData.DispBlock = Convert.ToInt32(arr[1]);
                            cotData.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("PROCESS") != -1)
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        SpinChamberStepCls stepData = new SpinChamberStepCls();
                        stepData.Index = index;
                        stepData.Name = arr[1];
                        stepData.StepTime = Convert.ToSingle(arr[2]);
                        stepData.SpinSpeed = Convert.ToInt32(arr[3]);
                        stepData.SpinAcc = Convert.ToInt32(arr[4]);
                        stepData.DispNo = Convert.ToUInt32(arr[6]);
                        stepData.Arm1Pos = arr[7];
                        stepData.Arm1Speed = Convert.ToInt32(arr[8]);
                        stepData.IsArm1MoveWait = Convert.ToInt32(arr[9]).Equals(1) ? true : false;
                        stepData.Arm2Pos = arr[10];
                        stepData.Arm2Speed = Convert.ToInt32(arr[11]);
                        stepData.IsArm2MoveWait = Convert.ToInt32(arr[12]).Equals(1) ? true : false;
                        cotData.StepList.Add(stepData);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessCotRecipe(string filename, ProcessCotDataCls cotData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);

                sw.WriteLine("< Coater Flow >");
                sw.WriteLine("[POS],END,CENTER,POS3,POS2,POS1,BEGIN,HOME,,GAP From < WAFER CENTER > to < END/POS3/POS2/POS1/BEGIN >,,,,,,,");
                sw.WriteLine(string.Format("Gap,{0},FIX,{1},{2},{3},{4},FIX,,,,,,,,,", cotData.PosEnd, cotData.Pos3, cotData.Pos2, cotData.Pos1, cotData.PosBegin));
                sw.WriteLine(string.Format("Pump Recipe,{0},", cotData.PumpRecipe));
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1},", cotData.StopRange, cotData.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1},", cotData.DispBlock, cotData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5,6,7,8,9,10,11");
                sw.WriteLine(",[string],[sec],[rpm],[rmp/s],,[1 / 0],[pos],[mm/s],[1 / 0],[pos],[mm/s],[1 / 0] ");
                sw.WriteLine("Process,Name,Time [sec],Spd [rmp],Accel,Spare,Dispense No,DEV Pos,DEV Spd,DEV Wait,RNS Pos,RNS Spd,RNS Wait");

                foreach (SpinChamberStepCls step in cotData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.SpinSpeed,
                        step.SpinAcc,
                        "X",
                        step.DispNo,
                        step.Arm1Pos,
                        step.Arm1Speed,
                        step.IsArm1MoveWait.Equals(true) ? 1 : 0,
                        step.Arm2Pos,
                        step.Arm2Speed,
                        step.IsArm2MoveWait.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessDevRecipe(string filename, ref ProcessDevDataCls devData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (isRead == false)
                    {
                        if (arr[0].Trim().ToUpper().IndexOf("GAP") != -1)
                        {
                            devData.PosEnd = Convert.ToInt32(arr[1]);
                            devData.Pos3 = Convert.ToInt32(arr[3]);
                            devData.Pos2 = Convert.ToInt32(arr[4]);
                            devData.Pos1 = Convert.ToInt32(arr[5]);
                            devData.PosBegin = Convert.ToInt32(arr[6]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("STOP RANGE") != -1)
                        {
                            devData.StopRange = Convert.ToInt32(arr[1]);
                            devData.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("DISPN BLOCK") != -1)
                        {
                            devData.DispBlock = Convert.ToInt32(arr[1]);
                            devData.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper().IndexOf("PROCESS") != -1)
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        SpinChamberStepCls stepData = new SpinChamberStepCls();
                        stepData.Index = index;
                        stepData.Name = arr[1];
                        stepData.StepTime = Convert.ToSingle(arr[2]);
                        stepData.SpinSpeed = Convert.ToInt32(arr[3]);
                        stepData.SpinAcc = Convert.ToInt32(arr[4]);
                        stepData.DispNo = Convert.ToUInt32(arr[5]);
                        stepData.Arm1Pos = arr[6];
                        stepData.Arm1Speed = Convert.ToInt32(arr[7]);
                        stepData.IsArm1MoveWait = Convert.ToInt32(arr[8]).Equals(1) ? true : false;
                        stepData.Arm2Pos = arr[9];
                        stepData.Arm2Speed = Convert.ToInt32(arr[10]);
                        stepData.IsArm2MoveWait = Convert.ToInt32(arr[11]).Equals(1) ? true : false;
                        devData.StepList.Add(stepData);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
            return isDone;
        }

        public bool SaveProcessDevRecipe(string filename, ProcessDevDataCls devData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);

                sw.WriteLine("< Develop Flow >");
                sw.WriteLine("[POS],END,CENTER,POS3,POS2,POS1,BEGIN,HOME,,GAP From < WAFER CENTER > to < END/POS3/POS2/POS1/BEGIN >,,,,,,,");
                sw.WriteLine(string.Format("Gap,{0},FIX,{1},{2},{3},{4},FIX,,,,,,,,,", devData.PosEnd, devData.Pos3, devData.Pos2, devData.Pos1, devData.PosBegin));
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1},", devData.StopRange, devData.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1},", devData.DispBlock, devData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5,6,7,8,9,10,11,");
                sw.WriteLine(",[string],[sec],[rpm],[rmp/s],,[1 / 0],[pos],[mm/s],[1 / 0],[pos],[mm/s],[1 / 0]");
                sw.WriteLine("Process,Name,Time [sec],Spd [rmp],Accel,Spare,Nz0,Nz Pos,Nz Spd,COT Wait,RNS Pos,RNS Spd,RNS Wait");

                foreach (SpinChamberStepCls step in devData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.SpinSpeed,
                        step.SpinAcc,
                        step.DispNo,
                        step.Arm1Pos,
                        step.Arm1Speed,
                        step.IsArm1MoveWait.Equals(true) ? 1 : 0,
                        step.Arm2Pos,
                        step.Arm2Speed,
                        step.IsArm2MoveWait.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessADHRecipe(string filename, ref ProcessADHDataCls adhData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().IndexOf("[Data]") != -1)
                        {
                            adhData.SetValue = Convert.ToSingle(arr[1]);
                            adhData.AlarmMaxValue = Convert.ToSingle(arr[2]);
                            adhData.AlarmMinValue = Convert.ToSingle(arr[2]);
                            adhData.StopMaxValue = Convert.ToSingle(arr[2]);
                            adhData.StopMinValue = Convert.ToSingle(arr[2]);
                        }
                        else if (arr[0].Trim().IndexOf("Dispn Block") != -1)
                        {
                            adhData.DispBlock = Convert.ToInt32(arr[1]);
                            adhData.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().IndexOf("Process") != -1)
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        ADHStepCls step = new ADHStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.StepTime = Convert.ToSingle(arr[2]);
                        step.DispenseNo = Convert.ToUInt32(arr[3]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        index++;
                        adhData.StepList.Add(step);

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessADHRecipe(string filename, ProcessADHDataCls adhData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< ADH Flow >");
                sw.WriteLine(",Setting,Alarm Max,Alarm Min,Stop Max,Stop Min");
                sw.WriteLine(string.Format("[Data],{0},{1},{2},{3},{4}", adhData.SetValue, adhData.AlarmMaxValue, adhData.AlarmMinValue, adhData.StopMaxValue, adhData.StopMinValue));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", adhData.DispBlock, adhData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,");
                sw.WriteLine(",[string],[sec],,[1 / 0],");
                sw.WriteLine("Process,Name,Time [sec],Dispense No,Pin Position,");

                foreach (ADHStepCls step in adhData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.DispenseNo,
                        step.IsPinPos.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessLHPRecipe(string filename, ref ProcessChamberDataCls lhpData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if(!isRead)
                    {
                        if (arr[0].Trim() == "[Data]")
                        {
                            lhpData.SetValue = Convert.ToSingle(arr[1]);
                            lhpData.AlarmMaxValue = Convert.ToSingle(arr[2]);
                            lhpData.AlarmMinValue = Convert.ToSingle(arr[3]);
                            lhpData.StopMaxValue = Convert.ToSingle(arr[4]);
                            lhpData.StopMinValue = Convert.ToSingle(arr[5]);
                        }
                        else if (arr[0].Trim() == "Process")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        ChamberStepCls step = new ChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.StepTime = Convert.ToSingle(arr[2]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        step.IsShutter = Convert.ToInt32(arr[5]).Equals(1) ? true : false;
                        lhpData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessLHPRecipe(string filename, ProcessChamberDataCls LhpData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< LHP Process Flow >");
                sw.WriteLine(",Setting,Alarm Max,Alarm Min,Stop Max,Stop Min");
                sw.WriteLine(string.Format("[Data],{0},{1},{2},{3},{4}", LhpData.SetValue, LhpData.AlarmMaxValue, LhpData.AlarmMinValue, LhpData.StopMaxValue, LhpData.StopMinValue));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4");
                sw.WriteLine(",[string],[sec],,[1 / 0],[1 / 0]");
                sw.WriteLine("Process,Name,Time [sec],Spare,Pin Position,Shutter Status");

                foreach (ChamberStepCls step in LhpData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},X,{3},{4}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.IsPinPos.Equals(true) ? 1 : 0,
                        step.IsShutter.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessHHPRecipe(string filename, ref ProcessChamberDataCls HhpData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim() == "[Data]")
                        {
                            HhpData.SetValue = Convert.ToSingle(arr[1]);
                            HhpData.AlarmMaxValue = Convert.ToSingle(arr[2]);
                            HhpData.AlarmMinValue = Convert.ToSingle(arr[3]);
                            HhpData.StopMaxValue = Convert.ToSingle(arr[4]);
                            HhpData.StopMinValue = Convert.ToSingle(arr[5]);
                        }
                        else if (arr[0].Trim() == "Process")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        ChamberStepCls step = new ChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.StepTime = Convert.ToSingle(arr[2]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        step.IsShutter = Convert.ToInt32(arr[5]).Equals(1) ? true : false;
                        HhpData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessHHPRecipe(string filename, ProcessChamberDataCls HhpData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< HHP Process Flow >");
                sw.WriteLine(",Setting,Alarm Max,Alarm Min,Stop Max,Stop Min");
                sw.WriteLine(string.Format("[Data],{0},{1},{2},{3},{4}", HhpData.SetValue, HhpData.AlarmMaxValue, HhpData.AlarmMinValue, HhpData.StopMaxValue, HhpData.StopMinValue));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4");
                sw.WriteLine(",[string],[sec],,[1 / 0],[1 / 0]");
                sw.WriteLine("Process,Name,Time [sec],Spare,Pin Position,Shutter Status");

                foreach (ChamberStepCls step in HhpData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},X,{3},{4}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.IsPinPos.Equals(true) ? 1 : 0,
                        step.IsShutter.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessCPLRecipe(string filename, ref ProcessChamberDataCls CplData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim() == "[Data]")
                        {
                            CplData.SetValue = Convert.ToSingle(arr[1]);
                            CplData.AlarmMaxValue = Convert.ToSingle(arr[2]);
                            CplData.AlarmMinValue = Convert.ToSingle(arr[3]);
                            CplData.StopMaxValue = Convert.ToSingle(arr[4]);
                            CplData.StopMinValue = Convert.ToSingle(arr[5]);
                        }
                        else if (arr[0].Trim() == "Process")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        ChamberStepCls step = new ChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.StepTime = Convert.ToSingle(arr[2]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        CplData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessCPLRecipe(string filename, ProcessChamberDataCls CplData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< CPL Process Flow >");
                sw.WriteLine(",Setting,Alarm Max,Alarm Min,Stop Max,Stop Min");
                sw.WriteLine(string.Format("[Data],{0},{1},{2},{3},{4}", CplData.SetValue, CplData.AlarmMaxValue, CplData.AlarmMinValue, CplData.StopMaxValue, CplData.StopMinValue));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3");
                sw.WriteLine(",[string],[sec],,[1 / 0]");
                sw.WriteLine("Process,Name,Time [sec],Spare,Pin Position");

                foreach (ChamberStepCls step in CplData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},X,{3}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.IsPinPos.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadProcessTCPRecipe(string filename, ref ProcessChamberDataCls TcpData)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim() == "[Data]")
                        {
                            TcpData.SetValue = Convert.ToSingle(arr[1]);
                            TcpData.AlarmMaxValue = Convert.ToSingle(arr[2]);
                            TcpData.AlarmMinValue = Convert.ToSingle(arr[3]);
                            TcpData.StopMaxValue = Convert.ToSingle(arr[4]);
                            TcpData.StopMinValue = Convert.ToSingle(arr[5]);
                        }
                        else if (arr[0].Trim() == "Process")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        ChamberStepCls step = new ChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.StepTime = Convert.ToSingle(arr[2]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        TcpData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveProcessTCPRecipe(string filename, ProcessChamberDataCls TcpData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< CPL Process Flow >");
                sw.WriteLine(",Setting,Alarm Max,Alarm Min,Stop Max,Stop Min");
                sw.WriteLine(string.Format("[Data],{0},{1},{2},{3},{4}", TcpData.SetValue, TcpData.AlarmMaxValue, TcpData.AlarmMinValue, TcpData.StopMaxValue, TcpData.StopMinValue));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3");
                sw.WriteLine(",[string],[sec],,[1 / 0]");
                sw.WriteLine("Process,Name,Time [sec],Spare,Pin Position");

                foreach (ChamberStepCls step in TcpData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},X,{3}",
                        step.Index,
                        step.Name,
                        step.StepTime,
                        step.IsPinPos.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadCleanCOTRecipe(string filename, ref CleanDataCls cleanData)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim() == "[Pump Recipe]")
                        {
                            cleanData.PumpRecipe = arr[1];
                        }
                        else if (arr[0].Trim() == "Stop Range")
                        {
                            cleanData.StopRange = Convert.ToInt32(arr[1]);
                            cleanData.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim() == "Dispn Block")
                        {
                            cleanData.DispBlock = Convert.ToInt32(arr[1]);
                            cleanData.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim() == "Step")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        CleanStepCls step = new CleanStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.Loop = Convert.ToInt32(arr[2]);
                        step.StepTime = Convert.ToSingle(arr[3]);
                        step.SpinSpeed = Convert.ToInt32(arr[4]);
                        step.SpinAcc = Convert.ToInt32(arr[5]);
                        step.DispenseNo = Convert.ToUInt32(arr[6]);
                        cleanData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveCleanCOTRecipe(string filename, CleanDataCls cleanData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Coater Cleaning Recipe >");
                sw.WriteLine(string.Format("Pump Recipe,{0}", cleanData.PumpRecipe));
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1}", cleanData.StopRange, cleanData.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", cleanData.DispBlock, cleanData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5");
                sw.WriteLine("[int],[string],[int],[float],[int],[int],[int]");
                sw.WriteLine("Step,Name,Loop, Time, Spin Speed,Spin Accel,Dispense No");

                foreach (CleanStepCls step in cleanData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                        step.Index,
                        step.Name,
                        step.Loop,
                        step.StepTime,
                        step.SpinSpeed,
                        step.SpinAcc,
                        step.DispenseNo));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadCleanDEVRecipe(string filename, ref CleanDataCls cleanData)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim() == "Stop Range")
                        {
                            cleanData.StopRange = Convert.ToInt32(arr[1]);
                            cleanData.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim() == "Dispn Block")
                        {
                            cleanData.DispBlock = Convert.ToInt32(arr[1]);
                            cleanData.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim() == "Step")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        CleanStepCls step = new CleanStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.Loop = Convert.ToInt32(arr[2]);
                        step.StepTime = Convert.ToSingle(arr[3]);
                        step.SpinSpeed = Convert.ToInt32(arr[4]);
                        step.SpinAcc = Convert.ToInt32(arr[5]);
                        step.DispenseNo = Convert.ToUInt32(arr[6]);
                        cleanData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveCleanDEVRecipe(string filename, CleanDataCls cleanData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Dev Cleaning Recipe >");
                sw.WriteLine(string.Format("Pump Recipe,{0}", cleanData.PumpRecipe));
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1}", cleanData.StopRange, cleanData.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", cleanData.DispBlock, cleanData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5");
                sw.WriteLine("[int],[string],[int],[float],[int],[int],[int]");
                sw.WriteLine("Step,Name,Loop, Time, Spin Speed,Spin Accel,Dispense No");

                foreach (CleanStepCls step in cleanData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                        step.Index,
                        step.Name,
                        step.Loop,
                        step.StepTime,
                        step.SpinSpeed,
                        step.SpinAcc,
                        step.DispenseNo));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadCleanCondRecipe(string filename, ref CleanCondDataCls cleanData)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        CleanCondStepCls step = new CleanCondStepCls();
                        step.Index = index;
                        step.ModuleNo = Convert.ToInt32(arr[1]);
                        step.JigModuleNo = Convert.ToInt32(arr[2]);
                        step.Cnt = Convert.ToInt32(arr[3]);
                        step.Interval = Convert.ToInt32(arr[4]);
                        step.IsCond = Convert.ToInt32(arr[5]).Equals(1) ? true : false;
                        step.RecipeName = arr[6];
                        cleanData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveCleanCondRecipe(string filename, CleanCondDataCls cleanData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Cleaning Condition Recipe >");
                sw.WriteLine();                
                sw.WriteLine("No,0,1,2,3,4,5");
                sw.WriteLine("[int],[string],[int],[float],[int],[int],[int]");
                sw.WriteLine("Step,Cleaning Module,Jig Module,Wafer Count,Interval,Cond,Cleaning Recipe");

                foreach (CleanCondStepCls step in cleanData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6}",
                        step.Index,
                        step.ModuleNo,
                        step.JigModuleNo,
                        step.Cnt,
                        step.Interval,
                        step.IsCond.Equals(true) ? 1 : 0,
                        step.RecipeName));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadDummyCondRecipe(string filename, ref DummyConditionCls dummyData)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                        else if (arr[0].Trim().ToUpper() == "PUMP RECIPE")
                        {
                            dummyData.PumpRecipe = arr[1];
                        }
                        else if(arr[0].Trim().ToUpper() == "DISPN BLOCK")
                        {
                            dummyData.DispBlock = Convert.ToInt32(arr[1]);
                            dummyData.DispModule = Convert.ToInt32(arr[3]);
                        }
                    }
                    else
                    {
                        DummyConditionStepCls step = new DummyConditionStepCls();
                        step.Index = index;
                        step.DispenseNo = Convert.ToUInt32(arr[1]);
                        step.WaferCnt = Convert.ToInt32(arr[2]);
                        step.Interval = Convert.ToInt32(arr[3]);
                        step.Lotspec = Convert.ToInt32(arr[4]);
                        step.IsCond = Convert.ToInt32(arr[5]).Equals(1) ? true : false;
                        step.Timing = Convert.ToInt32(arr[6]);
                        step.IsRecipeUse = Convert.ToInt32(arr[7]).Equals(1) ? true : false;
                        step.RecipeCnt = Convert.ToInt32(arr[8]);
                        step.RecipeInterval = Convert.ToInt32(arr[9]);
                        step.RecipeTime = Convert.ToInt32(arr[10]);
                        step.Recipe = arr[11];
                        dummyData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveDummyCondRecipe(string filename, DummyConditionCls dummyData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Dummy Condition >");
                sw.WriteLine(string.Format("Pump Recipe,{0}", dummyData.PumpRecipe));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", dummyData.DispBlock, dummyData.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5,6,7,8,9,10");
                sw.WriteLine("[int],[int],[int],[sec],[string],[1 / 0],[string],[1 / 0],[int],[int],[int],[string]");
                sw.WriteLine("Step,Dispense No,Wafer Cnt,Interval,Lot Spec,Cond, Timing, Recipe Use,Recipe Cnt,Recipe Interval,Recipe Time,Recipe name");

                foreach (DummyConditionStepCls step in dummyData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                        step.Index,
                        step.DispenseNo,
                        step.WaferCnt,
                        step.Interval,
                        step.Lotspec,
                        step.IsCond.Equals(true) ? 1 : 0,
                        step.Timing,
                        step.IsRecipeUse.Equals(true) ? 1 : 0,
                        step.RecipeCnt,
                        step.RecipeInterval,
                        step.RecipeTime,
                        step.Recipe));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadDummyCondLinkRecipe(string filename, ref DummyConditionLinkCls dummyData)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                    }
                    else
                    {
                        DummyConditionLinkStepCls step = new DummyConditionLinkStepCls();
                        step.Index = index;
                        step.ModuleNo = Convert.ToInt32(arr[1]);
                        step.RecipeName = arr[2];
                        dummyData.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveDummyCondLinkRecipe(string filename, DummyConditionLinkCls dummyData)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Dummy Condition Link >");
                sw.WriteLine("No,0,1");
                sw.WriteLine("[int],[int],[string]");
                sw.WriteLine("Step,Dispense No,Recipe name");

                foreach (DummyConditionLinkStepCls step in dummyData.StepList)
                {
                    sw.WriteLine(string.Format("{0},{1},{2}",
                        step.Index,
                        step.ModuleNo,
                        step.RecipeName));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadDummySeqADHRecipe(string filename, ref ProcessADHDataCls data)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                        else if (arr[0].Trim().ToUpper() == "DISPN BLOCK")
                        {
                            data.DispBlock = Convert.ToInt32(arr[1]);
                            data.DispModule = Convert.ToInt32(arr[3]);
                        }
                    }
                    else
                    {
                        ADHStepCls step = new ADHStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.Loop = Convert.ToInt32(arr[2]);
                        step.StepTime = Convert.ToSingle(arr[3]);
                        step.IsPinPos = Convert.ToInt32(arr[4]).Equals(1) ? true : false;
                        step.DispenseNo = Convert.ToUInt32(arr[5]);
                        data.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveDummySeqADHRecipe(string filename, ProcessADHDataCls data)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< ADH Dummy Seq Flow >");
                sw.WriteLine();
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", data.DispBlock, data.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4");
                sw.WriteLine("[int],[string],[int],[sec],[1 / 0],[string]");
                sw.WriteLine("Step,Name,Loop,Time,Pin Pos,Dispense No");

                foreach (ADHStepCls step in data.StepList)
                {
                    if (step.Name == string.Empty) continue;
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}",
                        step.Index,
                        step.Name,
                        step.Loop,
                        step.StepTime,
                        step.IsPinPos.Equals(true) ? 1 : 0,
                        step.DispenseNo));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadDummySeqCOTRecipe(string filename, ref ProcessCotDataCls data)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                        else if (arr[0].Trim().ToUpper() == "DISPN BLOCK")
                        {
                            data.DispBlock = Convert.ToInt32(arr[1]);
                            data.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper() == "STOP RANGE")
                        {
                            data.StopRange = Convert.ToInt32(arr[1]);
                            data.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper() == "PUMP RECIPE")
                        {
                            data.PumpRecipe = arr[1];
                        }
                    }
                    else
                    {
                        SpinChamberStepCls step = new SpinChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.Loop = Convert.ToInt32(arr[2]);
                        step.StepTime = Convert.ToSingle(arr[3]);
                        step.DispNo = Convert.ToUInt32(arr[4]);
                        step.Arm1Pos = arr[5];
                        step.Arm1Speed = Convert.ToInt32(arr[6]);
                        step.IsArm1MoveWait = Convert.ToInt32(arr[7]).Equals(1) ? true : false;
                        step.Arm2Pos = arr[8];
                        step.Arm2Speed = Convert.ToInt32(arr[9]);
                        step.IsArm2MoveWait = Convert.ToInt32(arr[10]).Equals(1) ? true : false;
                        data.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveDummySeqCOTRecipe(string filename, ProcessCotDataCls data)
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Coater Dummy Seq Flow >");
                sw.WriteLine();
                sw.WriteLine(string.Format("Pump Recipe,{0}", data.PumpRecipe));
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1}", data.StopRange, data.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", data.DispBlock, data.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5,6,7,8,9");
                sw.WriteLine("[int],[string],[1 / 0],[sec],[1 / 0],[pos],[mm/s],[1 / 0],[pos],[mm/s],[1 / 0]");
                sw.WriteLine("Step,Name,Loop,Time,Dispense No,ARM1 Pos,ARM1 Spd,ARM1 Wait,ARM2 Pos,ARM2 Spd,ARM2 Wait");

                foreach (SpinChamberStepCls step in data.StepList)
                {
                    if (step.Name == string.Empty) continue;
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        step.Index,
                        step.Name,
                        step.Loop,
                        step.StepTime,
                        step.DispNo,
                        step.Arm1Pos,
                        step.Arm1Speed,
                        step.IsArm1MoveWait.Equals(true) ? 1 : 0,
                        step.Arm2Pos,
                        step.Arm2Speed,
                        step.IsArm2MoveWait.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadDummySeqDEVRecipe(string filename, ref ProcessDevDataCls data)
        {
            bool isDone = false;

            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            int index = 1;
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    string[] arr = line.Split(',');

                    if (!isRead)
                    {
                        if (arr[0].Trim().ToUpper() == "STEP")
                        {
                            isRead = true;
                            continue;
                        }
                        else if (arr[0].Trim().ToUpper() == "DISPN BLOCK")
                        {
                            data.DispBlock = Convert.ToInt32(arr[1]);
                            data.DispModule = Convert.ToInt32(arr[3]);
                        }
                        else if (arr[0].Trim().ToUpper() == "STOP RANGE")
                        {
                            data.StopRange = Convert.ToInt32(arr[1]);
                            data.AlarmRange = Convert.ToInt32(arr[3]);
                        }
                    }
                    else
                    {
                        SpinChamberStepCls step = new SpinChamberStepCls();
                        step.Index = index;
                        step.Name = arr[1];
                        step.Loop = Convert.ToInt32(arr[2]);
                        step.StepTime = Convert.ToSingle(arr[3]);
                        step.DispNo = Convert.ToUInt32(arr[4]);
                        step.Arm1Pos = arr[5];
                        step.Arm1Speed = Convert.ToInt32(arr[6]);
                        step.IsArm1MoveWait = Convert.ToInt32(arr[7]).Equals(1) ? true : false;
                        step.Arm2Pos = arr[8];
                        step.Arm2Speed = Convert.ToInt32(arr[9]);
                        step.IsArm2MoveWait = Convert.ToInt32(arr[10]).Equals(1) ? true : false;
                        data.StepList.Add(step);
                        index++;
                    }
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveDummySeqDEVRecipe(string filename, ProcessDevDataCls data) 
        {
            bool isDone = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine("< Developer Dummy Seq Flow >");
                sw.WriteLine();
                sw.WriteLine(string.Format("Stop Range,{0},Alarm Range,{1}", data.StopRange, data.AlarmRange));
                sw.WriteLine(string.Format("Dispn Block,{0},Dispn Module,{1}", data.DispBlock, data.DispModule));
                sw.WriteLine();
                sw.WriteLine("No,0,1,2,3,4,5,6,7,8,9");
                sw.WriteLine("[int],[string],[1 / 0],[sec],[1 / 0],[pos],[mm/s],[1 / 0],[pos],[mm/s],[1 / 0]");
                sw.WriteLine("Step,Name,Loop,Time,Dispense No,ARM1 Pos,ARM1 Spd,ARM1 Wait,ARM2 Pos,ARM2 Spd,ARM2 Wait");

                foreach (SpinChamberStepCls step in data.StepList)
                {
                    if (step.Name == string.Empty) continue;
                    sw.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        step.Index,
                        step.Name,
                        step.Loop,
                        step.StepTime,
                        step.DispNo,
                        step.Arm1Pos,
                        step.Arm1Speed,
                        step.IsArm1MoveWait.Equals(true) ? 1 : 0,
                        step.Arm2Pos,
                        step.Arm2Speed,
                        step.IsArm2MoveWait.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }
            return isDone;
        }

        public bool ReadUserInfo()
        {
            bool isDone = true;
            Global.STUserList.Clear();
            DataTable dt = new DataTable();

            if(Global.STAccessDB.GetUserInfo(ref dt))
            {
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    LoginInfoCls info = new LoginInfoCls();
                    info.ID = dt.Rows[i]["ID"].ToString();
                    info.PassWord = dt.Rows[i]["PassWord"].ToString();
                    info.AuthLevel = (enAuthLevel)Convert.ToInt32(dt.Rows[i]["Auth"]);
                    Global.STUserList.Add(info);
                }                
            }

            dt.Clear();
            dt.Dispose();
            return isDone;
        }

        public bool CreateUserInfo(string id, string pass, int auth)
        {
            return Global.STAccessDB.CreateUserInfo(id, pass, auth); 
        }

        public bool ModifyUserInfo(string id, string pass, int auth)
        {
            return Global.STAccessDB.ModifyUserInfo(id, pass, auth);
        }

        public bool DeleteUserInfo(string id)
        {
            return Global.STAccessDB.DeleteUserInfo(id);
        }

        public bool ReadMaintSupportData()
        {
            Global.STMaintSupportList.Clear();
            DataTable dt = new DataTable();

            if (Global.STAccessDB.GetMaintSupport(ref dt))
            {
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        MaintSupportCls maint = new MaintSupportCls();
                        maint.BlockNo = Convert.ToInt32(dt.Rows[i]["BlockNo"]);
                        maint.ModuleNo = Convert.ToInt32(dt.Rows[i]["ModuleNo"]);
                        maint.Item = dt.Rows[i]["Item"].ToString();
                        maint.IsWatch = Convert.ToInt32(dt.Rows[i]["Watch"]).Equals(1) ? true : false;
                        maint.WarnLevel = Convert.ToInt32(dt.Rows[i]["WarningLevel"]);
                        maint.LimitValue = Convert.ToInt32(dt.Rows[i]["LimitValue"]);
                        maint.Amount = Convert.ToInt32(dt.Rows[i]["Amount"]);
                        maint.Type = Convert.ToInt32(dt.Rows[i]["Type"]);
                        maint.Unit = Convert.ToInt32(dt.Rows[i]["Unit"]);
                        maint.Time = dt.Rows[i]["Time"].ToString();
                        maint.IsAlarm = Convert.ToInt32(dt.Rows[i]["Alarm"]).Equals(1) ? true : false;

                        if (maint.Type == 0) maint.UnitDisplay = ((enMaintDate)maint.Unit).ToString();
                        else maint.UnitDisplay = ((enMaintUnit)maint.Unit).ToString();

                        Global.STMaintSupportList.Add(maint);
                    }
                }
            }
            else return false;

            return true;
        }

        public bool SaveMaintSupportData()
        {
            bool isDone = true;
            foreach(MaintSupportCls maint in Global.STMaintSupportList)
            {
                if(!Global.STAccessDB.SetMaintSupport(maint))
                {
                    isDone = false;
                    break;
                }
            }

            return isDone;
        }

        public bool ReadJobInfoData(string filename)
        {
            bool isDone = true;
            FileInfo fileInfo = new FileInfo(filename);
            if (!fileInfo.Exists) return false;

            bool isRead = false;
            string line = string.Empty;
            StreamReader sr = null;
            int index = 0;

            try
            {
                sr = new StreamReader(filename);
                while(!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line == string.Empty) continue;
                    string[] arr = line.Split(',');
                    
                    if(arr[0] == "JobName")
                    {
                        Global.STJobInfo.JobName = arr[1];
                    }
                    else if(arr[0] == "Index")
                    {
                        isRead = true;
                        continue;
                    }

                    if(isRead)
                    {
                        Global.STJobInfo.LotInfoList[index].LotID = arr[1];
                        Global.STJobInfo.LotInfoList[index].RecipeName = arr[2];
                        Global.STJobInfo.LotInfoList[index].Comment = arr[3];
                        Global.STJobInfo.LotInfoList[index].SModuleCount = Convert.ToInt32(arr[4]);
                        Global.STJobInfo.LotInfoList[index].EModuleCount = Convert.ToInt32(arr[5]);

                        string[] arrModule;
                        if(Global.STJobInfo.LotInfoList[index].SModuleCount > 0)
                        {
                            arrModule = arr[6].Split('-');
                            for (int i = 0; i < arrModule.Length; i++)
                            {
                                if (arrModule[i] == string.Empty) continue;
                                Global.STJobInfo.LotInfoList[index].StartModuleList.Add(Convert.ToInt32(arrModule[i]));
                                Global.STJobInfo.LotInfoList[index].StartDisplay += arrModule[i];
                                if (arrModule.Length != i+1)
                                    Global.STJobInfo.LotInfoList[index].StartDisplay += ",";
                            }
                        }

                        if (Global.STJobInfo.LotInfoList[index].EModuleCount > 0)
                        {
                            arrModule = arr[7].Split('-');
                            for (int i = 0; i < arrModule.Length; i++)
                            {
                                if (arrModule[i] == string.Empty) continue;
                                Global.STJobInfo.LotInfoList[index].EndModuleList.Add(Convert.ToInt32(arrModule[i]));
                                Global.STJobInfo.LotInfoList[index].EndDisplay += arrModule[i];
                                if (arrModule.Length != i+1)
                                    Global.STJobInfo.LotInfoList[index].EndDisplay += ",";
                            }
                        }

                        Global.STJobInfo.LotInfoList[index].IsPilot = arr[8].Equals("1") ? true : false;

                        index++;
                    }                    
                }
            }
            catch (Exception e)
            {
                isDone = false;
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }

            return isDone;
        }

        public bool SaveJobInfo(string filename)
        {
            bool isDone = true;
            int index = 0;
            string temp = string.Empty;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetDirectoryName(filename));
            if (!directoryInfo.Exists) directoryInfo.Create();

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(filename);
                sw.WriteLine(string.Format("JobName,{0}", Global.STJobInfo.JobName));
                sw.WriteLine("Index,LotID,RecipeName,Comment,SCount,ECount,SModule,EModule,Pilot");
                for (int i = 0; i < Global.STJobInfo.LotInfoList.Count; i++)
                {
                    LotInfoCls lot = Global.STJobInfo.LotInfoList[i];
                    if (lot.LotID == string.Empty || lot.RecipeName == string.Empty || lot.StartModuleList.Count == 0) continue;
                    sw.Write(string.Format("{0},{1},{2},{3},{4},{5},", index, lot.LotID, lot.RecipeName, lot.Comment, lot.StartModuleList.Count, lot.EndModuleList.Count));

                    temp = string.Empty;
                    for(int j = 0; j < lot.StartModuleList.Count; j++)
                    {
                        temp += lot.StartModuleList[j].ToString();
                        if (j != lot.StartModuleList.Count - 1) temp += "-";
                    }

                    sw.Write(temp + ",");

                    temp = string.Empty;
                    for (int j = 0; j < lot.EndModuleList.Count; j++)
                    {
                        temp += lot.EndModuleList[j].ToString();
                        if (j != lot.EndModuleList.Count - 1) temp += "-";
                    }

                    sw.WriteLine(string.Format("{0},{1}", temp, lot.IsPilot.Equals(true) ? 1 : 0));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                isDone = false;
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

            return isDone;
        }
    }
}