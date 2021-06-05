// <copyright file="KeyboardAndMouseIdleGetter.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>

namespace AutoPCSX2SaveState.KeyboardAndMouse
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;
	using System.Windows.Input;

	internal class KeyboardAndMouseIdleGetter
	{
		private DateTime lastChangeTime;
		private Point lastMousePosition;
		private bool[] lastKeyboardState;

		public KeyboardAndMouseIdleGetter()
		{
			this.lastChangeTime = DateTime.Now;
			this.lastMousePosition = default;
			this.lastKeyboardState = new bool[256];
		}

		[DllImport("user32.dll")]
		private static extern short GetKeyState(int nVirtKey);

		public void Poll()
		{
			if (!this.lastMousePosition.Equals(Cursor.Position))
			{
				this.lastMousePosition = Cursor.Position;
				Console.WriteLine($"Mouse position changed. {this.lastMousePosition}");
				this.lastChangeTime = DateTime.Now;
			}

			var keyboardState = new bool[256];
			bool keyboardStateChanged = false;
			for (int i = 0; i < 255; i++)
			{
				bool keyState = (GetKeyState(i) & 0b1000000000000000) != 0;
				keyboardState[i] = keyState;
				if (keyState != this.lastKeyboardState[i])
				{
					string text = keyState ? "Down" : "Up";
					Console.WriteLine($"Key changed. {(Keys)i}, {text}");
					keyboardStateChanged = true;
				}
			}

			if (keyboardStateChanged)
			{
				this.lastKeyboardState = keyboardState;
				Console.WriteLine("Keys changed.");
				this.lastChangeTime = DateTime.Now;
			}
		}

		public double GetIdleTime()
		{
			double idleSeconds = (DateTime.Now - this.lastChangeTime).TotalSeconds;
			Console.WriteLine($"Keyboard and mouse idle time: {idleSeconds} seconds");
			return idleSeconds;
		}
	}
}
