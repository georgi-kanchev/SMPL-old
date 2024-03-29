﻿using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class ColorSamples : Slider
	{
		public static readonly Color[] Colors = new Color[]
		{ Color.Wood, Color.Wood - 30, Color.Water, Color.LeafSummer, Color.Grass };

		public ColorSamples(string uniqueID, CreationDetails creationDetails, int size, bool isVertical)
			: base(uniqueID, creationDetails, size, isVertical) { IndexValue = -1; }
		public override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $"[LEFT HOLD] Pick a color sample";
		}
		protected override void OnIndexValueChanged()
		{
			if (Colors.Length <= IndexValue) return;
			WorldEditor.Brush = new Point(WorldEditor.Brush.X, WorldEditor.Brush.Y) { Color = Colors[IndexValue] };
			IndexValue = -1;
			Screen.EditCell(new(0, 4), WorldEditor.Brush, 1, WorldEditor.Brush.Color);
			ColorPick.UpdateBrushColorPickers();
		}
		protected override void OnDisplayStep(Point screenPos, int step)
		{
			var tile = Screen.GetCellIndexesAtPosition(screenPos, 1);
			if (Colors.Length <= step) return;
			Screen.EditCell(screenPos, tile, 1, Colors[step]);
		}
	}
}
