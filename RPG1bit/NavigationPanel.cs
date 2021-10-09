﻿using SMPL.Components;
using SMPL.Data;
using SMPL.Gear;
using static RPG1bit.Object;

namespace RPG1bit
{
	public class NavigationPanel : Thing
	{
		public class Tab : Thing
		{
			public static Textbox Textbox { get; set; }
			public static Area Area { get; set; }
			public static Effects Effects { get; set; }

			public enum Type { None, Single, Multi, SaveLoad }
			public static Type CurrentTabType { get; set; }
			public static string Title { get; set; }

			public Tab(string uniqueID) : base(uniqueID)
			{
				Camera.Event.Subscribe.Display(uniqueID);

				Area = new("tab-area");
				Effects = new("tab-effects");
				Effects.OutlineWidth = 10;
				Effects.BackgroundColor = new();

				var sz = Camera.WorldCamera.Size / 2;
				Area.Position = new Point(60 * 25.5, 60 * 7.9) - new Point(sz.W, sz.H);
				Area.Size = new(60 * 13, 60 * 12);

				Textbox = new("tab-textbox");
				Textbox.AreaUniqueID = "tab-area";
				Textbox.EffectsUniqueID = "tab-effects";
				Textbox.FontPath = "Assets\\font.ttf";
				Textbox.CharacterSize = 128;
				Textbox.Spacing = new(4, 0.5);
				Textbox.Scale = new(0.4, 0.4);
				Textbox.OriginPercent = new(50, 0);
				Textbox.Text = "";
			}

			public static void Close()
			{
				CurrentTabType = Type.None;
				Title = "";
				Textbox.Text = "";
				Display();
				DisplayAllObjects();
			}
			public static void Open(Type type, string title)
			{
				Close();
				CurrentTabType = type;
				Title = title;
			}

			public override void OnCameraDisplay(Camera camera)
			{
				if (Textbox == null) return;
				Textbox.Display(camera);
			}
		}
		public class Info : Thing
		{
			public const string GameVersion = "v0.1";
			public static Textbox Textbox { get; set; }
			public static Area Area { get; set; }
			public static Effects Effects { get; set; }

			private static bool clickable, leftClickable, rightClickable, dragable;

			public Info(string uniqueID) : base(uniqueID)
			{
				Camera.Event.Subscribe.Display(uniqueID);

				Area = new("info-area");
				Effects = new("info-effects");
				Effects.OutlineWidth = 10;

				var sz = Camera.WorldCamera.Size / 2;
				Area.Position = new Point(60 * 25.2, 60 * 16.6) - new Point(sz.W, sz.H);
				Area.Size = new(60 * 12.3, 60 * 4);

				Textbox = new("info-textbox");
				Textbox.AreaUniqueID = "info-area";
				Textbox.EffectsUniqueID = "info-effects";
				Textbox.FontPath = "Assets\\font.ttf";
				Textbox.CharacterSize = 128;
				Textbox.Spacing = new(4, -6);
				Textbox.Scale = new(0.35, 0.35);
				Textbox.Text = "";
			}
			public static void Display()
			{
				for (int y = 15; y < 18; y++)
					for (int x = 19; x < 32; x++)
						Screen.EditCell(new Point(x, y), new Point(1, 23), 1, new Color());
				ShowClickableIndicator(clickable);
				ShowDragableIndicator(dragable);
				ShowLeftClickableIndicator(leftClickable);
				ShowRightClickableIndicator(rightClickable);
			}

			public override void OnCameraDisplay(Camera camera)
			{
				if (Textbox == null) return;
				Textbox.Display(camera);
			}

			public static void ShowClickableIndicator(bool show = true)
			{
				clickable = show;
				Screen.EditCell(new Point(31, 16), new Point(2, 22), 1, show ? Color.White : new Color());
			}
			public static void ShowDragableIndicator(bool show = true)
			{
				if (clickable && show == false) return;
				dragable = show;
				var both = clickable && dragable ? new Point(2, 23) : new Point(3, 22);
				Screen.EditCell(new Point(31, 16), both, 1, show ? Color.White : new Color());
			}
			public static void ShowLeftClickableIndicator(bool show = true)
			{
				leftClickable = show;
				Screen.EditCell(new Point(31, 15), new Point(29, 15), 1, show ? Color.White : new Color());
			}
			public static void ShowRightClickableIndicator(bool show = true)
			{
				rightClickable = show;
				Screen.EditCell(new Point(31, 17), new Point(30, 15), 1, show ? Color.White : new Color());
			}
		}

		public NavigationPanel(string uniqueID) : base(uniqueID)
		{
			new StartSingle("start-singleplayer", new CreationDetails()
			{
				Name = "start-singleplayer",
				Position = new(19, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(43, 16) },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
			});
			new StartMulti("start-multiplayer", new CreationDetails()
			{
				Name = "start-multiplayer",
				Position = new(20, 0) { Color = Color.GrayDark },
				TileIndexes = new Point[] { new(44, 16) },
				Height = 1,
				IsUI = true,
			});
			new SaveLoad("save-load", new CreationDetails()
			{
				Name = "save-load",
				Position = new(23, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(42, 16) },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
			});
			new MapEditor("map-editor", new CreationDetails()
			{
				Name = "map-editor",
				Position = new(21, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(47, 06) },
				Height = 1,
				IsUI = true,
				IsConfirmingClick = true,
				IsLeftClickable = true
			});

			new AdjustVolume("adjust-sound-volume", new CreationDetails()
			{
				Name = "Sound effects",
				Position = new(26, 0) { Color = new Color(175, 175, 175) },
				TileIndexes = new Point[] { new(38, 16) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});
			new AdjustVolume("adjust-music-volume", new CreationDetails()
			{
				Name = "Music",
				Position = new(27, 0) { Color = new Color(175, 175, 175) },
				TileIndexes = new Point[] { new(39, 16) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});

			new MinimizeGame("-", new CreationDetails()
			{
				Name = "-",
				Position = new(30, 0) { Color = Color.Gray },
				TileIndexes = new Point[] { new(37, 20) },
				Height = 1,
				IsUI = true,
				IsLeftClickable = true,
			});
			new ExitGame("x", new CreationDetails()
			{
				Name = "x",
				Position = new(31, 0) { Color = Color.RedDark },
				TileIndexes = new Point[] { new(40, 13) },
				Height = 1,
				IsLeftClickable = true,
				IsUI = true,
				IsConfirmingClick = true,
			});
		}
		public static void Display()
		{
			for (int y = 0; y < 18; y++)
				for (int x = 18; x < 32; x++)
				{
					Screen.EditCell(new Point(x, y), new Point(1, 22), 0, Color.BrownDark);
					Screen.EditCell(new Point(x, y), new Point(0, 0), 1, new());
				}

			for (int x = 18; x < 32; x++)
			{
				Screen.EditCell(new Point(x, 1), new Point(4, 22), 0, Color.Brown);
				Screen.EditCell(new Point(x, 14), new Point(4, 22), 0, Color.Brown);
			}
			for (int y = 0; y < 18; y++)
				Screen.EditCell(new Point(18, y), new Point(4, 22), 0, Color.Brown);

			Screen.DisplayText(new(19, 1), 1, Color.GrayLight, Tab.Title);
			Screen.EditCell(new Point(28, 0), new Point(33, 15), 1, Color.Gray);
			Screen.EditCell(new Point(22, 0), new Point(4, 22), 0, Color.Brown);
			Screen.EditCell(new Point(29, 0), new Point(4, 22), 0, Color.Brown);

			Info.Display();
		}
	}
}
