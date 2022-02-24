using System;
using UnityEngine;

namespace Utilities
{
	public static class ColorUtil
	{
		public static Color SetAlpha(this Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		public static float[] ToArray(this Color c)
		{
			return new float[]
			{
				c.r,
				c.g,
				c.b,
				c.a
			};
		}
	}
}
