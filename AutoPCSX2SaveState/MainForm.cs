using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SlimDX.DirectInput;
using SlimDX.XInput;
using AutoPCSX2SaveState.KeyboardAndMouse;
using AutoPCSX2SaveState.Controllers;

namespace AutoPCSX2SaveState
{
    public partial class MainForm : Form
    {
        private bool running = false;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private System.Timers.Timer delayTimer = new System.Timers.Timer();
        private System.Timers.Timer updateInfoTimer = new System.Timers.Timer(500);
        private int timesSaved = 0;
        private DateTime lastSave = DateTime.MinValue;
        private DateTime targetSaveTime=DateTime.Now;
        private AutoHotkey.Interop.AutoHotkeyEngine ahk = new AutoHotkey.Interop.AutoHotkeyEngine();
        private ControllerIdleGetter controllerIdleGetter;

        public MainForm()
        {
            InitializeComponent();
            timer.Elapsed += timer_Elapsed;
            delayTimer.Elapsed += timer_Elapsed;
            updateInfoTimer.Elapsed += updateInfoTimer_Elapsed;
            delayTimer.AutoReset = false;
            timer.AutoReset = true;

            controllerIdleGetter = new ControllerIdleGetter();
        }

        void updateInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateLabels();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerExpired();

            targetSaveTime= DateTime.Now.AddMilliseconds(timer.Interval);

            if (delayTimer.Enabled)
            {
                DateTime delayTarget = DateTime.Now.AddMilliseconds(delayTimer.Interval);
                if (delayTarget<targetSaveTime)
                {
                    targetSaveTime = delayTarget;
                }
            }

            updateLabels();
        }

        private void start()
        {
            timer.Interval = (double)nudSaveInterval.Value * 1000;
            targetSaveTime = DateTime.Now.AddMilliseconds(timer.Interval);
            timer.Start();
            updateInfoTimer.Start();
            btnStartStop.Text = "Stop";
            this.Text = "AutoSaveState - Running";
        }

        private void stop()
        {
            timer.Stop();
            delayTimer.Stop();
            updateInfoTimer.Stop();
            btnStartStop.Text = "Start";
            this.Text = "AutoSaveState - Stopped";
            lblTimeUntilSave.Text = "X seconds";
        }

        private void timerExpired()
        {
            double idleTime = getIdleTimeInSeconds();
            double minIdleTime = (double)nudButtonDelay.Value;

            // if the user has not pressed a button for a time longer than the minimum idle time we send the keystrokes
            if (idleTime + 0.001 > minIdleTime)
            {
                // stop the delaytimer since it is not needed anymore
                delayTimer.Stop();
                // try to save
                save();
                // restart the main timer
                timer.Interval = (double)nudSaveInterval.Value * 1000;
                // quit the function since we did all we needed to do
                return;
            }

            // this method should fire again as soon as the user is projected to have been idle long enough 
            double newInterval = (minIdleTime - idleTime) * 1000 + 100;
            delayTimer.Interval = newInterval;
            // start the timer
            delayTimer.Start();
        }

        private void save()
        {
            Color color = this.BackColor;
            this.BackColor = Color.Green;
            Application.DoEvents();
            ahk.ExecRaw(@"
Send, {F2 Down}
Sleep, 50
Send, {F2 Up}
Sleep, 500
Send, {F1}
");
            System.Threading.Thread.Sleep(500);
            this.BackColor = color;
            timesSaved++;
            lastSave = DateTime.Now;
        }

        delegate void updateLabelsDelegate();
        private void updateLabels()
        {
            if (lblHead.InvokeRequired)
            {
                lblHead.Invoke(new updateLabelsDelegate(updateLabels));
                return;
            }

            lblNumSaves.Text = String.Format("{0}", timesSaved);
            lblTimeUntilSave.Text = String.Format("{0:0.0} seconds",(targetSaveTime- DateTime.Now).TotalSeconds);
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
            return  Math.Min(KeyboardAndMouseIdleGetter.GetIdleTime(), controllerIdleGetter.GetIdleTime()); 
        }

    }

    

    
}
