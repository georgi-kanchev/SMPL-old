﻿using SMPL.Gear;
using SMPL.Data;
using SMPL.Components;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Game.Event.Subscribe.Update(uniqueID);
		}
		public override void OnGameUpdate()
		{
			var mousePosMap = Map.ScreenToMapPosition(Screen.GetCellAtCursorPosition());
			Hoverer.TileIndexes = Map.IsHovered()
					 ? CanMoveIntoCell(mousePosMap) ? new(4, 23) : Hoverer.DefaultTileIndexes
					 : Hoverer.DefaultTileIndexes;

			Hoverer.Area.Angle = 0;
			if (CellIsInLeftReach(mousePosMap)) Hoverer.Area.Angle = 270;
			if (CellIsInRightReach(mousePosMap)) Hoverer.Area.Angle = 90;
			if (CellIsInUpReach(mousePosMap)) Hoverer.Area.Angle = 0;
			if (CellIsInDownReach(mousePosMap)) Hoverer.Area.Angle = 180;
		}
		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Base.LeftClickPosition != mousePos) return;

			var mousePosMap = Map.ScreenToMapPosition(mousePos);
			var movement = new Point(0, 0);

			if (CellIsInLeftReach(mousePosMap)) movement = new Point(-1, 0);
			else if (CellIsInRightReach(mousePosMap)) movement = new Point(1, 0);
			else if (CellIsInUpReach(mousePosMap)) movement = new Point(0, -1);
			else if (CellIsInDownReach(mousePosMap)) movement = new Point(0, 1);

			Move(movement);
		}
	}
}