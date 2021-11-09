﻿using SMPL.Gear;
using SMPL.Data;
using Newtonsoft.Json;

namespace RPG1bit
{
	public class Player : Unit
	{
		public Player(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails)
		{
			Keyboard.Event.Subscribe.KeyPress(uniqueID);

			World.IsShowingRoofs = World.TileHasRoof(Position) == false;
		}
		public override void OnKeyboardKeyPress(Keyboard.Key key)
		{
			var dir = new Point();
			if (key == Keyboard.Key.LeftArrow) dir = new(-1, 0);
			else if (key == Keyboard.Key.RightArrow) dir = new(1, 0);
			else if (key == Keyboard.Key.UpArrow) dir = new(0, -1);
			else if (key == Keyboard.Key.DownArrow) dir = new(0, 1);

			TryMove(dir);
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
			if (Health[0] <= 0 || button != Mouse.Button.Left)
				return;

			var mousePos = Screen.GetCellAtCursorPosition();
			if (Base.LeftClickPosition != mousePos)
				return;

			var mousePosWorld = World.ScreenToWorldPosition(mousePos);
			var movement = new Point(0, 0);

			if (CellIsInLeftReach(mousePosWorld)) movement = new Point(-1, 0);
			else if (CellIsInRightReach(mousePosWorld)) movement = new Point(1, 0);
			else if (CellIsInUpReach(mousePosWorld)) movement = new Point(0, -1);
			else if (CellIsInDownReach(mousePosWorld)) movement = new Point(0, 1);

			TryMove(movement);
		}
		private void TryMove(Point movement)
		{
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
			UpdateRoofs();
		}
		public void UpdateRoofs()
		{
			if (World.TileHasRoof(Position) == false && World.TileHasRoof(PreviousPosition))
				World.IsShowingRoofs = true;
			else if (World.TileHasRoof(Position) && World.TileHasRoof(PreviousPosition) == false)
				World.IsShowingRoofs = false;
		}
	}
}
