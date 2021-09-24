﻿using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public static class Screen
	{
		public static Sprite Sprite { get; set; }
		public static Area Area { get; set; }

		private static void CreateAndInitializeScreen()
		{
			Camera.CallWhen.Display(OnDisplay);

			Area = new("map-area");
			Area.Size = Camera.WorldCamera.Size;

			Sprite = new("map-sprite");
			Sprite.AreaUniqueID = Area.UniqueID;
			Sprite.TexturePath = "Assets\\graphics.png";
			Sprite.SetQuadGrid("cell", new(32, 18), new(16, 16));
			Sprite.RemoveQuad("map-sprite");

			for (int y = 0; y < 18; y++)
				for (int x = 0; x < 32; x++)
				{
					var quadID = $"cell {x} {y}";
					var quad = Sprite.GetQuad(quadID);
					var c = new Color();

					Sprite.RemoveQuad(quadID);

					quad.TileGridWidth = new Size(1, 1);
					quad.SetTextureCropTile(new(0, 0));
					quad.SetColor(c, c, c, c);
					Sprite.SetQuad($"0 {quadID}", quad);
					Sprite.SetQuad($"1 {quadID}", quad);
					Sprite.SetQuad($"2 {quadID}", quad);
				}
		}
		public static void Create()
		{
			CreateAndInitializeScreen();

			NavigationPanel.CreateButtons();
			NavigationPanel.Info.Create();
			Object.Initialize();
			Hoverer.Create();
		}

		public static Point GetCellAtCursorPosition()
		{
			var size = new Point(Camera.WorldCamera.Size.W, Camera.WorldCamera.Size.H);
			var topRight = Camera.WorldCamera.Position - size / 2;
			var botRight = Camera.WorldCamera.Position + size / 2;
			var x = Number.Map(Mouse.Cursor.PositionWindow.X, new(topRight.X, botRight.X), new(0, 32));
			var y = Number.Map(Mouse.Cursor.PositionWindow.Y, new(topRight.Y, botRight.Y), new(0, 18));
			return new Point((int)x, (int)y);
		}
		public static Point GetCellIndexesAtPosition(Point position, int depth)
		{
			var id = $"{depth} cell {position.X} {position.Y}";
			var coord = Sprite.GetQuad(id).CornerA.TextureCoordinate;
			return new(coord.X / 17, coord.Y / 17);
		}
		public static void EditCell(Point cellIndexes, Point tileIndexes, int depth, Color color)
		{
			var id = $"{depth} cell {cellIndexes.X} {cellIndexes.Y}";
			var q = Sprite.GetQuad(id);
			q.SetTextureCropTile(tileIndexes);
			q.SetColor(color, color, color, color);
			Sprite.SetQuad(id, q);
		}

		private static void OnDisplay(Camera camera)
		{
			if (Sprite == null) return;
			Sprite.Display(camera);
		}
	}
}