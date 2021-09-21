﻿using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public static class Map
	{
		public enum Session { None, Single, Multi, MapEdit }

		public static Point[,] Data { get; set; } = new Point[100, 100];
		private static Point cameraPosition;
		public static Point CameraPosition
		{
			get { return cameraPosition; }
			set
			{
				cameraPosition = new Point(
					Number.Limit(value.X, new Number.Range(8, Data.GetLength(1) - 8)),
					Number.Limit(value.X, new Number.Range(8, Data.GetLength(0) - 8)));
			}
		}
		public static Session CurrentSession { get; set; }

		public static void CreateUIButtons()
		{
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-up",
				Position = new(0, 1) { Color = Color.Gray },
				TileIndexes = new Point[] { new(23, 20) },
				Height = 1,
				IsUI = true
			});
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-down",
				Position = new(0, 2) { Color = Color.Gray },
				TileIndexes = new Point[] { new(25, 20) },
				Height = 1,
				IsUI = true
			});
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-left",
				Position = new(1, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(26, 20) },
				Height = 1,
				IsUI = true
			});
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-move-right",
				Position = new(2, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(24, 20) },
				Height = 1,
				IsUI = true
			});
			new MoveCamera(new Object.CreationDetails()
			{
				Name = "camera-center",
				Position = new(0, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(19, 14) },
				Height = 1,
				IsUI = true
			});
			if (Gate.EnterOnceWhile("game-buttons", CurrentSession == Session.Single || CurrentSession == Session.Multi))
			{
				new SlotHead(new Object.CreationDetails()
				{
					Name = "head",
					Position = new(0, 7) { Color = Color.Gray },
					TileIndexes = new Point[] { new(5, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotBody(new Object.CreationDetails()
				{
					Name = "body",
					Position = new(0, 8) { Color = Color.Gray },
					TileIndexes = new Point[] { new(6, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotFeet(new Object.CreationDetails()
				{
					Name = "feet",
					Position = new(0, 9) { Color = Color.Gray },
					TileIndexes = new Point[] { new(7, 22) },
					Height = 1,
					IsUI = true
				});

				new SlotHandLeft(new Object.CreationDetails()
				{
					Name = "hand-left",
					Position = new(0, 5) { Color = Color.Gray },
					TileIndexes = new Point[] { new(8, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotHandRight(new Object.CreationDetails()
				{
					Name = "hand-right",
					Position = new(0, 4) { Color = Color.Gray },
					TileIndexes = new Point[] { new(9, 22) },
					Height = 1,
					IsUI = true
				});

				new SlotBack(new Object.CreationDetails()
				{
					Name = "carry-back",
					Position = new(0, 11) { Color = Color.Gray },
					TileIndexes = new Point[] { new(10, 22) },
					Height = 1,
					IsUI = true
				});
				new SlotWaist(new Object.CreationDetails()
				{
					Name = "carry-waist",
					Position = new(0, 12) { Color = Color.Gray },
					TileIndexes = new Point[] { new(11, 22) },
					Height = 1,
					IsUI = true
				});
			}
			if (Gate.EnterOnceWhile("map-editor-buttons", CurrentSession == Session.MapEdit))
			{

			}
		}
		public static void DestroyAllSessionObjects()
		{
			if (CurrentSession == Session.None) return;

			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if (kvp.Value[i].IsUI && kvp.Value[i].Position.X > 18) continue;
					objsToDestroy.Add(kvp.Value[i]);
				}

			for (int i = 0; i < objsToDestroy.Count; i++)
				objsToDestroy[i].Destroy();
		}

		public static void Display()
		{
			for (int y = 0; y < Data.GetLength(0); y++)
				for (int x = 0; x < Data.GetLength(1); x++)
				{
					var pos = ScreenToMapPosition(new(x, y));
					if (pos.X > 17 || pos.Y > 17 || pos.X < 0 || pos.Y < 0) continue;
					Screen.EditCell(pos, Data[x, y], 0, Color.Green);
				}
		}
		public static void DisplayNavigationPanel()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.BrownDark);
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.BrownDark);
			}
		}

		public static Point ScreenToMapPosition(Point screenPos)
		{
			return screenPos - CameraPosition + new Point(9, 9);
		}
		public static Point MapToScreenPosition(Point mapPos)
		{
			return mapPos - CameraPosition + new Point(9, 9);
		}
	}
}
