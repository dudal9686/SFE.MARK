using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFE.TRACK.Model
{
    public class AxisCls : AxisBaseCls
    {  
        public AxisCls() { }    
        
        public AxisCls(int axisNo)
        {
            AxisNo = axisNo;
        }

        public AxisCls(int axisNo, int boardNo)
        {
            AxisNo = axisNo;
            BoardNo = boardNo;
        }

        public static bool OpenDevice()
        {
            bool isOpen = true;
            try
            {
                if (CAXL.AxlIsOpened() == 0)
                {
                    if (CAXL.AxlOpen(7) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS) isOpen = false;

                    string motorFile = @"D:\Motor\AjinMotor.mot";

                    if (CAXM.AxmMotLoadParaAll(motorFile) != (uint)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    {
                        isOpen = false;
                    }
                }
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); isOpen = false; }

            return isOpen;
        }

        public override bool IsServoOn()
        {
            uint nErrorCode = 0;
            uint nServoOn = 0;

            nErrorCode = CAXM.AxmSignalIsServoOn(AxisNo, ref nServoOn);
            Thread.Sleep(10);
            if(nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) return nServoOn.Equals(0) ? false : true;
            //else MessageBox.Show

            return false;
        }

        public override bool ServoOn()
        {
            uint nErrorCode = 0;
            bool isServoOn = false;
            if (!SetAlarmReset()) return false;
            if (!IsServoOn()) nErrorCode = CAXM.AxmSignalServoOn(AxisNo, 1);
            Thread.Sleep(100);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                //Global.STLog.AddLog(string.Format("[ServoOn] {0}Axis ServoOn Fail"));
                return false;
            }
            else
            {
                StartTimer();
                while (IsTimeOver(3000))
                {
                    if (IsServoOn()) { isServoOn = true; break; }
                }
            }
            return isServoOn;
        }

        public override bool ServoOff()
        {
            uint nErrorCode = 0;
            if (IsServoOn()) nErrorCode = CAXM.AxmSignalServoOn(AxisNo, 0);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
            {
                //Global.STLog.AddLog(string.Format("[ServoOff] {0}Axis ServoOff Fail"));
                return false; 
            }
            return true;
        }

        public override bool GetCurPosition(ref double dPosition)
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmStatusGetActPos(AxisNo, ref dPosition);

            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) { dPosition = 0; return false; }
            return true;
        }

        public override bool GetCurTorque(ref double dTorque)
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmStatusReadTorque(AxisNo, ref dTorque);

            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) { dTorque = 0; return false; }
            return true;
        }

        public override bool GetServoAlarm()
        {
            uint nErrorCode = 0;
            uint nState = 0;
            nErrorCode = CAXM.AxmSignalReadServoAlarm(AxisNo, ref nState);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) { return false; }
            if (nState == (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE)
            {
                ushort codeAlarm = 0;
                nErrorCode = CAXM.AxmSignalReadServoAlarmCode(AxisNo, ref codeAlarm);
                //Servo Alarm
                return true;
            }

            return false;
        }

        public override bool CheckInMotion()
        {
            uint nErrorCode = 0;
            uint nStatus = 0;
            nErrorCode = CAXM.AxmStatusReadInMotion(AxisNo, ref nStatus);

            if (nErrorCode == (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS && nStatus == (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE) { return true; }

            return false;
        }

        public override bool CheckInposition()
        {
            uint nErrorCode = 0;
            uint nStatus = 0;
            nErrorCode = CAXM.AxmSignalReadInpos(AxisNo, ref nStatus);
            if (nErrorCode == (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS && nStatus == (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE) { return true; }
            return false;
        }

        public override bool CheckPlusLimit()
        {
            uint nErrorCode = 0;
            uint nPositive = 0;
            uint nNegative = 0;
            nErrorCode = CAXM.AxmSignalReadLimit(AxisNo, ref nPositive, ref nNegative);

            if (nErrorCode != 0) { return false; }
            if (nPositive == (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE) return true;

            return false;
        }

        public override bool CheckMinusLimit()
        {
            uint nErrorCode = 0;
            uint nPositive = 0;
            uint nNegative = 0;
            nErrorCode = CAXM.AxmSignalReadLimit(AxisNo, ref nPositive, ref nNegative);

            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) { return false; }
            if (nNegative == (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE) return true;

            return false;
        }

        public override bool CheckAlarm()
        {
            if (GetServoAlarm() || CheckMinusLimit() || CheckPlusLimit()) return true;
            return false;
        }

        public override bool SetAlarmReset()
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmSignalServoAlarmReset(AxisNo, (uint)AXT_MOTION_SIGNAL_LEVEL.ACTIVE);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) 
            {
                //Global.STLog.AddLog(string.Format("[SetAlarmReset] {0}Axis AlarmReset Fail", AxisNo));
                return false; 
            }
            Thread.Sleep(20);
            
            return true;
        }

        public override bool MoveStop(double dDec)
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmMoveStop(AxisNo, dDec);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS) { return false; }

            return true;
        }

        public override bool MoveEStop()
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmMoveEStop(AxisNo);

            if (nErrorCode != 0) { return false; }

            return true;
        }

        public override bool MoveNStop()
        {
            uint nErrorCode = 0;
            nErrorCode = CAXM.AxmMoveSStop(AxisNo);

            if (nErrorCode != 0) { return false; }

            return true;
        }

        public override bool MoveAxis(double acc, double dec, double vel, double dist, enMoveWait waitmode = enMoveWait.NOWAIT)
        {
            if(!IsServoOn())
            {
                //Global.STLog.AddLog(string.Format("[MoveAxis] {0}Axis Servo On Fail", AxisNo));
                return false;
            }

            if(!CheckAlarm())
            {
                //Global.STLog.AddLog(string.Format("[MoveAxis] {0}Axis No Alarm Clear", AxisNo));
                return false;
            }

            uint nErrorCode = 0;

            if (waitmode == enMoveWait.NOWAIT) nErrorCode = CAXM.AxmMoveStartPos(AxisNo, dist, vel, acc, dec);
            else if (waitmode == enMoveWait.WAIT) nErrorCode = CAXM.AxmMovePos(AxisNo, dist, vel, acc, dec);

            Thread.Sleep(5);
            if(nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                //Global.STLog.AddLog(string.Format("[MoveAxis] {0}Axis Position Move Fail", AxisNo));
                return false;
            }

            return true;
        }

        public override bool MoveVelAxis(double vel, double acc, double dec) //JOG
        {
            if (!IsServoOn())
            {
                //Global.STLog.AddLog(string.Format("[MoveVelAxis] {0}Axis Servo On Fail", AxisNo));
                return false;
            }

            if (!CheckAlarm())
            {
                //Global.STLog.AddLog(string.Format("[MoveVelAxis] {0}Axis No Alarm Clear", AxisNo));
                return false;
            }

            uint nErrorCode = 0;

            nErrorCode = CAXM.AxmMoveVel(AxisNo, vel, acc, dec);


            Thread.Sleep(5);
            if (nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                //Global.STLog.AddLog(string.Format("[MoveVelAxis] {0}Axis Position Move Fail", AxisNo));
                return false;
            }
            return true;
        }
        public override bool AxisHomeSearch()
        {
            HomeState = enHomeState.HOME_NONE;
            if(CheckAlarm())
            {
                if (!SetAlarmReset()) { HomeState = enHomeState.HOME_ERROR; return false; }            
            }

            if (!IsServoOn())
            {
                if (!ServoOn()) { HomeState = enHomeState.HOME_ERROR; return false; }
            }

            if (!StartOrigin()) return false;
            if (!CheckHomeEnd(30000))
            {
                HomeState = enHomeState.HOME_ERROR;
                //Global.STLog.AddLog(string.Format("[AxisHomeSearch] {0}Axis HomeSearch Fail", AxisNo));
                return false;
            }
            else HomeState = enHomeState.HOME_OK;

            return true;
        }

        public override bool StartOrigin()
        {
            uint nErrorCode = CAXM.AxmHomeSetStart(AxisNo);
            if(nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            {
                HomeState = enHomeState.HOME_ERROR;
                //Global.STLog.AddLog(string.Format("[StartOrigin] {0}Axis HomeSearch Fail", AxisNo));
                return false;
            }
            return true;
        }

        public override bool CheckHomeEnd(long timeOut)
        {
            StartTimer();
            bool isHome = false;
            uint nHomeResult = 0;
            uint nErrorCode = 0;

            while(IsTimeOver(timeOut))
            {
                nErrorCode = CAXM.AxmHomeGetResult(AxisNo, ref nHomeResult);
                if(nErrorCode != (int)AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                {
                    HomeState = enHomeState.HOME_ERROR;
                    //Global.STLog.AddLog(string.Format("[CheckHomeEnd] {0}Axis CheckHomeEnd Fail", AxisNo));
                    isHome = false;
                    break;
                }

                switch(nHomeResult)
                {
                    case (uint)AXT_MOTION_HOME_RESULT.HOME_SUCCESS :
                        isHome = true;
                        break;
                }

                if (isHome) break;
                Thread.Sleep(10);
            }

            MoveStop(0);
            Thread.Sleep(10);
            return isHome;
        }
    }
}
