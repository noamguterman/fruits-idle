using System;
using UnityEngine;

namespace Utilities.Patterns
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;

        private static readonly object LockObject = new object();

        private static bool applicationIsQuitting;

        protected MonoSingleton()
		{
		}

		public static T Instance
		{
			get
			{
				T result;
				if (MonoSingleton<T>.applicationIsQuitting)
				{
					result = default(T);
					return result;
				}
				object lockObject = MonoSingleton<T>.LockObject;
				lock (lockObject)
				{
					if (MonoSingleton<T>.instance != null)
					{
						result = MonoSingleton<T>.instance;
					}
					else
					{
						MonoSingleton<T>.instance = (T)((object)UnityEngine.Object.FindObjectOfType(typeof(T)));
						if (UnityEngine.Object.FindObjectsOfType(typeof(T)).Length > 1)
						{
							result = MonoSingleton<T>.instance;
						}
						else if (MonoSingleton<T>.instance != null)
						{
							result = MonoSingleton<T>.instance;
						}
						else
						{
							GameObject gameObject = new GameObject();
							MonoSingleton<T>.instance = gameObject.AddComponent<T>();
							gameObject.name = "(singleton) " + typeof(T);
							DontDestroyOnLoad(gameObject);
							result = MonoSingleton<T>.instance;
						}
					}
				}
				return result;
			}
		}

		public static void Initialize()
		{
			MonoSingleton<T>.instance = MonoSingleton<T>.Instance;
		}

		public void OnDestroy()
		{
			MonoSingleton<T>.applicationIsQuitting = true;
		}

	}
}
