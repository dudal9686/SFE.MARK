﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Threading;
using System.Data;
using System.Windows;
using SFE.TRACK.Model;

namespace SFE.TRACK.DB
{    
    public class AccessDBCls
    {
        OleDbConnection dbConnect = null;
        OleDbCommand dbCommand = null;
        OleDbDataAdapter dbAdapter = null;
        String connectString = string.Empty;
        Mutex sync = new Mutex();

        public AccessDBCls()
        {
            connectString = string.Format("Provider=Microsoft.Jet.OleDb.4.0; Data Source={0}", System.Windows.Forms.Application.StartupPath + @"\MachineDB.mdb");
        }

        private bool Open()
        {
            try
            {
                dbConnect = new OleDbConnection(connectString);
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
                dbCommand = new OleDbCommand(query, dbConnect);
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
                dbCommand = new OleDbCommand(query, dbConnect);
                dbAdapter = new OleDbDataAdapter(dbCommand);
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

        public bool SaveDispenseInfo(int blockNo, int moduleNo)
        {
            string query = string.Empty;
            bool isDone = true;
            Model.ModuleBaseCls module = Global.GetModule(blockNo, moduleNo);

            if (module == null) return false;

            for(int i = 0; i < module.DispenseList.Count; i++)
            {
                DispenseInfoCls info = module.DispenseList[i];
                query = string.Format("UPDATE tbDispenseInfo SET Use = {0}, DummyUse = {1}, RecipeUse= {2} WHERE BlockNo = {3} AND ModuleNo = {4} AND DispNo = {5}",
                                        info.IsUse.Equals(true) ? 1 : 0,
                                        info.IsUseDummy.Equals(true) ? 1 : 0,
                                        info.IsUseRecipe.Equals(true) ? 1 : 0,
                                        blockNo,
                                        moduleNo,
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
    }
}