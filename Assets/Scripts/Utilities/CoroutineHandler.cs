using System;
using UnityEngine;

namespace Utilities
{
	public class CoroutineHandler : MonoBehaviour
	{
		private void Awake()
		{
			name = "Coroutine Handler";
			hideFlags = HideFlags.HideInHierarchy;
			DontDestroyOnLoad(gameObject);
		}
	}
}
