using System;

namespace AutoPCSX2SaveState.KeyboardAndMouse {
	class KeyboardAndMouseIdleGetter {
		public static double GetIdleTime() {
			return TimeSpan.FromMilliseconds(IdleTimeFinder.GetIdleTime()).TotalSeconds;
		}
	}
}
