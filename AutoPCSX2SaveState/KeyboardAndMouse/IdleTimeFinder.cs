using System;
using System.Runtime.InteropServices;

namespace AutoPCSX2SaveState.KeyboardAndMouse {
	internal struct LASTINPUTINFO {
		public uint CbSize;
		public uint DwTime;
	}

	/// <summary>
	/// Helps to find the idle time, (in ticks) spent since the last user input
	/// </summary>
	public class IdleTimeFinder {
		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		[DllImport("Kernel32.dll")]
		private static extern uint GetLastError();

		public static uint GetIdleTime() {
			LASTINPUTINFO lastInPut = new LASTINPUTINFO();
			lastInPut.CbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
			GetLastInputInfo(ref lastInPut);

			return ((uint)Environment.TickCount - lastInPut.DwTime);
		}

		/// <summary>
		/// Get the Last input time in ticks
		/// </summary>
		/// <returns></returns>
		public static long GetLastInputTime() {
			LASTINPUTINFO lastInPut = new LASTINPUTINFO();
			lastInPut.CbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
			if (!GetLastInputInfo(ref lastInPut)) {
				throw new Exception(GetLastError().ToString());
			}
			return lastInPut.DwTime;
		}
	}
}
