﻿using SMPL.Gear;

namespace RPG1bit
{
	public class SlotHead : Object
	{
		public SlotHead(CreationDetails creationDetails) : base(creationDetails) { }

		protected override void OnDroppedUpon()
		{
			var asd = HoldingObject;
		}
	}
}