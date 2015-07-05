using System.Collections.Generic;
using System.Timers;

namespace AutoPCSX2SaveState.Controllers
{
    class ControllerIdleGetter
    {
        private List<IController> controllers = new List<IController>();
        private Timer pollTimer = new Timer(50);
        public ControllerIdleGetter() {
            controllers = ControllerManager.GetControllers();
            pollTimer.Elapsed += pollTimer_Elapsed;
            pollTimer.Start();
        }

        void pollTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (IController controller in controllers)
            {
                controller.Poll();
            }
        }

        public double GetIdleTime() {
            double idleTime = double.MaxValue;
            double tempIdleTime;
            foreach (IController controller in controllers)
            {
                tempIdleTime = controller.GetIdleTime();
                if (tempIdleTime<idleTime)
                {
                    idleTime = tempIdleTime;
                }
            }
            return idleTime;
        }
    }
}
