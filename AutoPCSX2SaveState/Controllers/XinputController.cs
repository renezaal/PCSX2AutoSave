using SlimDX.XInput;
using System;

namespace AutoPCSX2SaveState.Controllers {
	class XinputController : IController {
		private readonly UserIndex _user;
		private Controller _controller;
		private DateTime _lastActivity;
		private uint _lastPacket;
		private ulong _lastButtonsState;
		private byte _lastRightTriggerState;
		private byte _lastLeftTriggerState;

		public XinputController(UserIndex user) {
			if (((int)user) < 0 || ((int)user) > 3) {
				throw new ArgumentOutOfRangeException("user");
			}

			this._user = user;
			_controller = new Controller(user);
			_lastActivity = DateTime.Now;
			_lastPacket = 0;
			_lastButtonsState = 0;
			_lastRightTriggerState = 0;
			_lastLeftTriggerState = 0;
		}

		public double GetIdleTime() {
			return (DateTime.Now - _lastActivity).TotalSeconds;
		}

		public void Poll() {
			if (!_controller.IsConnected) {
				return;
			}

			State state = _controller.GetState();

			if (state.PacketNumber == _lastPacket) {
				return;
			}
			_lastPacket = state.PacketNumber;

			ulong buttonsState = (ulong)state.Gamepad.Buttons;

			if (buttonsState != _lastButtonsState) {
				_lastActivity = DateTime.Now;
				_lastButtonsState = buttonsState;
				return;
			}

			byte leftTriggerState = state.Gamepad.LeftTrigger;
			if (Math.Abs(leftTriggerState - _lastLeftTriggerState) > Gamepad.GamepadTriggerThreshold) {
				_lastActivity = DateTime.Now;
				return;
			}
			_lastLeftTriggerState = leftTriggerState;

			byte rightTriggerState = state.Gamepad.RightTrigger;
			if (Math.Abs(rightTriggerState - _lastRightTriggerState) > Gamepad.GamepadTriggerThreshold) {
				_lastActivity = DateTime.Now;
				return;
			}
			_lastRightTriggerState = rightTriggerState;
		}

		public void Dispose() {
			// do nothing
		}
	}
}
