using SlimDX.DirectInput;
using System;

namespace AutoPCSX2SaveState.Controllers {
	class DirectInputController : IController {
		private Joystick _stick;
		private ulong _lastState;
		private DateTime _lastActivity;

		public DirectInputController(Joystick stick) {
			this._stick = stick;
			_lastState = 0;
			_lastActivity = DateTime.Now;
		}

		public double GetIdleTime() {
			return (DateTime.Now - _lastActivity).TotalSeconds;
		}

		public void Poll() {
			if (_stick.Disposed) {
				return;
			}
			ulong buttonsState = 0;
			JoystickState jState = _stick.GetCurrentState();

			bool[] buttonsArray = jState.GetButtons();

			for (int i = 0; i < buttonsArray.Length; i++) {
				buttonsState |= (ulong)(buttonsArray[i] ? 1 : 0) << i;
			}

			if (buttonsState != _lastState) {
				_lastActivity = DateTime.Now;
				_lastState = buttonsState;
			}
		}

		public void Dispose() {
			_stick.Dispose();
		}
	}
}
