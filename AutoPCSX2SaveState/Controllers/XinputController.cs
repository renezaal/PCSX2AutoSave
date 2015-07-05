using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPCSX2SaveState.Controllers
{
    class XinputController : IController
    {
        private UserIndex user;
        private Controller controller;
        private DateTime lastActivity;
        private uint lastPacket;
        private ulong lastButtonsState;
        private byte lastRightTriggerState;
        private byte lastLeftTriggerState;


        public XinputController(UserIndex user)
        {
            if (((int)user) < 0 || ((int)user) > 3)
            {
                throw new ArgumentOutOfRangeException("user");
            }

            this.user = user;
            controller = new Controller(user);
            lastActivity = DateTime.Now;
            lastPacket = 0;
            lastButtonsState = 0;
            lastRightTriggerState = 0;
            lastLeftTriggerState = 0;
        }


        public double GetIdleTime()
        {
            return (DateTime.Now - lastActivity).TotalSeconds;
        }


        public void Poll()
        {
            if (!controller.IsConnected)
            {
                return;
            }

            State state = controller.GetState();

            if (state.PacketNumber == lastPacket)
            {
                return;
            }
            lastPacket = state.PacketNumber;

            ulong buttonsState = (ulong)state.Gamepad.Buttons;

            if (buttonsState != lastButtonsState)
            {
                lastActivity = DateTime.Now;
                lastButtonsState = buttonsState;
                return;
            }

            byte leftTriggerState = state.Gamepad.LeftTrigger;
            if (Math.Abs(leftTriggerState - lastLeftTriggerState) > Gamepad.GamepadTriggerThreshold)
            {
                lastActivity = DateTime.Now;
                return;
            }
            lastLeftTriggerState = leftTriggerState;


            byte rightTriggerState = state.Gamepad.RightTrigger;
            if (Math.Abs(rightTriggerState - lastRightTriggerState) > Gamepad.GamepadTriggerThreshold)
            {
                lastActivity = DateTime.Now;
                return;
            }
            lastRightTriggerState = rightTriggerState;
        }
    }
}
