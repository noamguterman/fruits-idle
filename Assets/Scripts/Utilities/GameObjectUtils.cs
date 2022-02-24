using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class GameObjectUtils
	{
		public static bool IsPrefab([NotNull] this GameObject unityGameObject)
		{
			return unityGameObject.scene.rootCount == 0;
		}
	}
}
