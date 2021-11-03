﻿using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class Key : Item
	{
		public Key(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			MaxQuantity = 6;
			CanCarryOnWaist = true;
			CanCarryInBag = true;
			CanCarryInQuiver = true;
		}

		public override Item OnSplit() => CloneObject(this, $"{UniqueID}-{Performance.FrameCount}");
		public override void OnItemInfoDisplay()
		{
			NavigationPanel.Tab.Texts["item-info"] = $"\t\t\t\tKey x{Quantity}/{MaxQuantity}\n\nGreat for unlocking locked things.";
		}
	}
}