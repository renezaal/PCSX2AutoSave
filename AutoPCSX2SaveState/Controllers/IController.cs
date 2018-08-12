namespace AutoPCSX2SaveState.Controllers {
	interface IController {
		double GetIdleTime();
		void Poll();
		void Dispose();
	}
}
