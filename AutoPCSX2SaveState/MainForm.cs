using AutoPCSX2SaveState.Controllers;
using AutoPCSX2SaveState.KeyboardAndMouse;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;

namespace AutoPCSX2SaveState {
	public partial class MainForm : Form {
		private bool _running = false;
		private System.Timers.Timer _timer = new System.Timers.Timer();
		private System.Timers.Timer _delayTimer = new System.Timers.Timer();
		private System.Timers.Timer _updateInfoTimer = new System.Timers.Timer(500);
		private int _timesSaved = 0;
		private DateTime _lastSave = DateTime.Now;
		private DateTime _targetSaveTime = DateTime.Now;
		private AutoHotkey.Interop.AutoHotkeyEngine _ahk = AutoHotkey.Interop.AutoHotkeyEngine.Instance;
		private ControllerIdleGetter _controllerIdleGetter;

		public MainForm() {
			InitializeComponent();
			_timer.Elapsed += Timer_Elapsed;
			_delayTimer.Elapsed += Timer_Elapsed;
			_updateInfoTimer.Elapsed += UpdateInfoTimer_Elapsed;
			_delayTimer.AutoReset = false;
			_timer.AutoReset = true;

			_controllerIdleGetter = new ControllerIdleGetter();
			Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
		}

		void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e) {
			AutomationElement element = sender as AutomationElement;
			if (element == null) { return; }

			int processId = element.Current.ProcessId;
			using (Process process = Process.GetProcessById(processId)) {
				_currentProcess = process.ProcessName;
				//Debug.WriteLine(String.Format("Current process: {0}",currentProcess));
			}
		}
		private string _currentProcess = String.Empty;

		void UpdateInfoTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			UpdateLabels();
		}

		void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
			TimerExpired();

			_targetSaveTime = DateTime.Now.AddMilliseconds(_timer.Interval);

			if (_delayTimer.Enabled) {
				DateTime delayTarget = DateTime.Now.AddMilliseconds(_delayTimer.Interval);
				if (delayTarget < _targetSaveTime) {
					_targetSaveTime = delayTarget;
				}
			}

			UpdateLabels();
		}

		private void Start() {
			_timer.Interval = (double)nudSaveInterval.Value * 1000;
			_targetSaveTime = DateTime.Now.AddMilliseconds(_timer.Interval);
			_timer.Start();
			_updateInfoTimer.Start();
			btnStartStop.Text = "Stop";
			this.Text = "AutoSaveState - Running";
		}

		private void Stop() {
			_timer.Stop();
			_delayTimer.Stop();
			_updateInfoTimer.Stop();
			btnStartStop.Text = "Start";
			this.Text = "AutoSaveState - Stopped";
			lblTimeUntilSave.Text = "X seconds";
		}

		private void TimerExpired() {
			if (!_currentProcess.StartsWith("pcsx2", StringComparison.InvariantCultureIgnoreCase)) {
				_delayTimer.Stop();
				return;
			}
			double idleTime = GetIdleTimeInSeconds();
			double minIdleTime = (double)nudButtonDelay.Value;

			// if the user has not pressed a button for a time longer than the minimum idle time we send the keystrokes
			if (idleTime + 0.001 > minIdleTime) {
				// stop the delaytimer since it is not needed anymore
				_delayTimer.Stop();
				// try to save
				Save();
				// restart the main timer
				_timer.Interval = (double)nudSaveInterval.Value * 1000;
				// quit the function since we did all we needed to do
				return;
			}

			// this method should fire again as soon as the user is projected to have been idle long enough
			double newInterval = (minIdleTime - idleTime) * 1000 + 100;
			_delayTimer.Interval = newInterval;
			// start the timer
			_delayTimer.Start();
		}

		private void Save() {
			Debug.WriteLine(String.Format(@"Saving at: {0:HH':'mm':'ss}, Time since last save: {1:0.00} minutes", DateTime.Now, (DateTime.Now - _lastSave).TotalMinutes));
			Color color = this.BackColor;
			this.BackColor = Color.Green;
			Application.DoEvents();
			_ahk.ExecRaw(@"
Send, {F2 Down}
Sleep, 50
Send, {F2 Up}
Sleep, 500
Send, {F1}
");
			System.Threading.Thread.Sleep(500);
			this.BackColor = color;
			_timesSaved++;
			_lastSave = DateTime.Now;
		}

		delegate void updateLabelsDelegate();
		private void UpdateLabels() {
			if (lblHead.InvokeRequired) {
				lblHead.Invoke(new updateLabelsDelegate(UpdateLabels));
				return;
			}

			lblNumSaves.Text = String.Format("{0}", _timesSaved);
			lblTimeUntilSave.Text = String.Format("{0:0.0} seconds", (_targetSaveTime - DateTime.Now).TotalSeconds);
		}

		private void BtnStartStop_Click(object sender, EventArgs e) {
			if (_running) { Stop(); } else { Start(); }
			_running = !_running;
		}

		private double GetIdleTimeInSeconds() {
			return Math.Min(KeyboardAndMouseIdleGetter.GetIdleTime(), _controllerIdleGetter.GetIdleTime());
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			Stop();
			_controllerIdleGetter.Dispose();
		}
	}
}
