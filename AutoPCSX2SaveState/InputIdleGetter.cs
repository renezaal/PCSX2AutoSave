// <copyright file="InputIdleGetter.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>

namespace AutoPCSX2SaveState
{
	using System;
	using System.Collections.Generic;
	using System.Timers;
	using AutoPCSX2SaveState.Controllers;
	using AutoPCSX2SaveState.KeyboardAndMouse;

	internal class InputIdleGetter
	{
		private readonly List<IController> controllers = new List<IController>();
		private Timer pollTimer = new Timer(50);
		private KeyboardAndMouseIdleGetter keyboardAndMouseIdleGetter;

		public InputIdleGetter()
		{
			this.controllers = ControllerManager.GetControllers();
			this.keyboardAndMouseIdleGetter = new KeyboardAndMouseIdleGetter();
			this.pollTimer.Elapsed += this.PollTimer_Elapsed;
			this.pollTimer.Start();
		}

		private void PollTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			foreach (IController controller in this.controllers)
			{
				controller.Poll();
			}

			this.keyboardAndMouseIdleGetter.Poll();
		}

		public double GetIdleTime()
		{
			double idleTime = double.MaxValue;
			double tempIdleTime;
			foreach (IController controller in this.controllers)
			{
				tempIdleTime = controller.GetIdleTime();
				if (tempIdleTime < idleTime)
				{
					idleTime = tempIdleTime;
				}
			}

			idleTime = Math.Min(this.keyboardAndMouseIdleGetter.GetIdleTime(), idleTime);

			return idleTime;
		}

		public void Dispose()
		{
			foreach (IController controller in this.controllers)
			{
				controller.Dispose();
			}
		}
	}
}
