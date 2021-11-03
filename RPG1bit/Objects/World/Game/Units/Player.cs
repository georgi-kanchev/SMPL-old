﻿using SMPL.Gear;
using SMPL.Data;
using Newtonsoft.Json;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Point PreviousPosition { get; set; }

		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			PreviousPosition = Position;
			World.IsShowingRoofs = World.TileHasRoof(Position) == false;
		}
		public override void OnGameUpdate()
		{
			base.OnGameUpdate();

			var mousePosWorld = World.ScreenToWorldPosition(Screen.GetCellAtCursorPosition());
			Hoverer.TileIndexes = Hoverer.DefaultTileIndexes;

			if (World.IsHovered() && CellIsInReach(mousePosWorld))
			{
				if (CanMoveIntoCell(mousePosWorld))
				{
					Hoverer.TileIndexes = new(4, 23);
					Hoverer.Area.Angle = 0;
					if (CellIsInLeftReach(mousePosWorld)) Hoverer.Area.Angle = 270;
					if (CellIsInRightReach(mousePosWorld)) Hoverer.Area.Angle = 90;
					if (CellIsInUpReach(mousePosWorld)) Hoverer.Area.Angle = 0;
					if (CellIsInDownReach(mousePosWorld)) Hoverer.Area.Angle = 180;
				}

				var objs = objects.ContainsKey(mousePosWorld) ? objects[mousePosWorld] : new();
				for (int i = 0; i < objs.Count; i++)
					if (objs[i] is IInteractable)
					{
						Hoverer.TileIndexes = new(27, 14);
						return;
					}
			}
		}
		public override void OnMouseButtonRelease(Mouse.Button button)
		{
			base.OnMouseButtonRelease(button);
			var mousePos = Screen.GetCellAtCursorPosition();
			if (Base.LeftClickPosition != mousePos) return;

			var mousePosWorld = World.ScreenToWorldPosition(mousePos);
			var movement = new Point(0, 0);

			if (CellIsInLeftReach(mousePosWorld)) movement = new Point(-1, 0);
			else if (CellIsInRightReach(mousePosWorld)) movement = new Point(1, 0);
			else if (CellIsInUpReach(mousePosWorld)) movement = new Point(0, -1);
			else if (CellIsInDownReach(mousePosWorld)) movement = new Point(0, 1);

			PreviousPosition = Position;
			if (movement != new Point() && Move(movement))
			{
				AdvanceTime();
				TileIndexes = World.PositionHasWaterAsHighest(Position) ? new(20, 23) : new(25, 0);

				for (int i = 0; i < objects[Position].Count; i++)
					if (objects[Position][i] is Boat boat)
					{
						var tile = boat.GetPlayerTile();
						TileIndexes = new(tile.X, tile.Y) { C = TileIndexes.C };
					}
			}

			if (IsRoof(Position) == false && IsRoof(PreviousPosition))
				World.IsShowingRoofs = true;
			else if (IsRoof(Position) && IsRoof(PreviousPosition) == false)
				World.IsShowingRoofs = false;

			bool IsRoof(Point worldPos)
			{
				for (int i = 0; i < 3; i++)
				{
					var tile = World.RawData.ContainsKey(worldPos) ? World.RawData[worldPos][i] : new();
					if (WorldEditor.Tiles["roof"].Contains(tile))
						return true;
				}
				return false;
			}
		}
	}
}