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

namespace AutoPCSX2SaveState
{
    public partial class Form1 : Form
    {
        private bool running = false;
        private System.Timers.Timer timer = new System.Timers.Timer();
        private System.Timers.Timer delayTimer = new System.Timers.Timer();
        private System.Timers.Timer checkControllerTimer = new System.Timers.Timer();
        private int timesSaved = 0;
        private DateTime lastActivity = DateTime.Now;
        private DateTime lastSave = DateTime.MinValue;
        private Joystick[] sticks;
        private Controller[] controllers;
        private Dictionary<object, ulong> buttonStates = new Dictionary<object, ulong>();
        private Dictionary<object, float> triggerStates = new Dictionary<object, float>();

        public Form1()
        {
            InitializeComponent();
            timer.Elapsed += timer_Elapsed;
            delayTimer.Elapsed += timer_Elapsed;
            delayTimer.AutoReset = false;
            timer.AutoReset = true;
            sticks = GamePadHelper.GetSticks(new DirectInput());
            foreach (Joystick stick in sticks)
            {
                buttonStates[stick] = 0;
            }
            controllers = getControllers();
            foreach (Controller controller in controllers)
            {
                string key = controller.GetHashCode().ToString();
                triggerStates[key + "left"] = 0;
                triggerStates[key + "right"] = 0;
            }
            checkControllerTimer.AutoReset = true;
            checkControllerTimer.Interval = 50;
            checkControllerTimer.Elapsed += checkControllerTimer_Elapsed;
            checkControllerTimer.Start();
        }

        void checkControllerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkControllerIdle();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerExpired();
        }

        private void anyActivity() { lastActivity = DateTime.Now; }

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
            //lblHead.Text = String.Format("Idle time: {0}, times saved: {1}, time since last save: {2}", idleTime, timesSaved,(DateTime.Now-lastSave).TotalSeconds);
            lblIdle.Text = String.Format("Idle time: {0}", idleTime);
            lblMinIdle.Text = String.Format("Minimum idle time: {0}", minIdleTime);
            lblNumSaves.Text = String.Format("Number of saves: {0}", timesSaved);
            lblTimeSinceLastSave.Text = String.Format("Time since last save: {0}", DateTime.Now - lastSave);
            // if the user has not pressed a button for a time longer than the minimum idle time we send the keystrokes
            if (idleTime + 0.001 > minIdleTime)
            {
                // stop the delaytimer since it is not needed anymore
                delayTimer.Stop();
                // send the keystrokes
                sendKeystrokes();
                //lblHead.Text = String.Format("Idle time: {0}, times saved: {1}", idleTime, timesSaved);
                // quit the function since we did all we needed to do
                return;
            }

            // this method should fire again as soon as the user is projected to have been idle long enough 
            double newInterval = (minIdleTime - idleTime) * 1000 + 100;
            lblRetryInterval.Text = String.Format("Delay interval: {0}", newInterval);
            delayTimer.Interval = newInterval;
            // start the timer
            delayTimer.Start();
        }

        private void sendKeystrokes()
        {
            //SendKeys.SendWait("{F2}");
            //System.Threading.Thread.Sleep(50);
            //SendKeys.SendWait("{F1}");
            timesSaved++;
            lastSave = DateTime.Now;
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
            double idleTimeLastActivity = (DateTime.Now - lastActivity).TotalSeconds;
            // somehow this returns in seconds
            double idleTimeLastInputInfo = TimeSpan.FromMilliseconds(IdleTimeFinder.GetIdleTime()).TotalSeconds;
            double idleTime = Math.Min(idleTimeLastActivity, idleTimeLastInputInfo); ;
            return idleTime;
        }

        private void checkControllerIdle()
        {
            JoystickState jState;
            ulong buttons;
            foreach (Joystick stick in sticks)
            {
                buttons = 0;
                jState = stick.GetCurrentState();
                bool[] buttonsTemp = jState.GetButtons();
                for (int i = 0; i < buttonsTemp.Length; i++)
                {
                    buttons |= (ulong)(buttonsTemp[i] ? 1 : 0) << i;
                }
                if (buttons != buttonStates[stick])
                {
                    lastActivity = DateTime.Now;
                    buttonStates[stick] = buttons;
                    return;
                }
            }
            State cState;
            foreach (Controller controller in controllers)
            {
                if (!controller.IsConnected)
                {
                    continue;
                }
                cState = controller.GetState();
                buttons = (ulong)cState.Gamepad.Buttons;
                if (!buttonStates.ContainsKey(controller))
                {
                    buttonStates[controller] = buttons;
                }
                if (buttons != buttonStates[controller])
                {
                    lastActivity = DateTime.Now;
                    buttonStates[controller] = buttons;
                    return;
                }
                string key = controller.GetHashCode().ToString();
                float triggerState=cState.Gamepad.LeftTrigger;
                float previousTriggerState=triggerStates[key+"left"];
                if (Math.Abs(triggerState-previousTriggerState) > Gamepad.GamepadTriggerThreshold)
                {
                    lastActivity = DateTime.Now;
                    return;
                }
                triggerStates[key + "left"] = triggerState;

                triggerState = cState.Gamepad.RightTrigger;
                previousTriggerState = triggerStates[key + "right"];
                if (Math.Abs(triggerState - previousTriggerState) > Gamepad.GamepadTriggerThreshold)
                {
                    lastActivity = DateTime.Now;
                    return;
                }
                triggerStates[key + "right"] = triggerState;
            }
        }

        private Controller[] getControllers()
        {
            List<Controller> controllers = new List<Controller>();
            Controller controller;
            foreach (UserIndex user in Enum.GetValues(typeof(UserIndex)).Cast<UserIndex>())
            {
                if (user == UserIndex.Any)
                {
                    continue;
                }
                controller = new Controller(user);
                if (controller.IsConnected)
                {
                    controllers.Add(controller);
                }
            }
            return controllers.ToArray();
        }

    }

    public static class GamePadHelper
    {
        public static Joystick[] GetSticks(DirectInput input)
        {

            List<SlimDX.DirectInput.Joystick> sticks = new List<SlimDX.DirectInput.Joystick>(); // Creates the list of joysticks connected to the computer via USB.
            foreach (DeviceInstance device in input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                // Creates a joystick for each game device in USB Ports
                try
                {
                    Joystick stick = new SlimDX.DirectInput.Joystick(input, device.InstanceGuid);
                    stick.Acquire();

                    // Gets the joysticks properties and sets the range for them.
                    foreach (DeviceObjectInstance deviceObject in stick.GetObjects())
                    {
                        if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
                            stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
                    }

                    // Adds how ever many joysticks are connected to the computer into the sticks list.
                    sticks.Add(stick);
                }
                catch (DirectInputException)
                {
                }
            }
            return sticks.ToArray();
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
