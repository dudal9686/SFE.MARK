using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Threading;
using System.Data;
using System.Windows;
using SFE.TRACK.Model;
using MySql.Data.MySqlClient;

namespace SFE.TRACK.DB
{    
    public class AccessDBCls // MariaDB 로 교체
    {
        MySqlConnection dbConnect = null;
        MySqlCommand dbCommand = null;
        MySqlDataAdapter dbAdapter = null;

        //OleDbConnection dbConnect = null;
        //OleDbCommand dbCommand = null;
        //OleDbDataAdapter dbAdapter = null;
        String connectString = string.Empty;
        Mutex sync = new Mutex();

        public AccessDBCls()
        {
            //connectString = string.Format("Provider=Microsoft.Jet.OleDb.4.0; Data Source={0}", /*System.Windows.Forms.Application.StartupPath + */@"D:\MDB\MachineDB.mdb");
            connectString = string.Format("Server=127.0.0.1;Port=3306;Database=SFE_MARK;Uid=root;Pwd=1234");
        }

        private bool Open()
        {
            try
            {
                dbConnect = new MySqlConnection(connectString);
                dbConnect.Open();
            }            
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private bool Close()
        {
            try
            {
                if (dbConnect.State == System.Data.ConnectionState.Connecting) dbConnect.Close();
                if (dbConnect != null) dbConnect.Dispose();
                if (dbCommand != null) dbCommand.Dispose();
                if (dbAdapter != null) dbAdapter.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private bool ExecuteNonQuery(String query)
        {
            sync.WaitOne();
            if (!Open())
            {
                sync.ReleaseMutex();
                return false;
            }

            try
            {
                dbCommand = new MySqlCommand(query, dbConnect);
                dbCommand.CommandTimeout = 3000;
                dbCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                sync.ReleaseMutex();
                MessageBox.Show(ex.Message);
                return false;
            }

            if (!Close())
            {
                sync.ReleaseMutex();
                return false;
            }

            sync.ReleaseMutex();
            return true;
        }

        private bool ExecuteReader(String query, ref DataTable dt)
        {
            sync.WaitOne();
            if (!Open())
            {
                sync.ReleaseMutex();
                return false;
            }
            try
            {
                dbCommand = new MySqlCommand(query, dbConnect);
                dbAdapter = new MySqlDataAdapter(dbCommand);
                dbAdapter.Fill(dt);
            }
            catch (Exception ex)
            {
                sync.ReleaseMutex();
                MessageBox.Show(ex.Message);
                return false;
            }

            if (!Close())
            {
                sync.ReleaseMutex();
                return false;
            }

            sync.ReleaseMutex();
            return true;
        }

        public bool GetModuleInfo(ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbModuleInfo");
            return ExecuteReader(query, ref dt);
        }

        public bool SetModuleInfo(ModuleBaseCls moduleBase)
        {
            string query = string.Format("UPDATE tbModuleInfo SET `Usage` = {0}, `UseCRA` = {1}, `UsePRA` = {2}, `UseIRA` = {3}  WHERE `BlockNo` = {4} AND `ModuleNo` = {5}", 
                moduleBase.Use.Equals(true) ? 1 : 0, 
                moduleBase.IsUseCRA.Equals(true) ? 1 : 0,
                moduleBase.IsUsePRA.Equals(true) ? 1 : 0,
                moduleBase.IsUseIRA.Equals(true) ? 1 : 0,
                moduleBase.BlockNo, 
                moduleBase.ModuleNo);
            return ExecuteNonQuery(query);
        }

        public bool GetDispenseInfo(int blockNo, int moduleNo, ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbDispenseInfo WHERE BlockNo = {0} AND ModuleNo = {1}", blockNo, moduleNo);
            return ExecuteReader(query, ref dt);
        }

        public bool GetDispenseInfo(ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbDispenseInfo");
            return ExecuteReader(query, ref dt);
        }

        public bool GetFlowControlData(string type, uint dispNo, ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbFlowControlData WHERE `Type` = '{0}' AND `DispIndex` = {1}", type, dispNo);
            return ExecuteReader(query, ref dt);
        }

        public bool SetFlowControlData(DispenseInfoCls dispInfo)
        {
            bool isDone = true;
            DataTable dt = new DataTable();
            string query = string.Empty;

            if(GetFlowControlData(dispInfo.Type, dispInfo.DispNo, ref dt))
            {
                if(dt.Rows.Count == 0)
                {
                    //Insert
                    query = string.Format("INSERT INTO tbFlowControlData(`Type`, `DispIndex`, `DispName`, `PulseRate`, `SamplingDelayTime`, `ReferenceValue`, `Calibration`," +
                        "`AlarmUpper`, `AlarmLower`, `StopUpper`, `StopLower`, `CheckTiming`, `FlowMonitoring`)" +
                        " VALUES('{0}',{1},'{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12})",
                        dispInfo.Type,
                        dispInfo.DispNo,
                        dispInfo.DispName,
                        dispInfo.FlowControlData.PulseRate,
                        dispInfo.FlowControlData.SamplingDelayTime,
                        dispInfo.FlowControlData.ReferenceValue,
                        dispInfo.FlowControlData.Calibration,
                        dispInfo.FlowControlData.AlarmUpper,
                        dispInfo.FlowControlData.AlarmLower,
                        dispInfo.FlowControlData.StopUpper,
                        dispInfo.FlowControlData.StopLower,
                        dispInfo.FlowControlData.CheckTiming.Equals(true) ? 1 : 0,
                        dispInfo.FlowControlData.FlowMonitoring.Equals(true) ? 1 : 0);
                }
                else
                {
                    //Update
                    query = string.Format("UPDATE tbFlowControlData SET `PulseRate` = {0}, `SamplingDelayTime` = {1}, `ReferenceValue` = {2}, `Calibration` = {3}, " +
                        "`AlarmUpper` = {4}, `AlarmLower` = {5}, `StopUpper` = {6}, `StopLower` = {7}, `CheckTiming` = {8}, `FlowMonitoring` = {9} " +
                        "WHERE `Type` = '{10}' AND `DispIndex` = {11}",
                        dispInfo.FlowControlData.PulseRate,
                        dispInfo.FlowControlData.SamplingDelayTime,
                        dispInfo.FlowControlData.ReferenceValue,
                        dispInfo.FlowControlData.Calibration,
                        dispInfo.FlowControlData.AlarmUpper,
                        dispInfo.FlowControlData.AlarmLower,
                        dispInfo.FlowControlData.StopUpper,
                        dispInfo.FlowControlData.StopLower,
                        dispInfo.FlowControlData.CheckTiming.Equals(true) ? 1 : 0,
                        dispInfo.FlowControlData.FlowMonitoring.Equals(true) ? 1 : 0,
                        dispInfo.Type,
                        dispInfo.DispNo);
                }

                isDone = ExecuteNonQuery(query);
            }

            dt.Clear();
            dt.Dispose();

            return isDone;
        }

        public bool GetAutoSupplyControlData(string type, uint dispNo, ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbAutoSupplyControlData WHERE `Type` = '{0}' AND DispIndex = {1}", type, dispNo);
            return ExecuteReader(query, ref dt);
        }

        public bool SetAutoSupplyControlData(DispenseInfoCls dispInfo)
        {
            bool isDone = false;
            string query = string.Empty;
            DataTable dt = new DataTable();
            if(GetAutoSupplyControlData(dispInfo.Type, dispInfo.DispNo, ref dt))
            {
                if(dt.Rows.Count == 0)
                {
                    query = string.Format("INSERT INTO tbAutoSupplyControlData(`Type`,`DispIndex`,`DispName`,`AutoMode`,`LiquidSource`,`SupplyTime`,`VacuumTime`, " +
                        "`WaitTime`,`SupplyDelayTime`,`PurgeTime`) VALUES('{0}',{1},'{2}',{3},{4},{5},{6},{7},{8},{9})",
                        dispInfo.Type,
                        dispInfo.DispNo,
                        dispInfo.DispName,
                        dispInfo.AutoSupplyControlData.AutoMode.Equals(true) ? 1 : 0,
                        dispInfo.AutoSupplyControlData.LiquidSource,
                        dispInfo.AutoSupplyControlData.SupplyTime,
                        dispInfo.AutoSupplyControlData.VacuumeTime,
                        dispInfo.AutoSupplyControlData.WaitTime,
                        dispInfo.AutoSupplyControlData.SupplyDelayTime,
                        dispInfo.AutoSupplyControlData.PurgeTime);
                }
                else
                {
                    query = string.Format("UPDATE tbAutoSupplyControlData SET `DispName`='{0}',`AutoMode`={1},`LiquidSource`={2},`SupplyTime`={3},`VacuumTime`={4}, " +
                        "`WaitTime`={5},`SupplyDelayTime`={6},`PurgeTime`={7} WHERE Type='{8}' AND DispIndex={9}",
                        dispInfo.DispName,
                        dispInfo.AutoSupplyControlData.AutoMode.Equals(true) ? 1 : 0,
                        dispInfo.AutoSupplyControlData.LiquidSource,
                        dispInfo.AutoSupplyControlData.SupplyTime,
                        dispInfo.AutoSupplyControlData.VacuumeTime,
                        dispInfo.AutoSupplyControlData.WaitTime,
                        dispInfo.AutoSupplyControlData.SupplyDelayTime,
                        dispInfo.AutoSupplyControlData.PurgeTime,
                        dispInfo.Type,
                        dispInfo.DispNo);
                }

                isDone = ExecuteNonQuery(query);
            }

            dt.Clear();
            dt.Dispose();

            return isDone;
        }

        public bool GetPumpControlData(string type, uint dispNo, ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbPumpControlData WHERE `Type` ='{0}' AND `DispIndex` = {1}", type, dispNo);
            return ExecuteReader(query, ref dt);
        }

        public bool SetPumpControlData(DispenseInfoCls dispInfo)
        {
            bool isDone = false;
            DataTable dt = new DataTable();
            string query = string.Empty;

            if(GetPumpControlData(dispInfo.Type, dispInfo.DispNo, ref dt))
            {
                if(dt.Rows.Count == 0)
                {
                    query = string.Format("INSERT INTO tbPumpControlData(`Type`,`DispIndex`,`DispName`,`PumpCapa`,`PassOper`,`PulseCount`,`SpareReload`,`TotalDispCountAlarm`," +
                        "`TotalDispCountStop`,`Calibration`) VALUES('{0}',{1},'{2}',{3},{4},{5},{6},{7},{8},{9})",
                        dispInfo.Type,
                        dispInfo.DispNo,
                        dispInfo.DispName,
                        dispInfo.PumpControlData.PumpCapa,
                        dispInfo.PumpControlData.PassOper.Equals(true) ? 1 : 0,
                        dispInfo.PumpControlData.PulseCount,
                        dispInfo.PumpControlData.SpareReload,
                        dispInfo.PumpControlData.TotalDispCountAlarm,
                        dispInfo.PumpControlData.TotalDispCountStop,
                        dispInfo.PumpControlData.Calibration);
                }
                else 
                {
                    query = string.Format("UPDATE tbPumpControlData SET `DispName` = '{0}',`PumpCapa`={1},`PassOper`={2},`PulseCount`={3},`SpareReload`={4},`TotalDispCountAlarm`={5}, " +
                        "`TotalDispCountStop`={6},`Calibration`={7} WHERE Type = '{8}' AND DispIndex = {9}",
                        dispInfo.DispName,
                        dispInfo.PumpControlData.PumpCapa,
                        dispInfo.PumpControlData.PassOper.Equals(true) ? 1 : 0,
                        dispInfo.PumpControlData.PulseCount,
                        dispInfo.PumpControlData.SpareReload,
                        dispInfo.PumpControlData.TotalDispCountAlarm,
                        dispInfo.PumpControlData.TotalDispCountStop,
                        dispInfo.PumpControlData.Calibration,
                        dispInfo.Type,
                        dispInfo.DispNo);
                }

                isDone = ExecuteNonQuery(query);
            }

            dt.Clear();
            dt.Dispose();

            return isDone;
        }

        public bool SaveDispenseInfo()
        {
            string query = string.Empty;
            bool isDone = true;

            for (int i = 0; i < Global.STDispenseList.Count; i++)
            {
                DispenseInfoCls info = Global.STDispenseList[i];
                query = string.Format("UPDATE tbDispenseInfo SET Use = {0}, DummyUse = {1}, RecipeUse= {2} WHERE Type = '{3}' AND DispNo = {4}",
                                        info.IsUse.Equals(true) ? 1 : 0,
                                        info.IsUseDummy.Equals(true) ? 1 : 0,
                                        info.IsUseRecipe.Equals(true) ? 1 : 0,
                                        info.Type,
                                        info.DispNo);

                isDone = ExecuteNonQuery(query);

                if (!isDone) break;

            }

            return isDone;
        }

        public int GetMaintMode(int blockNo, int moduleNo)
        {
            int reVal = -1;
            DataTable dt = new DataTable();
            string query = string.Format("SELECT * FROM tbModuleInfo WHERE BlockNo = {0} AND ModuleNo = {1}", blockNo, moduleNo);

            if(ExecuteReader(query, ref dt))
            {
                if (dt.Rows.Count > 0) reVal = Convert.ToInt32(dt.Rows[0]["MaintMode"]);
            }

            dt.Clear();
            dt.Dispose();
            return reVal;
        }

        public bool SetMaintMode(int blockNo, int moduleNo, int value)
        {
            string query = string.Format("UPDATE tbModuleInfo SET `MaintMode` = {0} WHERE `BlockNo` = {1} AND `ModuleNo` = {2}", value, blockNo, moduleNo);

            return ExecuteNonQuery(query);
        }

        public bool GetUserInfo(ref DataTable dt)
        {
            string query = "SELECT * FROM tbUserInfo";
            return ExecuteReader(query, ref dt);
        }

        public bool CreateUserInfo(string id, string pass, int auth)
        {
            string query = string.Format("INSERT INTO tbUserInfo(`ID`, `PassWord`, `Auth`) VALUES('{0}', '{1}', {2})", id, pass, auth);
            return ExecuteNonQuery(query);
        }

        public bool ModifyUserInfo(string id, string pass, int auth)
        {
            string query = string.Format("UPDATE tbUserInfo SET `PassWord` = '{0}', `Auth` = {1} WHERE `ID` = '{2}'", pass, auth, id);
            return ExecuteNonQuery(query);
        }

        public bool DeleteUserInfo(string id)
        {
            string query = string.Format("DELETE FROM tbUserInfo WHERE ID = '{0}'", id);
            return ExecuteNonQuery(query);
        }

        public bool GetMaintSupport(ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbMaintSupport");
            return ExecuteReader(query, ref dt);
        }

        public bool SetMaintSupport(MaintSupportCls maint)
        {
            string query = string.Format("UPDATE tbMaintSupport SET `Watch`= {0}, `WarningLevel` = {1}, `LimitValue` = {2}, " +
                "`Amount` = {3}, `Type` = {4}, `Unit` = {5}, `Time` = '{6}', Alarm = {7} WHERE " +
                "`BlockNo` = {8} AND `ModuleNo` = {9} AND `Item` = '{10}'",
                maint.IsWatch.Equals(true) ? 1 : 0,
                maint.WarnLevel,
                maint.LimitValue,
                maint.Amount,
                maint.Type,
                maint.Unit,
                maint.Time,
                maint.IsAlarm.Equals(true) ? 1 : 0,
                maint.BlockNo,
                maint.ModuleNo,
                maint.Item);
            return ExecuteNonQuery(query);
        }

        public bool GetCassetteInfo(ref DataTable dt)
        {
            string query = "SELECT * FROM tbCassette";
            return ExecuteReader(query, ref dt);
        }

        public bool SetCassetteInfo(int cstNo, int index, int use)
        {
            string query = string.Format("UPDATE tbCassette SET `Usage` = {0} WHERE `CstNo` = {1} AND `Index` = {2}", use, cstNo, index);
            return ExecuteNonQuery(query);
        }

        public bool GetMonitoringData(ref DataTable dt)
        {
            string query = string.Format("SELECT * FROM tbMonitoringData");
            return ExecuteReader(query, ref dt);
        }
        /// <summary>
        /// csv 에 있는 데이터를 한번에 import 하려고 만든 함수
        /// </summary>
        /// <returns></returns>
        public bool SetImportMonitoringData()
        {
            string query = string.Empty;
            for (int i = 0; i < Global.STMonitoringList.Count; i++)
            {
                MonitoringDataCls data = Global.STMonitoringList[i];
                query = string.Format("INSERT INTO tbMonitoringData(`BlockNo`,`ModuleNo`,`MeasDataName`,`ControllerName`,`Use`,`InitTemp`,`OverTemp`,`SettlingDetermTime`,`SettlingTimeOut`,`RangeMax`,`RangeMin`) VALUES({0},{1},'{2}','{3}',{4},{5},{6},{7},{8},{9},{10})",
                    data.BlockNo,
                    data.ModuleNo,
                    data.MeasDataName,
                    data.ControllerName,
                    data.IsUse.Equals(true) ? 1 : 0,
                    data.InitTemp,
                    data.OverTemp,
                    data.SettlingDetermTime,
                    data.SettlingTimeOut,
                    data.RangeMax,
                    data.RangeMin);
                ExecuteNonQuery(query);
            }
            
            return true;
        }

        public bool SetMonitoringData(MonitoringDataCls data)
        {
            string query = string.Format("UPDATE tbMonitoringData SET `Use`={0},`InitTemp`={1},`OverTemp`={2},`SettlingDetermTime`={3},`SettlingTimeOut`={4},`RangeMax`={5},`RangeMin`={6} " +
                "WHERE `BlockNo`={7} AND `ModuleNo`={8} AND `ControllerName`='{9}'",
                data.IsUse.Equals(true) ? 1 : 0,
                data.InitTemp,
                data.OverTemp,
                data.SettlingDetermTime,
                data.SettlingTimeOut,
                data.RangeMax,
                data.RangeMin,
                data.BlockNo,
                data.ModuleNo,
                data.ControllerName);
            return ExecuteNonQuery(query);
        }

        public bool DeletePumpRecipe()
        {
            string query = "DELETE FROM tbPumpRecipe";
            return ExecuteNonQuery(query);
        }

        public bool SetPumpRecipeList()
        {
            bool isRet = true;
            string query = string.Empty;
            foreach (DirFileListCls recipe in Global.PumpRecipeFileList)
            {
                query = string.Format("INSERT INTO tbPumpRecipe (RecipeName) VALUES ('{0}');", recipe.FileName);
                if (!ExecuteNonQuery(query)) { isRet = false; break; }
            }

            return isRet;
        }
    }
}
