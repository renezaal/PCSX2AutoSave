// <copyright file="XinputController.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>

namespace AutoPCSX2SaveState.Controllers
{
	using System;
	using Vortice.XInput;

	internal class XinputController : IController
	{
		private readonly int userIndex;
		private DateTime lastActivity;
		private int lastPacket;
		private GamepadButtons lastButtonsState;
		private byte lastRightTriggerState;
		private byte lastLeftTriggerState;

		public XinputController(int user)
		{
			if (((int)user) < 0 || ((int)user) > 3)
			{
				throw new ArgumentOutOfRangeException("user");
			}

			this.userIndex = user;
			this.lastActivity = DateTime.Now;
			this.lastPacket = 0;
			this.lastButtonsState = 0;
			this.lastRightTriggerState = 0;
			this.lastLeftTriggerState = 0;
		}

		public double GetIdleTime()
		{
			double idleSeconds = (DateTime.Now - this.lastActivity).TotalSeconds;
			Console.WriteLine($"Constroller index {userIndex} idle time: {idleSeconds} seconds");
			return idleSeconds;
		}

		public void Poll()
		{
			if (!XInput.GetState(this.userIndex, out State state))
			{
				// Console.WriteLine("Not connected or error");
				return;
			}

			// Console.WriteLine($"Polling XInput at index {_user}");

			// Console.WriteLine($"Packetnumber: {state.PacketNumber}, _lastPacketNumber {_lastPacket}");
			if (state.PacketNumber == this.lastPacket)
			{
				return;
			}

			this.lastPacket = state.PacketNumber;

			GamepadButtons buttonsState = state.Gamepad.Buttons;

			if (buttonsState != this.lastButtonsState)
			{
				// Console.WriteLine($"Buttons state changed, new state {buttonsState}");
				this.lastActivity = DateTime.Now;
				this.lastButtonsState = buttonsState;
				return;
			}

			byte leftTriggerState = state.Gamepad.LeftTrigger;
			if (Math.Abs(leftTriggerState - this.lastLeftTriggerState) > Gamepad.TriggerThreshold)
			{
				// Console.WriteLine($"leftTriggerState changed, new state {leftTriggerState}");
				this.lastActivity = DateTime.Now;
				return;
			}

			this.lastLeftTriggerState = leftTriggerState;

			byte rightTriggerState = state.Gamepad.RightTrigger;
			if (Math.Abs(rightTriggerState - this.lastRightTriggerState) > Gamepad.TriggerThreshold)
			{
				// Console.WriteLine($"rightTriggerState changed, new state {rightTriggerState}");
				this.lastActivity = DateTime.Now;
				return;
			}

			this.lastRightTriggerState = rightTriggerState;

			// Console.WriteLine($"Last activity: {_lastActivity}");
		}

		public void Dispose()
		{
			// do nothing
		}
	}
}
