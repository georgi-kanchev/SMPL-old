﻿using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using System.Collections.Generic;

namespace RPG1bit
{
	public class Object
	{
		public struct CreationDetails
		{
			public string Name { get; set; }
			public Point[] TileIndexes { get; set; }
			public Point Position { get; set; }
			public int Height { get; set; }
			public bool IsDragable { get; set; }
			public bool IsLeftClickable { get; set; }
			public bool IsRightClickable { get; set; }
			public bool IsConfirmingClick { get; set; }
			public bool IsUI { get; set; }
			public bool IsInTab { get; set; }
			public NavigationPanel.Tab.Type AppearOnTab { get; set; }
		}

		public static readonly Dictionary<Point, List<Object>> objects = new();
		public static readonly Dictionary<Point, string> descriptions = new()
		{
			{ new(33, 15), "Graphics 1-Bit Pack by kenney.nl\n" +
				"Font DPComic by cody@zone38.net\n" +
				"Music by opengameart.org/users/yubatake\n" +
				"Music by opengameart.org/users/avgvsta\n" +
				$"Game {NavigationPanel.Info.GameVersion} & SFX(software: Bfxr) by dodo" },
			{ new(01, 22), "" }, // background color
			{ new(00, 00), "Void." },
			{ new(04, 22), "Game navigation panel." },
			{ new(00, 23), "Game navigation panel." },
			{ new(29, 15), "Game navigation panel." },
			{ new(30, 15), "Game navigation panel." },
			{ new(02, 22), "Game navigation panel." },
			{ new(03, 22), "Game navigation panel." },
			{ new(01, 23), "Information box." },

			{ new(13, 22), "Left click and type anything..." },

			{ new(44, 16), "Start a new multiplayer game session.\n      (not available in this version)" },

			{ new(05, 22), "On your head:" },
			{ new(06, 22), "On your body:" },
			{ new(07, 22), "On your feet:" },
			{ new(08, 22), "In your left hand:" },
			{ new(09, 22), "In your right hand:" },
			{ new(10, 22), "On your back:" },
			{ new(11, 22), "On your waist:" },

			{ new(37, 18), "Change the brush color." },
			{ new(41, 19), "Change the brush type." },
			{ new(42, 18), "Change the brush height." },
			{ new(36, 17), "Change the brush height." },
			{ new(37, 17), "Change the brush height." },
			{ new(38, 17), "Change the brush height." },
			{ new(41, 13), "[LMB] Paint a tile.\n[Shift + LMB] Paint a square of tiles." },
			{ new(42, 13), "[RMB] Erase a tile.\n[Shift + RMB] Erase a square of tiles." },
			{ new(43, 13), "[MMB] Pick a brush from the tile\n    at that particular height." },

			{ new(04, 00), "Grass." }, { new(05, 00), "Grass." }, { new(06, 00), "Grass." }, { new(07, 00), "Grass." },
			{ new(00, 01), "Tree." }, { new(1, 1), "Tree." }, { new(2, 1), "Trees." }, { new(3, 1), "Trees." }, { new(4, 1), "Tree." },
			{ new(05, 01), "Tree." }, { new(6, 1), "Tree." }, { new(7, 1), "Tree." },
			{ new(05, 2), "Cactuses." }, { new(6, 2), "Cactus." },
			{ new(07, 02), "Palm." },
			{ new(04, 02), "Rocks." },
			{ new(00, 02), "Bush." }, { new(1, 2), "Bush." }, { new(2, 2), "Bush." }, { new(3, 2), "Dead bush." },

			{ new(32, 00), "Helmet." },
		};
		public static Object HoldingObject { get; set; }

		public string Name { get; }
		public Point TileIndexes { get; }
		public int Height { get; }
		public bool IsDragable { get; }
		public bool IsLeftClickable { get; }
		public bool IsRightClickable { get; }
		public bool IsConfirmingClick { get; }
		public bool IsUI { get; }
		public bool IsInTab { get; }
		public NavigationPanel.Tab.Type AppearOnTab { get; }

