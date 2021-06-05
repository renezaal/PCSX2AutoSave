// <copyright file="IController.cs" company="Epiphaner">
// Copyright (c) Epiphaner. All rights reserved.
// </copyright>

namespace AutoPCSX2SaveState.Controllers
{
	internal interface IController
	{
		double GetIdleTime();

		void Poll();

		void Dispose();
	}
}
