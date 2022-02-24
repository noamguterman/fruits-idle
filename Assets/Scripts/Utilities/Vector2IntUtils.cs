using System;
using UnityEngine;

namespace Utilities
{
	public static class Vector2IntUtils
	{
		public static float Lerp(this Vector2Int v, float t)
		{
			return Mathf.Lerp((float)v.x, (float)v.y, t);
		}
	}
}
