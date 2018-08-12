using System.Collections.Generic;
using System.Timers;

namespace AutoPCSX2SaveState.Controllers {
	class ControllerIdleGetter {
		private readonly List<IController> _controllers = new List<IController>();
		private Timer _pollTimer = new Timer(50);
		public ControllerIdleGetter() {
			_controllers = ControllerManager.GetControllers();
			_pollTimer.Elapsed += PollTimer_Elapsed;
			_pollTimer.Start();
		}

		void PollTimer_Elapsed(object sender, ElapsedEventArgs e) {
			foreach (IController controller in _controllers) {
				controller.Poll();
			}
		}

		public double GetIdleTime() {
			double idleTime = double.MaxValue;
			double tempIdleTime;
			foreach (IController controller in _controllers) {
				tempIdleTime = controller.GetIdleTime();
				if (tempIdleTime < idleTime) {
					idleTime = tempIdleTime;
				}
			}
			return idleTime;
		}

		public void Dispose() {
			foreach (IController controller in _controllers) {
				controller.Dispose();
			}
			ControllerManager.Dispose();
		}
	}
}
