﻿using SFML.Graphics;
using SFML.System;
using System;

namespace SMPL
{
	public struct Point
	{
		public double X { get; set; }
		public double Y { get; set; }
		public Color Color { get; set; }

		public Point(double x, double y)
		{
			Color = Color.White;
			X = x;
			Y = y;
		}
		public void Display(Camera camera)
		{
			if (Window.DrawNotAllowed()) return;
			var vert = new Vertex[] { new(From(this), Color.From(Color)) };
			camera.rendTexture.Draw(vert, PrimitiveType.Points);
		}

		public bool IsInvalid => double.IsNaN(X) || double.IsNaN(Y);
		public static double Distance(Point pointA, Point pointB)
		{
			return Math.Sqrt(Math.Pow(pointB.X - pointA.X, 2) + Math.Pow(pointB.Y - pointA.Y, 2));
		}
		public static Point MoveAtAngle(Point point, double angle, double speed, Time.Unit timeUnit = Time.Unit.Second)
		{
			if (timeUnit == Time.Unit.Second) speed *= Performance.DeltaTime;
			var dir = Number.AngleToDirection(angle);

			point.X += dir.X * speed;
			point.Y += dir.Y * speed;
			return point;
		}
		public static Point MoveTowardTarget(Point point, Point targetPoint, double speed,
			Time.Unit timeUnit = Time.Unit.Second)
		{
			var ang = Number.AngleBetweenPoints(point, targetPoint);
			return MoveAtAngle(point, ang, speed, timeUnit);
		}
		public static Point PercentTowardTarget(Point point, Point targetPoint, Size percent)
		{
			var x = Number.FromPercent(percent.W, new Bounds(point.X, targetPoint.X));
			var y = Number.FromPercent(percent.H, new Bounds(point.Y, targetPoint.Y));
			return new Point(x, y);
		}

		public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
		public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
		public static Point operator *(Point a, Point b) => new(a.X * b.X, a.Y * b.Y);
		public static Point operator /(Point a, Point b) => new(a.X / b.X, a.Y / b.Y);
		public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Point a, Point b) => a.X != b.X || a.Y != b.Y;
		public static Point operator /(Point a, double b) => new(a.X / b, a.Y / b);
		public static Point operator *(Point a, double b) => new(a.X * b, a.Y * b);

		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override bool Equals(object obj) => default;
		/// <summary>
		/// This default <see cref="object"/> method is not implemented.
		/// </summary>
		public override int GetHashCode() => default;

		public override string ToString() => $"{X} {Y}";

		internal static Point To(Vector2f point) => new(point.X, point.Y);
		internal static Vector2f From(Point point) => new((float)point.X, (float)point.Y);
	}
}
