using SlimDX.DirectInput;
using System.Collections.Generic;

namespace AutoPCSX2SaveState.Controllers {
	public static class GamePadHelper {
		public static Joystick[] GetSticks(DirectInput input) {
			List<Joystick> sticks = new List<Joystick>(); // Creates the list of joysticks connected to the computer via USB.
			foreach (DeviceInstance device in input.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly)) {
				// Creates a joystick for each game device in USB Ports
				try {
					Joystick stick = new Joystick(input, device.InstanceGuid);
					stick.Acquire();

					// Gets the joysticks properties and sets the range for them.
					foreach (DeviceObjectInstance deviceObject in stick.GetObjects()) {
						if ((deviceObject.ObjectType & ObjectDeviceType.Axis) != 0)
							stick.GetObjectPropertiesById((int)deviceObject.ObjectType).SetRange(-100, 100);
					}

					// Adds how ever many joysticks are connected to the computer into the sticks list.
					sticks.Add(stick);
				} catch (DirectInputException) {
				}
			}
			return sticks.ToArray();
		}
	}
}
