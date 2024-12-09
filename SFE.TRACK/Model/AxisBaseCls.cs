using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SFE.TRACK.Model
{    
    public class AxisBaseCls
    {
        public Stopwatch StopWatch = new Stopwatch();
        private int boardNo = 0;
        private int axisNo = 0;
        private enHomeState homeState = enHomeState.HOME_NONE;

        public AxisBaseCls() { }
       

        public int AxisNo
        {
            get { return axisNo; }
            set { axisNo = value; }
        }

        public int BoardNo
        {
            get { return boardNo; }
            set { boardNo = value; }
        }

        public enHomeState HomeState
        {
            get { return homeState; }
            set { homeState = value; }
        }

        public virtual bool ServoOn() { return true; }
        public virtual bool ServoOff() { return true; }
        public virtual bool IsServoOn() { return true; }
        public virtual bool GetCurPosition(ref double dPosition) { return true; }
        public virtual bool GetCurTorque(ref double dTorque) { return true; }
        public virtual bool GetServoAlarm() { return true; }
        public virtual bool CheckPlusLimit() { return true; }
        public virtual bool CheckMinusLimit() { return true; }
        public virtual bool CheckHomeState() { return true; }
        public virtual bool SetAlarmReset() { return true; }
        public virtual bool CheckAlarm() { return true; }
        public virtual bool CheckInposition() { return true; }
        public virtual bool MoveStop(double dDec) { return true; }
        public virtual bool MoveEStop() { return true; } //EMG
        public virtual bool MoveNStop() { return true; } //Normal
        public virtual bool CheckInMotion() { return true; }
        public virtual bool AxisHomeSearch() { return true; }
        public virtual bool StartOrigin() { return true; }
        public virtual bool CheckHomeEnd(long timeOut) { return true; }
        public virtual bool MoveAxis(double acc, double dec, double vel, double dist, enMoveWait wait = enMoveWait.NOWAIT) { return true; }
        public virtual bool MoveVelAxis(double vel, double acc, double dec) { return true; }
        public void SetTime()
        {
            StopWatch.Stop();
            StopWatch.Reset();
        }

        public void StartTimer()
        {
            SetTime();
            StopWatch.Start();
        }

        public bool IsTimeOver(long time)
        {
            long compareTime = StopWatch.ElapsedMilliseconds;
            if (compareTime > time) return false;
            return true;
        }

    }
}
