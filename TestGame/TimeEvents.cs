﻿using SMPL;

namespace TestGame
{
	public class TimeEvents : SMPL.TimeEvents
	{
		public override void OnEachTick()
		{
			Camera.WorldCamera.Zoom -= 0.001;
		}
	}
}