		private bool leftClicked;
		private static Point prevCursorPos = new(-1, 0);
		public static Point LeftClickPosition { get; private set; }
		public static Point RightClickPosition { get; private set; }
		private Point position;
		public Point Position
		{
			get { return position; }
			set
			{
				position = value;
				if (objects.ContainsKey(value) == false)
				{
					objects.Add(value, new List<Object>() { this });
					return;
				}
				objects[position].Remove(this);
				objects[value].Add(this);
			}
		}

		public static void Initialize()
		{
			Game.CallWhen.Running(StaticAlways, 0);
			Mouse.CallWhen.ButtonPress(OnButtonClicked);
		}
		public Object(CreationDetails creationDetails)
		{
			Mouse.CallWhen.ButtonRelease(OnButtonRelease);
			Game.CallWhen.Running(Always);

			Name = creationDetails.Name;
			TileIndexes = creationDetails.TileIndexes.Length == 0 ? creationDetails.TileIndexes[0] :
				creationDetails.TileIndexes[(int)Probability.Randomize(new(0, creationDetails.TileIndexes.Length - 1))];
			Position = creationDetails.Position;
			Height = creationDetails.Height;
			IsDragable = creationDetails.IsDragable;
			IsLeftClickable = creationDetails.IsLeftClickable;
			IsConfirmingClick = creationDetails.IsConfirmingClick;
			IsUI = creationDetails.IsUI;
			AppearOnTab = creationDetails.AppearOnTab;
			IsInTab = creationDetails.IsInTab;
		}
		public void Destroy()
		{
			objects[Position].Remove(this);
		}
		public static void DisplayAllObjects()
		{
			foreach (var kvp in objects)
				for (int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Display();
		}
		public static List<Object> PickByPosition(Point position)
		{
			return objects.ContainsKey(position) ? objects[position] : new List<Object>();
		}

		private void Always()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var cursorPos = Screen.GetCellAtCursorPosition();
			if (Gate.EnterOnceWhile($"on-hover-{Position}", cursorPos == Position))
			{
				var quad = Screen.Sprite.GetQuad($"{Height} cell {cursorPos.X} {cursorPos.Y}");
				var coord = quad.CornerA.TextureCoordinate;
				var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);

				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				var key = tileIndex.IsInvalid() ? new(0, 0) : tileIndex;
				NavigationPanel.Info.Textbox.Text = descriptions.ContainsKey(key) ? descriptions[key] : "";
				NavigationPanel.Info.ShowClickableIndicator(IsLeftClickable);
				NavigationPanel.Info.ShowDragableIndicator(IsDragable);
				NavigationPanel.Info.ShowLeftClickableIndicator(IsLeftClickable || IsDragable);
				leftClicked = false;
				OnHovered();
			}
			if (IsDragable &&
				Gate.EnterOnceWhile($"on-unhover-{Position}", cursorPos != Position &&
				Mouse.ButtonIsPressed(Mouse.Button.Left) && Position == LeftClickPosition))
			{
				HoldingObject = this;
				Hoverer.CursorTextureTileIndexes = TileIndexes;
				Hoverer.CursorTextureColor = Position.Color;
				OnDragStart();
			}
		}
		private static void StaticAlways()
		{
			if (Screen.Sprite == null || NavigationPanel.Info.Textbox == null || Window.CurrentState == Window.State.Minimized) return;
			var mousePos = Screen.GetCellAtCursorPosition();

			if (Map.CurrentSession == Map.Session.None && mousePos.X < 18)
			{
				NavigationPanel.Info.Textbox.Scale = new(0.6, 0.6);
				NavigationPanel.Info.Textbox.Text = $"{Window.Title} {NavigationPanel.Info.GameVersion}";
				return;
			}
			if (mousePos != prevCursorPos)
			{
				var isOverMap = mousePos.X < 18 && mousePos.Y < 18 && mousePos.X > 0 && mousePos.Y > 0;
				if (Map.CurrentSession == Map.Session.MapEdit && isOverMap &&
					(Mouse.ButtonIsPressed(Mouse.Button.Left) || Mouse.ButtonIsPressed(Mouse.Button.Right)))
					MapEditor.EditCurrentTile();

				NavigationPanel.Info.Textbox.Text = "";
				NavigationPanel.Info.Textbox.Scale = new(0.35, 0.35);
				NavigationPanel.Info.ShowClickableIndicator(false);
				NavigationPanel.Info.ShowDragableIndicator(false);
				NavigationPanel.Info.ShowLeftClickableIndicator(false);

				for (int i = 0; i < 3; i++)
				{
					var quadID = $"{i} cell {mousePos.X} {mousePos.Y}";
					var quad = Screen.Sprite.GetQuad(quadID);
					var coord = quad.CornerA.TextureCoordinate;
					var tileIndex = coord / new Point(quad.TileSize.W + quad.TileGridWidth.W, quad.TileSize.H + quad.TileGridWidth.H);
					var key = tileIndex.IsInvalid() ? new(0, 0) : tileIndex;
					var description = descriptions.ContainsKey(key) ? descriptions[key] : "";
					var sep = i != 0 && description != "" && NavigationPanel.Info.Textbox.Text != "" ? "\n" : "";

					if (NavigationPanel.Info.Textbox.Text != "" && description == descriptions[new(0, 0)]) break;
					NavigationPanel.Info.Textbox.Text = $"{description}{sep}{NavigationPanel.Info.Textbox.Text}";
				}
			}
			prevCursorPos = mousePos;
		}

