﻿using Newtonsoft.Json;
using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Map : Thing
	{
		public enum Session { None, Single, Multi, MapEdit }

		public static Point TileBarrier => new(0, 22);
		public static Point TilePlayer => new(24, 8);

		public static Size Size => new(1000, 1000);
		public static Point[,,] RawData { get; set; }
		public static Point[,,] DefaultRawData => new Point[(int)Size.W, (int)Size.H, 4];
		public static string CurrentMapName { get; private set; }

		private static Point cameraPosition = new(Size.W / 2, Size.H / 2);
		public static Point CameraPosition
		{
			get { return cameraPosition; }
			set
			{
				cameraPosition = new Point(
					Number.Limit(value.X, new Number.Range(8, RawData.GetLength(0) - 9)),
					Number.Limit(value.Y, new Number.Range(8, RawData.GetLength(1) - 9)));
			}
		}
		public static Session CurrentSession { get; set; }

		public Map(string uniqueID) : base(uniqueID)
		{
			Assets.Event.Subscribe.LoadEnd(uniqueID);
		}
		public override void OnAssetsLoadEnd()
		{
			if (Assets.ValuesAreLoaded("map-data"))
			{
				var cameraPos = Text.FromJSON<Point>(Assets.GetValue("camera-position"));
				var mapData = Text.FromJSON<Point[,,]>(Assets.GetValue("map-data"));
				var mapOffset = Text.FromJSON<Point>(Assets.GetValue("map-offset"));
				if (Assets.ValuesAreLoaded("signs")) Text.FromJSON<List<Sign>>(Assets.GetValue("signs"));
				if (mapData != default && mapOffset != default) InsertMapData(mapData, mapOffset);
				if (cameraPos != default) CameraPosition = cameraPos;

				if (CurrentSession == Session.Single && mapData != default)
				{
					var bottomRight = mapOffset + new Point(mapData.GetLength(0), mapData.GetLength(1));
					var playerTiles = new List<Point>();
					for (int y = (int)mapOffset.Y; y < bottomRight.Y; y++)
						for (int x = (int)mapOffset.X; x < bottomRight.X; x++)
						{
							if (RawData[x, y, 3] == TilePlayer)
							{
								playerTiles.Add(new Point(x, y));
								RawData[x, y, 3] = default;
							}
							else if (RawData[x, y, 3] == TileBarrier)
							{
								var tile = TileBarrier;
								tile.Color = new Color();
								RawData[x, y, 3] = tile;
							}
						}

					var randPoint = new Point(RandX(), RandY());
					if (playerTiles.Count > 0)
						randPoint = playerTiles[(int)Probability.Randomize(new(0, playerTiles.Count - 1))];
					else
						while (RandPointIsBarrier())
							randPoint = new(RandX(), RandY());

					if (Assets.ValuesAreLoaded("map-name") == false)
					{
						var player = new Player("player", new Object.CreationDetails()
						{
							Name = "player",
							Position = randPoint,
							Height = 3,
							TileIndexes = new Point[] { new(25, 0) }
						});
						CameraPosition = player.Position;
					}

					bool RandPointIsBarrier() => RawData[(int)randPoint.X, (int)randPoint.Y, 3] == TileBarrier;
					double RandX() => Probability.Randomize(new(mapOffset.X, bottomRight.X - 1));
					double RandY() => Probability.Randomize(new(mapOffset.Y, bottomRight.Y - 1));
				}
			}
			if (Assets.ValuesAreLoaded("map-name"))
			{
				var player = Text.FromJSON<Player>(Assets.GetValue("player"));
				CameraPosition = player.Position;
			}
			Assets.UnloadAllValues();
			Display();
			Object.DisplayAllObjects();
		}

		public static void CreateUIButtons()
		{
			// these are not creating multiple times cuz they are destroyed beforehand
			new MoveCamera("camera-move-up", new Object.CreationDetails()
			{
				Name = "camera-move-up",
				Position = new(0, 1) { Color = Color.Gray },
				TileIndexes = new Point[] { new(19, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Up };
			new MoveCamera("camera-move-down", new Object.CreationDetails()
			{
				Name = "camera-move-down",
				Position = new(0, 2) { Color = Color.Gray },
				TileIndexes = new Point[] { new(21, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Down };
			new MoveCamera("camera-move-left", new Object.CreationDetails()
			{
				Name = "camera-move-left",
				Position = new(1, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(22, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Left };
			new MoveCamera("camera-move-right", new Object.CreationDetails()
			{
				Name = "camera-move-right",
				Position = new(2, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(20, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Right };

			if (CurrentSession == Session.Single || CurrentSession == Session.Multi)
			{
				new MoveCamera("camera-center", new Object.CreationDetails()
			{
				Name = "camera-center",
				Position = new(0, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(19, 14) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			}) { CurrentType = MoveCamera.Type.Center };
				new EquipSlot("head", new Object.CreationDetails()
				{
					Name = "head",
					Position = new(0, 7) { Color = Color.Gray },
					TileIndexes = new Point[] { new(5, 22) },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("body", new Object.CreationDetails()
				{
					Name = "body",
					Position = new(0, 8) { Color = Color.Gray },
					TileIndexes = new Point[] { new(6, 22) },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("feet", new Object.CreationDetails()
				{
					Name = "feet",
					Position = new(0, 9) { Color = Color.Gray },
					TileIndexes = new Point[] { new(7, 22) },
					Height = 1,
					IsUI = true
				});

				new EquipSlot("hand-left", new Object.CreationDetails()
				{
					Name = "hand-left",
					Position = new(0, 5) { Color = Color.Gray },
					TileIndexes = new Point[] { new(8, 22) },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("hand-right", new Object.CreationDetails()
				{
					Name = "hand-right",
					Position = new(0, 4) { Color = Color.Gray },
					TileIndexes = new Point[] { new(9, 22) },
					Height = 1,
					IsUI = true
				});

				new EquipSlot("carry-back", new Object.CreationDetails()
				{
					Name = "carry-back",
					Position = new(0, 11) { Color = Color.Gray },
					TileIndexes = new Point[] { new(10, 22) },
					Height = 1,
					IsUI = true
				});
				new EquipSlot("carry-waist", new Object.CreationDetails()
				{
					Name = "carry-waist",
					Position = new(0, 12) { Color = Color.Gray },
					TileIndexes = new Point[] { new(11, 22) },
					Height = 1,
					IsUI = true
				});
			}
			if (CurrentSession == Session.MapEdit)
			{
				new SwitchHeight("brush-height", new Object.CreationDetails()
				{
					Name = "height-up",
					Position = new(0, 5),
					Height = 1,
					IsUI = true,
					IsLeftClickable = true,
				});
			}
		}
		public static void DestroyAllSessionObjects()
		{
			if (CurrentSession == Session.None) return;

			var objsToDestroy = new List<Object>();
			foreach (var kvp in Object.objects)
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					if ((kvp.Value[i].IsUI && kvp.Value[i].Position.X > 18) || kvp.Value[i].IsInTab) continue;
					objsToDestroy.Add(kvp.Value[i]);
				}

			for (int i = 0; i < objsToDestroy.Count; i++)
				objsToDestroy[i].Destroy();
		}
		
		public static void Display()
		{
			if (RawData == null) return;
			for (int y = (int)CameraPosition.Y - 8; y < CameraPosition.Y + 9; y++)
				for (int x = (int)CameraPosition.X - 8; x < CameraPosition.X + 9; x++)
					for (int z = 0; z < 4; z++)
						Screen.EditCell(MapToScreenPosition(new(x, y)), RawData[x, y, z], z, RawData[x, y, z].Color);
		}
		public static void DisplayNavigationPanel()
		{
			for (int x = 0; x < 18; x++)
			{
				Screen.EditCell(new(x, 0), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(x, 0), new(1, 22), 0, Color.Brown / 1.3);
			}
			for (int y = 0; y < 18; y++)
			{
				Screen.EditCell(new(0, y), new(4, 22), 1, Color.Brown);
				Screen.EditCell(new(0, y), new(1, 22), 0, Color.Brown / 1.3);
			}
			if (CurrentSession == Session.MapEdit)
			{
				Screen.EditCell(new(0, 4), new(1, 22), 1, Color.White);

				Screen.EditCell(new(11, 0), new(42, 13), 1, Color.Gray);
			}
			else if (CurrentSession == Session.Single)
			{
				Screen.EditCell(new(4, 0), new(4, 23), 1, Color.Gray);
			}
		}
		public static void LoadMap(Session session, string name)
		{
			Assets.Load(Assets.Type.DataSlot, $"Maps\\{name}.mapdata");
			NavigationPanel.Tab.Close();
			NavigationPanel.Info.Textbox.Text = Object.descriptions[new(0, 23)];

			DestroyAllSessionObjects();
			CurrentSession = session;
			CurrentMapName = name;

			DisplayNavigationPanel();
			CreateUIButtons();
		}
		public static void InsertMapData(Point[,,] data, Point offset)
		{
			RawData = DefaultRawData;
			for (int y = 0; y < data.GetLength(1); y++)
				for (int x = 0; x < data.GetLength(0); x++)
					for (int z = 0; z < 4; z++)
						RawData[(int)(x + offset.X), (int)(y + offset.Y), z] = data[x, y, z];
		}

		public static bool IsHovered()
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			return mousePos.X > 0 && mousePos.Y > 0 && mousePos.X < 18 && mousePos.Y < 18;
		}
		public static Point MapToScreenPosition(Point mapPos)
		{
			return mapPos - CameraPosition + new Point(9, 9);
		}
		public static Point ScreenToMapPosition(Point screenPos)
		{
			return CameraPosition + (screenPos - new Point(9, 9));
		}
	}
}
