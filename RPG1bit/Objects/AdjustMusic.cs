﻿using SMPL.Data;

namespace RPG1bit.Objects
{
	public class AdjustMusic : Object
	{
		public static int Percent { get; set; } = 50;

		public AdjustMusic(CreationDetails creationDetails) : base(creationDetails) { }
		protected override void OnHovered() => ShowClickableIndicator();
		protected override void OnLeftClicked()
		{
			Percent += 10;
			if (Percent == 110) Percent = 0;
			Screen.EditCell(new Point(20, 1), new Point(39, 16), 0,
				new Color(255, 255, 255, Number.Map(Percent, new Number.Range(0, 100), new Number.Range(125, 255))));
			Info.Textbox.Text = $"Music volume: {Percent}%";
		}
	}
}
