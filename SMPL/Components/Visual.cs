﻿using SFML.Graphics;
using System;
using SMPL.Data;
using SMPL.Gear;

namespace SMPL.Components
{
	public abstract class Visual : Access
	{
		protected readonly uint creationFrame;
		protected readonly double rand;
		internal Area transform;
		internal Visual masking;

		private Effects effects;
		public Effects Effects
		{
			get { return AllAccess == Extent.Removed ? default : effects; }
			set
			{
				if (effects == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = effects;
				effects = value;
				if (Debug.CalledBySMPL) return;
				if (this is TextBox) TextBox.TriggerOnEffectsChange(this as TextBox, prev);
				else Sprite.TriggerOnEffectsChange(this as Sprite, prev);
			}
		}
		private Family family;
		public Family Family
		{
			get { return AllAccess == Extent.Removed ? default : family; }
			set
			{
				if (family == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				var prev = family;
				family = value;
				transform.family = family;
				if (Debug.CalledBySMPL) return;
				if (this is TextBox) TextBox.TriggerOnFamilyChange(this as TextBox, prev);
				else Sprite.TriggerOnFamilyChange(this as Sprite, prev);
			}
		}

		private bool isHidden;
		public bool IsHidden
		{
			get { return AllAccess != Extent.Removed && isHidden; }
			set
			{
				if (isHidden == value || (Debug.CalledBySMPL == false && IsCurrentlyAccessible() == false)) return;
				isHidden = value;
				if (this is TextBox) TextBox.TriggerOnVisibilityChange(this as TextBox);
				else Sprite.TriggerOnVisibilityChange(this as Sprite);
			}
		}

		internal void Dispose()
		{
			if (Effects != null)
			{
				if (Effects.shader != null) Effects.shader.Dispose();
				Effects.owner = null;
				Effects = null;
			}
			if (Family != null)
			{
				if (Family.Parent != null) Family.Parent.Family.children.Remove(this);
				Family.UnparentAllChildren();
				Family.owner = null;
			}
			if (masking != null && masking.Effects != null) masking.Effects.masks.Remove(this);
			AllAccess = Extent.Removed;
		}
		public Visual(Area component2D) : base()
		{
			creationFrame = Performance.FrameCount;
			rand = Number.Random(new Bounds(-9999, 9999), 5);
			transform = component2D;
		}
		//public abstract void DrawBounds(Camera camera, float thickness, Color color);
	}
}