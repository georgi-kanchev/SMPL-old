﻿namespace SMPL
{
	public struct Bounds
	{
		private double lower, upper;

		public double Lower
		{
			get { return lower; }
			set
			{
				//if (value > upper)
				//{
				//	lower = upper;
				//	upper = value;
				//	return;
				//}
				lower = value;
			}
		}
		public double Upper
		{
			get { return upper; }
			set
			{
				//if (value < lower)
				//{
				//	upper = lower;
				//	lower = value;
				//	return;
				//}
				upper = value;
			}
		}

		public Bounds(double lower, double upper)
		{
			this.lower = lower;
			this.upper = upper;
			Lower = this.lower;
			Upper = this.upper;
		}
	}
}