		private void OnButtonRelease(Mouse.Button button)
		{
			if (IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType) return;

			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left)
			{
				if (IsLeftClickable && Position == mousePos && Position == LeftClickPosition)
				{
					if (IsConfirmingClick && Map.CurrentSession != Map.Session.None && leftClicked == false)
					{
						leftClicked = true;
						NavigationPanel.Info.Textbox.Text = "A session is currently ongoing.\n" +
							"Any unsaved progress will be lost.\n" +
							"Left click again to continue.";
					}
					else
					{
						leftClicked = false;
						OnHovered();
						OnLeftClicked();
					}
				}
				if (HoldingObject != null)
				{
					if (objects.ContainsKey(mousePos))
						for (int i = 0; i < objects[mousePos].Count; i++)
							objects[mousePos][i].OnDroppedUpon();
					HoldingObject.OnDragEnd();
					HoldingObject = null;
					Hoverer.CursorTextureTileIndexes = new(36, 10);
				}
			}
			if (button == Mouse.Button.Right && IsRightClickable && Position == mousePos && Position == RightClickPosition)
			{
				OnHovered();
				OnRightClicked();
			}
		}
		private static void OnButtonClicked(Mouse.Button button)
		{
			var mousePos = Screen.GetCellAtCursorPosition();
			if (button == Mouse.Button.Left) LeftClickPosition = mousePos;
			if (button == Mouse.Button.Right) RightClickPosition = mousePos;

			var isOverMap = mousePos.X < 18 && mousePos.Y < 18 && mousePos.X > 0 && mousePos.Y > 0;
			if (Map.CurrentSession != Map.Session.MapEdit || isOverMap == false) return;
			if (button == Mouse.Button.Left || button == Mouse.Button.Right) MapEditor.EditCurrentTile();
			if (button == Mouse.Button.Middle) MapEditor.PickCurrentTile();
		}
		public void Display()
		{
			var scrPos = Map.MapToScreenPosition(Position);
			var isHiddenUI = IsInTab && AppearOnTab != NavigationPanel.Tab.CurrentTabType;
			if (Position.X >= 0 && Position.X <= 31 && Position.Y >= 0 && Position.Y <= 18)
				Screen.EditCell(IsUI ? Position : scrPos, TileIndexes, Height, isHiddenUI ? new Color() : Position.Color);
			OnDisplay();
		}

		public virtual void OnLeftClicked() { }
		public virtual void OnRightClicked() { }
		public virtual void OnHovered() { }
		public virtual void OnDragStart() { }
		public virtual void OnDragEnd() { }
		public virtual void OnDroppedUpon() { }
		public virtual void OnDisplay() { }
	}
}
