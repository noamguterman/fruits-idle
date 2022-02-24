using System;
using UnityEngine;

namespace Utilities
{
	public static class ObjectUtils
	{
		public static bool IsNull(this UnityEngine.Object obj)
		{
			return obj == null;
		}
	}
}
