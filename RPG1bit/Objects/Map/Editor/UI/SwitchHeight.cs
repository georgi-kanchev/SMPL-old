﻿using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class SwitchHeight : Object
	{
		public static int BrushHeight { get; private set; }

		public SwitchHeight(CreationDetails creationDetails) : base(creationDetails)
		{
			Screen.EditCell(new(0, 16), new(36, 17), 1, Color.Gray);
		}
		protected override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Change the brush height.";
		protected override void OnLeftClicked()
		{
			BrushHeight += Name == "height-up" ? 1 : -1;
			if (BrushHeight == 3) BrushHeight = 0;
			else if (BrushHeight == -1) BrushHeight = 2;

			Screen.EditCell(new(0, 16), new(36 + BrushHeight, 17), 1, Color.Gray);
		}
	}
}