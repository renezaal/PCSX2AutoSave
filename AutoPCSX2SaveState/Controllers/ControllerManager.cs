// <copyright file="ControllerManager.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>
namespace AutoPCSX2SaveState.Controllers
{
	using System.Collections.Generic;

	internal static class ControllerManager
	{
		public static List<IController> GetControllers()
		{
			List<IController> controllers = new List<IController>();

			controllers.AddRange(GetXinputControllers());
			return controllers;
		}

		private static List<XinputController> GetXinputControllers()
		{
			List<XinputController> controllers = new List<XinputController>();
			for (int i = 0; i < 4; i++)
			{
				controllers.Add(new XinputController(i));
			}

			return controllers;
		}
	}
}
