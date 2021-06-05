// <copyright file="MainForm.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>

namespace AutoPCSX2SaveState
{
	using System;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Automation;
	using System.Windows.Forms;

	public partial class MainForm : Form
	{
		private bool running = false;
		private System.Timers.Timer timer = new System.Timers.Timer();
		private System.Timers.Timer delayTimer = new System.Timers.Timer();
		private System.Timers.Timer updateInfoTimer = new System.Timers.Timer(500);
		private int timesSaved = 0;
		private DateTime lastSave = DateTime.Now;
		private DateTime targetSaveTime = DateTime.Now;
		private AutoHotkey.Interop.AutoHotkeyEngine ahk = AutoHotkey.Interop.AutoHotkeyEngine.Instance;
		private InputIdleGetter controllerIdleGetter;

		public MainForm()
		{
			this.InitializeComponent();
			this.timer.Elapsed += this.Timer_Elapsed;
			this.delayTimer.Elapsed += this.Timer_Elapsed;
			this.updateInfoTimer.Elapsed += this.UpdateInfoTimer_Elapsed;
			this.delayTimer.AutoReset = false;
			this.timer.AutoReset = true;

			this.controllerIdleGetter = new InputIdleGetter();
			Automation.AddAutomationFocusChangedEventHandler(this.OnFocusChanged);
		}

		private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
		{
			AutomationElement element = sender as AutomationElement;
			if (element == null) { return; }

			try
			{
				int processId = element.Current.ProcessId;
				using (Process process = Process.GetProcessById(processId))
				{
					this.currentProcess = process.ProcessName;
					Debug.WriteLine(String.Format("Current process: {0}", this.currentProcess));
				}
			}
			catch (Exception)
			{
				// Ignore.
			}
		}

		private string currentProcess = String.Empty;

		private void UpdateInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.UpdateLabels();
		}

		private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			this.TimerExpired();

			this.targetSaveTime = DateTime.Now.AddMilliseconds(this.timer.Interval);

			if (this.delayTimer.Enabled)
			{
				DateTime delayTarget = DateTime.Now.AddMilliseconds(this.delayTimer.Interval);
				if (delayTarget < this.targetSaveTime)
				{
					this.targetSaveTime = delayTarget;
				}
			}

			this.UpdateLabels();
		}

		private void Start()
		{
			this.timer.Interval = (double)this.nudSaveInterval.Value * 1000;
			this.targetSaveTime = DateTime.Now.AddMilliseconds(this.timer.Interval);
			this.timer.Start();
			this.updateInfoTimer.Start();
			this.btnStartStop.Text = "Stop";
			this.Text = "AutoSaveState - Running";
		}

		private void Stop()
		{
			this.timer.Stop();
			this.delayTimer.Stop();
			this.updateInfoTimer.Stop();
			this.btnStartStop.Text = "Start";
			this.Text = "AutoSaveState - Stopped";
			this.lblTimeUntilSave.Text = "X seconds";
		}

		private void TimerExpired()
		{
			if (!this.currentProcess.StartsWith("pcsx2", StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine($"Currentprocess is not pcsx2, name is {this.currentProcess}");
				this.delayTimer.Stop();
				return;
			}

			double idleTime = this.controllerIdleGetter.GetIdleTime();
			double minIdleTime = (double)this.nudButtonDelay.Value;

			// if the user has not pressed a button for a time longer than the minimum idle time we send the keystrokes
			if (idleTime + 0.001 > minIdleTime)
			{
				// stop the delaytimer since it is not needed anymore
				this.delayTimer.Stop();

				// try to save
				this.Save();

				// restart the main timer
				this.timer.Interval = (double)this.nudSaveInterval.Value * 1000;

				// quit the function since we did all we needed to do
				return;
			}
			else
			{
				Console.WriteLine("Idle time too short, rescheduling save");
			}

			// this method should fire again as soon as the user is projected to have been idle long enough
			double newInterval = ((minIdleTime - idleTime) * 1000) + 100;
			this.delayTimer.Interval = newInterval;

			// start the timer
			this.delayTimer.Start();
		}

		private void Save()
		{
			Debug.WriteLine($"Saving at: {DateTime.Now:HH':'mm':'ss}, Time since last save: {(DateTime.Now - this.lastSave).TotalMinutes:0.00} minutes");
			Color color = this.BackColor;
			this.BackColor = Color.Green;
			Application.DoEvents();
			this.ahk.ExecRaw(@"
Send, {F2 Down}
Sleep, 50
Send, {F2 Up}
Sleep, 500
Send, {F1}
");
			System.Threading.Thread.Sleep(500);
			this.BackColor = color;
			this.timesSaved++;
			this.lastSave = DateTime.Now;
		}

		private delegate void UpdateLabelsDelegate();

		private void UpdateLabels()
		{
			if (this.lblHead.InvokeRequired)
			{
				this.lblHead.Invoke(new UpdateLabelsDelegate(this.UpdateLabels));
				return;
			}

#pragma warning disable SA1121 // Use built-in type alias
			this.lblNumSaves.Text = String.Format("{0}", this.timesSaved);
			this.lblTimeUntilSave.Text = String.Format("{0:0.0} seconds", (this.targetSaveTime - DateTime.Now).TotalSeconds);
#pragma warning restore SA1121 // Use built-in type alias
		}

		private void BtnStartStop_Click(object sender, EventArgs e)
		{
			if (this.running) { this.Stop(); } else { this.Start(); }
			this.running = !this.running;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.Stop();
			this.controllerIdleGetter.Dispose();
		}
	}
}
