using System;
using UnityEngine;

namespace Utilities
{
	public static class Vector2Utils
	{
		public static Vector2 SetX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		public static Vector2 SetY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		public static Vector3 SetZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		public static Vector2 MoveX(this Vector2 v, float x)
		{
			return new Vector2(v.x + x, v.y);
		}

		public static Vector2 MoveY(this Vector2 v, float y)
		{
			return new Vector2(v.x, v.y + y);
		}

		public static float Lerp(this Vector2 v, float t)
		{
			return Mathf.Lerp(v.x, v.y, t);
		}

		public static Vector2 ScaleInverse(this Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x / v2.x, v1.y / v2.y);
		}
	}
}
