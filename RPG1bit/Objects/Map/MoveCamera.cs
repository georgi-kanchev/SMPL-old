﻿using SMPL.Data;
using SMPL.Gear;

namespace RPG1bit
{
	public class MoveCamera : Object
	{
		public enum Type { Center, Left, Right, Up, Down }
		public Keyboard.Key Key { get; private set; }
		private Type currentType;
		public Type CurrentType
		{
			get { return currentType; }
			set
			{
				currentType = value;
				switch (CurrentType)
				{
					case Type.Center: Key = Keyboard.Key.Space; break;
					case Type.Left: Key = Keyboard.Key.LeftArrow; break;
					case Type.Right: Key = Keyboard.Key.RightArrow; break;
					case Type.Up: Key = Keyboard.Key.UpArrow; break;
					case Type.Down: Key = Keyboard.Key.DownArrow; break;
				}
			}
		}
		private readonly Point[] directions = new Point[] { new(), new(-1, 0), new(1, 0), new(0, -1), new(0, 1) };

		public MoveCamera(CreationDetails creationDetails) : base(creationDetails)
		{
			Keyboard.CallWhen.KeyPress(OnKeyPressed);
		}

		private void OnKeyPressed(Keyboard.Key key)
		{
			if (Map.CurrentSession == Map.Session.None || key != Key) return;
			Execute();
		}

		protected override void OnHovered()
		{
			NavigationPanel.Info.Textbox.Text = $" [{Key}] Move the view: {CurrentType}.";
		}
		protected override void OnLeftClicked() => Execute();

		private void Execute()
		{
			Map.CameraPosition += directions[(int)CurrentType];
			Map.Display();
		}
	}
}