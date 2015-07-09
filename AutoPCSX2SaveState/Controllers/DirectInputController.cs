using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPCSX2SaveState.Controllers
{
    class DirectInputController : IController
    {
        private Joystick stick;
        private ulong lastState;
        private DateTime lastActivity;

        public DirectInputController(Joystick stick)
        {
            this.stick = stick;
            lastState = 0;
            lastActivity = DateTime.Now;
        }


        public double GetIdleTime()
        {
            return (DateTime.Now - lastActivity).TotalSeconds;
        }


        public void Poll()
        {
            if (stick.Disposed)
            {
                return;
            }
            ulong buttonsState = 0;
            JoystickState jState = stick.GetCurrentState();

            bool[] buttonsArray = jState.GetButtons();

            for (int i = 0; i < buttonsArray.Length; i++)
            {
                buttonsState |= (ulong)(buttonsArray[i] ? 1 : 0) << i;
            }

            if (buttonsState != lastState)
            {
                lastActivity = DateTime.Now;
                lastState = buttonsState;
            }
        }

        public void Dispose()
        {
            stick.Dispose();
        }
    }
}
