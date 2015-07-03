using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AutoPCSX2SaveState
{
    public partial class Form1 : Form
    {
        private bool running = false;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private System.Timers.Timer delayTimer = new System.Timers.Timer();
        private int timesSaved = 0;

        public Form1()
        {
            InitializeComponent();
            timer.Elapsed += timer_Elapsed;
            delayTimer.Elapsed += timer_Elapsed;
            delayTimer.AutoReset = false;
            timer.AutoReset = true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerExpired();
        }

        private void start()
        {
            timer.Interval = (double)nudSaveInterval.Value * 1000;
            timer.Start();
            btnStartStop.Text = "Stop";
            this.Text = "AutoSaveState - Running";
        }

        private void stop()
        {
            timer.Stop();
            delayTimer.Stop();
            btnStartStop.Text = "Start";
            this.Text = "AutoSaveState - Stopped";
        }

        delegate void timerExpiredDelegate();
        private void timerExpired()
        {
            if (lblHead.InvokeRequired)
            {
                lblHead.Invoke(new timerExpiredDelegate(timerExpired));
                return;
            }
            double idleTime = getIdleTimeInSeconds();
            double minIdleTime = (double)nudButtonDelay.Value;
            lblHead.Text = String.Format("Idle time: {0}, times saved: {1}", idleTime, timesSaved);

            // if the user has not pressed a button for a time longer than the minimum idle time we send the keystrokes
            if (idleTime + 0.001 > minIdleTime)
            {
                // stop the delaytimer since it is not needed anymore
                delayTimer.Stop();
                // send the keystrokes
                sendKeystrokes();
                // quit the function since we did all we needed to do
                return;
            }

            // this method should fire again as soon as the user is projected to have been idle long enough 
            delayTimer.Interval = (minIdleTime - idleTime) * 1000;
            // start the timer
            delayTimer.Start();
        }

        private void sendKeystrokes()
        {
            SendKeys.SendWait("{F2}");
            System.Threading.Thread.Sleep(50);
            SendKeys.SendWait("{F1}");
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (running)
            { stop(); }
            else
            { start(); }
            running = !running;
        }

        private double getIdleTimeInSeconds()
        {
            // somehow this returns in seconds
            return TimeSpan.FromTicks(IdleTimeFinder.GetIdleTime()).TotalMilliseconds;
        }
    }

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    /// <summary>
    /// Helps to find the idle time, (in ticks) spent since the last user input
    /// </summary>
    public class IdleTimeFinder
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in ticks
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }
    }
}
