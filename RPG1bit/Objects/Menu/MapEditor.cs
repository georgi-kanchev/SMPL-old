﻿using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class MapEditor : Object
	{
		public static readonly Dictionary<Point, Point[]> randomTiles = new()
		{
			{ new(01, 22), new Point[] { new(01, 22) } }, // background
			{ new(05, 00), new Point[] { new(04, 00), new(5, 0), new(6, 0), new(7, 0) } }, // grass
			{ new(00, 02), new Point[] { new(00, 02), new(1, 2), new(2, 2), new(3, 2) } }, // bushes
			{ new(15, 03), new Point[] { new(15, 03), new(16, 03) } }, // flowers
			{ new(04, 02), new Point[] { new(04, 02) } }, // rocks
			{ new(05, 01), new Point[] { new(03, 01), new(4, 1), new(5, 1), new(6, 1), new(7, 1) } }, // oak trees
			{ new(01, 01), new Point[] { new(00, 01), new(1, 1), new(2, 1) } }, // pine trees
			{ new(07, 02), new Point[] { new(07, 02) } }, // palm
			{ new(06, 02), new Point[] { new(05, 02), new(6, 2) } }, // cactus

			// water
			{ new(07, 03), new Point[] { new(07, 03) } }, { new(08, 03), new Point[] { new(08, 03) } },
			{ new(09, 03), new Point[] { new(09, 03) } }, { new(10, 03), new Point[] { new(10, 03) } },
			{ new(11, 03), new Point[] { new(11, 03) } }, { new(12, 03), new Point[] { new(12, 03) } },
			{ new(07, 04), new Point[] { new(07, 04) } }, { new(08, 04), new Point[] { new(08, 04) } },
			{ new(09, 04), new Point[] { new(09, 04) } }, { new(10, 04), new Point[] { new(10, 04) } },
			{ new(11, 04), new Point[] { new(11, 04) } }, { new(12, 04), new Point[] { new(12, 04) } },
			{ new(13, 04), new Point[] { new(13, 04) } }, { new(14, 04), new Point[] { new(14, 04) } },
			{ new(07, 05), new Point[] { new(07, 05) } }, { new(08, 05), new Point[] { new(08, 05) } },
			{ new(09, 05), new Point[] { new(09, 05) } }, { new(10, 05), new Point[] { new(10, 05) } },
			{ new(11, 05), new Point[] { new(11, 05) } },

			// bridge
			{ new(12, 05), new Point[] { new(12, 05) } }, { new(13, 05), new Point[] { new(13, 05) } },
			{ new(14, 05), new Point[] { new(14, 05) } }, { new(15, 05), new Point[] { new(15, 05) } },
			{ new(16, 05), new Point[] { new(16, 05) } },

			// dirt road
			{ new(08, 00), new Point[] { new(08, 00) } }, { new(09, 00), new Point[] { new(09, 00) } },
			{ new(10, 00), new Point[] { new(10, 00) } }, { new(11, 00), new Point[] { new(11, 00) } },
			{ new(12, 00), new Point[] { new(12, 00) } }, { new(13, 00), new Point[] { new(13, 00) } },
			{ new(14, 00), new Point[] { new(14, 00) } }, { new(15, 00), new Point[] { new(15, 00) } },
			{ new(09, 01), new Point[] { new(09, 01) } }, { new(10, 01), new Point[] { new(10, 01) } },
			{ new(11, 01), new Point[] { new(11, 01) } }, { new(12, 01), new Point[] { new(12, 01) } },
			{ new(13, 01), new Point[] { new(13, 01) } }, { new(14, 01), new Point[] { new(14, 01) } },

			// stone road
			{ new(15, 01), new Point[] { new(15, 01) } }, { new(16, 01), new Point[] { new(16, 01) } },
			{ new(17, 01), new Point[] { new(17, 01) } }, { new(08, 02), new Point[] { new(08, 02) } },
			{ new(09, 02), new Point[] { new(09, 02) } }, { new(10, 02), new Point[] { new(10, 02) } },
			{ new(11, 02), new Point[] { new(11, 02) } }, { new(12, 02), new Point[] { new(12, 02) } },
			{ new(13, 02), new Point[] { new(13, 02) } }, { new(14, 02), new Point[] { new(14, 02) } },
			{ new(15, 02), new Point[] { new(15, 02) } }, { new(16, 02), new Point[] { new(16, 02) } },
			{ new(17, 02), new Point[] { new(17, 02) } }, { new(13, 03), new Point[] { new(13, 03) } },
			{ new(14, 03), new Point[] { new(14, 03) } },

			// signs
			{ new(01, 07), new Point[] { new(01, 07) } }, { new(00, 07), new Point[] { new(00, 07) } },
			{ new(02, 07), new Point[] { new(02, 07) } },
		};
		private static Point BrushTile
		{
			get
			{
				var tiles = randomTiles[SwitchType.BrushType];
				var randomTileIndex = (int)Probability.Randomize(new Number.Range(0, tiles.Length - 1));
				var result = tiles[randomTileIndex];
				result.Color = SwitchColor.BrushColor;
				return result;
			}
		}

		public MapEditor(string uniqueID, CreationDetails creationDetails) : base(uniqueID, creationDetails) { }

		public override void OnHovered() => NavigationPanel.Info.Textbox.Text = "Start a new map edit session.";
		public override void OnLeftClicked()
		{
			Map.DestroyAllSessionObjects();
			Map.CurrentSession = Map.Session.MapEdit;
			Map.DisplayNavigationPanel();

			Map.RawData = Map.DefaultRawData;

			Map.CreateUIButtons();
			Map.Display(); // for the map iteself
			DisplayAllObjects(); // for the ui
		}
		public static Point[,,] GetSavableMapData(Point[,,] rawData, out Point offset)
		{
			var lastTilePos = new Point(-1, -1);
			var firstTilePos = new Point(rawData.GetLength(0), rawData.GetLength(1));

			for (int y = 0; y < rawData.GetLength(1); y++)
				for (int x = 0; x < rawData.GetLength(0); x++)
					for (int z = 0; z < 4; z++)
					{
						if (rawData[x, y, z] != new Point(0, 0))
						{
							if (x < firstTilePos.X) firstTilePos.X = x;
							if (y < firstTilePos.Y) firstTilePos.Y = y;
							if (lastTilePos.X < x) lastTilePos.X = x;
							if (lastTilePos.Y < y) lastTilePos.Y = y;
						}
					}

			if (lastTilePos.X == -1 || lastTilePos.Y == -1)
			{
				offset = default;
				return default;
			}
			var result = new Point[(int)(lastTilePos.X - firstTilePos.X + 1), (int)(lastTilePos.Y - firstTilePos.Y + 1), 4];
			for (int y = 0; y < result.GetLength(1); y++)
				for (int x = 0; x < result.GetLength(0); x++)
					for (int z = 0; z < 4; z++)
						result[x, y, z] = rawData[(int)(x + firstTilePos.X), (int)(y + firstTilePos.Y), z];
			offset = firstTilePos;
			return result;
		}

		public static void EditCurrentTile()
		{
			var LMB = Mouse.ButtonIsPressed(Mouse.Button.Left);
			var clickPos = Map.ScreenToMapPosition(LMB ? Base.LeftClickPosition : Base.RightClickPosition);
			var mousePos = Screen.GetCellAtCursorPosition();
			var pos = Map.ScreenToMapPosition(mousePos);
			var brushTile = BrushTile;
			var hoveredTile = Screen.GetCellIndexesAtPosition(mousePos, SwitchHeight.BrushHeight);

			// sign hardcoding
			if (TileIsSign(hoveredTile))
			{
				// overwrite / delete
				var heights = objects.ContainsKey(pos) ? objects[pos] : new();
				for (int i = 0; i < heights.Count; i++)
					if (heights[i].IsUI == false && (heights[i].Height == SwitchHeight.BrushHeight || heights[i] is Sign))
						heights[i].Destroy();
			}
			if (TileIsSign(brushTile) && LMB)
			{
				var sign = new Sign($"{pos}", new CreationDetails()
				{
					Name = "Sign",
					Position = new(pos.X, pos.Y) { Color = SwitchColor.BrushColor },
					Height = SwitchHeight.BrushHeight,
					TileIndexes = new Point[] { brushTile },
				});
				sign.OnHovered();
			}

			if (TileIsSign(brushTile) == false)
			{
				if (Keyboard.KeyIsPressed(Keyboard.Key.LeftShift) && clickPos != pos)
				{
					var dirY = pos.Y > clickPos.Y ? 1 : -1;
					var dirX = pos.X > clickPos.X ? 1 : -1;
					for (double y = clickPos.Y; y != pos.Y + dirY; y += dirY)
						for (double x = clickPos.X; x != pos.X + dirX; x += dirX)
							EditTile();
				}
				EditTile();
			}

			NavigationPanel.Tab.Close();
			Map.Display();
			DisplayAllObjects();

			void EditTile() => Map.RawData[(int)pos.X, (int)pos.Y, SwitchHeight.BrushHeight] = LMB ? brushTile : new(0, 0);
			bool TileIsSign(Point p) => p.Y == 7 && (p.X == 0 || p.X == 1 || p.X == 2);
		}
		public static void PickCurrentTile()
		{
			var indexes = Screen.GetCellIndexesAtPosition(Screen.GetCellAtCursorPosition(), SwitchHeight.BrushHeight);
			if (indexes == new Point(0, 0)) return;
			SwitchColor.SelectHoveredTileColor();
			SwitchColor.UpdateIndicator();
			SwitchType.SelectHoveredTileType();
			SwitchType.UpdateIndicator();
		}
		public static void EditSpecialTile(Point tileIndexes)
		{
			var pos = Map.ScreenToMapPosition(Screen.GetCellAtCursorPosition());
			var p = Map.RawData[(int)pos.X, (int)pos.Y, 3] == tileIndexes ? new(0, 0) : tileIndexes;
			Map.RawData[(int)pos.X, (int)pos.Y, 3] = p;
			Map.Display();
			NavigationPanel.Tab.Close();
		}
	}
}
