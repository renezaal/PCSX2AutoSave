using SlimDX.DirectInput;
using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPCSX2SaveState.Controllers
{
    static class ControllerManager
    {
        public static List<IController> GetControllers()
        {
            List<IController> controllers = new List<IController>();
            controllers.AddRange(GetDirectInputControllers());
            controllers.AddRange(GetXinputControllers());
            return controllers;
        }

        private static List<DirectInputController> GetDirectInputControllers()
        {
            Joystick[] sticks = GamePadHelper.GetSticks(new DirectInput());
            List<DirectInputController> controllers = new List<DirectInputController>();
            foreach (Joystick stick in sticks)
            {
                controllers.Add(new DirectInputController(stick));
            }
            return controllers;
        }
        private static List<XinputController> GetXinputControllers()
        {
            List<XinputController> controllers = new List<XinputController>();
            for (int i = 0; i < 4; i++)
            {
                UserIndex user = (UserIndex)i;
                controllers.Add(new XinputController(user));
            }
            return controllers;
        }
    }
}
