using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
    public static class ObjectPool
    {
        private static readonly Dictionary<Type, Stack<GameObject>> PushedObjectsType = new Dictionary<Type, Stack<GameObject>>();

        private static readonly Dictionary<int, Stack<GameObject>> PushedObjects = new Dictionary<int, Stack<GameObject>>();

        private static readonly Dictionary<int, Stack<GameObject>> PulledObjects = new Dictionary<int, Stack<GameObject>>();

        private static readonly int MaxObjectsCount = 1000;

        private static int currentObjectsCount;

        private static readonly bool ShowPulledObjectsInHierarchy = false;

        public static bool IsLoggingEnabled;

        public static T[] Create<T>(T unityObject, int amount) where T : UnityEngine.Object
        {
            if (amount <= 0)
            {
                return null;
            }
            if (unityObject == null)
            {
                LogMessage("Object you try to instantiate is null.", LogType.Error);
                return null;
            }
            T[] array = new T[amount];
            GameObject gameObject = GetGameObject(unityObject, true);
            for (int i = 0; i < amount; i++)
            {
                array[i] = UnityEngine.Object.Instantiate<T>(unityObject);
                if (gameObject.IsPrefab())
                {
                    AddToDict(GetGameObject(array[i], true), gameObject.GetInstanceID());
                }
            }
            return array;
        }

        public static T PullOrCreate<T>([NotNull] this T unityObject) where T : UnityEngine.Object
        {
            GameObject gameObject = GetGameObject(unityObject, true);
            T t = (T)((object)null);
            foreach (int num in PushedObjects.Keys)
            {
                if (num == gameObject.GetInstanceID())
                {
                    GameObject gameObject2 = PushedObjects[num].Pop();
                    gameObject2.hideFlags = HideFlags.None;
                    gameObject2.SetActive(true);
                    if (PushedObjects[num].Count == 0)
                    {
                        PushedObjects.Remove(num);
                    }
                    if (gameObject2 is T)
                    {
                        t = (gameObject2 as T);
                        break;
                    }
                    if (gameObject2.GetComponent<T>() != null)
                    {
                        t = gameObject2.GetComponent<T>();
                        break;
                    }
                }
            }
            if (t != null)
            {
                if (gameObject.IsPrefab())
                {
                    AddToDict(GetGameObject(t, true), gameObject.GetInstanceID());
                }
                currentObjectsCount--;
                return t;
            }
            foreach (Type type in PushedObjectsType.Keys)
            {
                if (type == typeof(T))
                {
                    GameObject gameObject3 = PushedObjectsType[type].Pop();
                    gameObject3.SetActive(true);
                    gameObject3.hideFlags = HideFlags.None;
                    if (PushedObjectsType[type].Count == 0)
                    {
                        PushedObjectsType.Remove(type);
                    }
                    if (gameObject3 is T)
                    {
                        t = (gameObject3 as T);
                        break;
                    }
                    if (gameObject3.GetComponent<T>() != null)
                    {
                        t = gameObject3.GetComponent<T>();
                        break;
                    }
                }
            }
            if (t == null)
            {
                t = UnityEngine.Object.Instantiate<T>(unityObject);
            }
            else
            {
                currentObjectsCount--;
            }
            if (gameObject.IsPrefab())
            {
                AddToDict(GetGameObject(t, true), gameObject.GetInstanceID());
            }
            return t;
        }

        private static GameObject GetGameObject(UnityEngine.Object unityObject, bool fromParent = false)
        {
            GameObject gameObject = (unityObject as GameObject) ?? ((!(unityObject is Component)) ? null : ((Component)unityObject).gameObject);
            if (gameObject == null)
            {
                return null;
            }
            if (fromParent)
            {
                while (gameObject.transform.parent != null)
                {
                    gameObject = gameObject.transform.parent.gameObject;
                }
            }
            return gameObject;
        }

        private static void AddToDict(GameObject unityGameObject, int id)
        {
            if (PulledObjects.Count > 0 && PulledObjects.ContainsKey(id))
            {
                PulledObjects[id].Push(unityGameObject);
            }
            else
            {
                Stack<GameObject> stack = new Stack<GameObject>();
                stack.Push(unityGameObject);
                PulledObjects.Add(id, stack);
            }
        }

        public static void Push<T>(params T[] unityObjects) where T : UnityEngine.Object
        {
            foreach (T unityObject in unityObjects)
            {
                PushInternal<T>(unityObject);
            }
        }

        public static void Push<T>([NotNull] this T unityObject) where T : UnityEngine.Object
        {
            PushInternal<T>(unityObject);
        }

        private static void PushInternal<T>(T unityObject) where T : UnityEngine.Object
        {
            GameObject gameObjectToPush = GetGameObject(unityObject, false);
            if (gameObjectToPush == null)
            {
                LogMessage("Object you try to push is null.", LogType.Error);
                return;
            }
            if (gameObjectToPush.IsPrefab())
            {
                LogMessage(string.Format("Object you try to push is a prefab. ({0})", gameObjectToPush.name), LogType.Error);
                return;
            }
            if (currentObjectsCount >= MaxObjectsCount)
            {
                LogMessage("Pool is full. Destroying Object.", LogType.Log);
                UnityEngine.Object.Destroy(unityObject);
                return;
            }
            gameObjectToPush.transform.SetParent(null);
            if (!ShowPulledObjectsInHierarchy)
            {
                gameObjectToPush.hideFlags = HideFlags.HideInHierarchy;
            }
            gameObjectToPush.SetActive(false);
            KeyValuePair<int, Stack<GameObject>> keyValuePair2 = PulledObjects.FirstOrDefault((KeyValuePair<int, Stack<GameObject>> keyValuePair) => keyValuePair.Value.Any((GameObject gm) => gm == gameObjectToPush));
            if (!keyValuePair2.Equals(default(KeyValuePair<int, Stack<GameObject>>)))
            {
                if (PushedObjects.ContainsKey(keyValuePair2.Key))
                {
                    if (PushedObjects[keyValuePair2.Key].Any((GameObject gm) => gm == gameObjectToPush))
                    {
                        LogMessage(string.Format("Object you try to push is already in pool. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Warning);
                        return;
                    }
                    if (IsLoggingEnabled)
                    {
                        LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
                    }
                    PushedObjects[keyValuePair2.Key].Push(gameObjectToPush);
                    currentObjectsCount++;
                }
                else
                {
                    if (IsLoggingEnabled)
                    {
                        LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
                    }
                    Stack<GameObject> stack = new Stack<GameObject>();
                    stack.Push(gameObjectToPush);
                    PushedObjects.Add(keyValuePair2.Key, stack);
                    currentObjectsCount++;
                }
            }
            else
            {
                bool flag = false;
                foreach (Type type in PushedObjectsType.Keys)
                {
                    if (type == typeof(T))
                    {
                        if (PushedObjectsType[type].All((GameObject o) => o != gameObjectToPush))
                        {
                            if (IsLoggingEnabled)
                            {
                                LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
                            }
                            PushedObjectsType[type].Push(gameObjectToPush);
                            currentObjectsCount++;
                        }
                        else
                        {
                            LogMessage(string.Format("Object you try to push is already in pool. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Warning);
                        }
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    return;
                }
                Stack<GameObject> stack2 = new Stack<GameObject>();
                stack2.Push(gameObjectToPush);
                PushedObjectsType.Add(typeof(T), stack2);
                currentObjectsCount++;
            }
        }

        private static void LogMessage(string msg, LogType logType = LogType.Log)
        {
            UnityEngine.Debug.unityLogger.Log(logType, string.Format("Object Pool: {0}", msg));
        }

    }
}












//using System;
//using System.Collections.Generic;
//using System.Linq;
//using JetBrains.Annotations;
//using UnityEngine;

//namespace Utilities
//{
//	public static class ObjectPool
//	{
//		public static T[] PullOrCreate<T>([NotNull] this T unityObject, int amount) where T : UnityEngine.Object
//		{
//			if (amount <= 0)
//			{
//				return null;
//			}
//			if (unityObject == null)
//			{
//				ObjectPool.LogMessage("Object you try to instantiate is null.", LogType.Error);
//				return null;
//			}
//			T[] array = new T[amount];
//			GameObject gameObject = ObjectPool.GetGameObject(unityObject, true);
//			for (int i = 0; i < amount; i++)
//			{
//				array[i] = unityObject.PullOrCreate<T>();
//				if (gameObject.IsPrefab())
//				{
//					ObjectPool.AddToDict(ObjectPool.GetGameObject(array[i], true), gameObject.GetInstanceID());
//				}
//			}
//			return array;
//		}

//		public static T PullOrCreate<T>([NotNull] this T unityObject) where T : UnityEngine.Object
//        {
//			GameObject gameObject = ObjectPool.GetGameObject(unityObject, true);
//			T t = default(T);
//			foreach (int num in ObjectPool.PushedObjects.Keys)
//			{
//				if (num == gameObject.GetInstanceID())
//				{
//					KeyValuePair<int, GameObject> keyValuePair = ObjectPool.PushedObjects[num].Pop();
//					keyValuePair.Value.hideFlags = HideFlags.None;
//					keyValuePair.Value.SetActive(true);
//					if (ObjectPool.PushedObjects[num].Count == 0)
//					{
//						ObjectPool.PushedObjects.Remove(num);
//					}
//					if (keyValuePair.Value.GetComponent<T>() != null)
//					{
//						t = keyValuePair.Value.GetComponent<T>();
//						break;
//					}
//				}
//			}
//			if (t != null)
//			{
//				if (gameObject.IsPrefab())
//				{
//					ObjectPool.AddToDict(ObjectPool.GetGameObject(t, true), gameObject.GetInstanceID());
//				}
//				ObjectPool.currentObjectsCount--;
//				if (ObjectPool.IsLoggingEnabled)
//				{
//					ObjectPool.LogMessage("Object Pulled. Object Name = " + t.name, LogType.Log);
//				}
//				return t;
//			}
//			foreach (Type type in ObjectPool.PushedObjectsType.Keys)
//			{
//				if (type == typeof(T))
//				{
//					GameObject gameObject2 = ObjectPool.PushedObjectsType[type].Pop();
//					gameObject2.SetActive(true);
//					gameObject2.hideFlags = HideFlags.None;
//					if (ObjectPool.PushedObjectsType[type].Count == 0)
//					{
//						ObjectPool.PushedObjectsType.Remove(type);
//					}
//					if (gameObject2 is T)
//					{
//						t = (gameObject2 as T);
//						break;
//					}
//					if (gameObject2.GetComponent<T>() != null)
//					{
//						t = gameObject2.GetComponent<T>();
//						break;
//					}
//				}
//			}

//            if (t != null)

//            {
//                ObjectPool.currentObjectsCount--;
//				if (ObjectPool.IsLoggingEnabled)
//				{
//					ObjectPool.LogMessage("Object Pulled. Object Name = " + t.name, LogType.Log);
//				}
//			}
//			if (gameObject.IsPrefab())
//			{
//				ObjectPool.AddToDict(ObjectPool.GetGameObject(t, true), gameObject.GetInstanceID());
//			}
//			return t;
//		}

//		private static GameObject GetGameObject(UnityEngine.Object unityObject, bool fromParent = false)
//		{
//			GameObject gameObject;
//			if ((gameObject = (unityObject as GameObject)) == null)
//			{
//				Component component = unityObject as Component;
//				gameObject = ((component != null) ? component.gameObject : null);
//			}
//			GameObject gameObject2 = gameObject;
//			if (gameObject2 == null)
//			{
//				return null;
//			}
//			if (fromParent)
//			{
//				while (gameObject2.transform.parent != null)
//				{
//					gameObject2 = gameObject2.transform.parent.gameObject;
//				}
//			}
//			return gameObject2;
//		}

//		private static void AddToDict(GameObject unityGameObject, int id)
//		{
//			if (ObjectPool.PulledObjects.Count > 0 && ObjectPool.PulledObjects.ContainsKey(id))
//			{
//				ObjectPool.PulledObjects[id].Push(unityGameObject);
//				return;
//			}

//            Stack<GameObject> stack = new Stack<GameObject>();
//            stack.Push(unityGameObject);
//            ObjectPool.PulledObjects.Add(id, stack);
//		}

//		public static void Push<T>(params T[] unityObjects) where T : UnityEngine.Object
//		{
//			for (int i = 0; i < unityObjects.Length; i++)
//			{
//				ObjectPool.PushInternal<T>(unityObjects[i]);
//			}
//		}

//		public static void Push<T>([NotNull] this T unityObject) where T : UnityEngine.Object
//        {
//			ObjectPool.PushInternal<T>(unityObject);
//		}

//		public static void PushUnsafe<T>([NotNull] this T unityObject, int initialPrefabTransformID) where T : UnityEngine.Object
//		{
//			GameObject gameObject = ObjectPool.GetGameObject(unityObject, false);
//			gameObject.transform.SetParent(null);
//			if (!ObjectPool.ShowPulledObjectsInHierarchy)
//			{
//				gameObject.hideFlags = HideFlags.HideInHierarchy;
//			}
//			gameObject.SetActive(false);
//			ObjectPool.AddPushedObjectToDict(initialPrefabTransformID, gameObject, true);
//		}

//		private static void PushInternal<T>(T unityObject) where T : UnityEngine.Object
//		{
//			GameObject gameObjectToPush = ObjectPool.GetGameObject(unityObject, false);
//			if (gameObjectToPush == null)
//			{
//				ObjectPool.LogMessage("Object you try to push is null.", LogType.Error);
//				return;
//			}
//			if (gameObjectToPush.IsPrefab())
//			{
//				ObjectPool.LogMessage("Object you try to push is a prefab. (" + gameObjectToPush.name + ")", LogType.Error);
//				return;
//			}
//			if (ObjectPool.currentObjectsCount >= ObjectPool.MaxObjectsCount)
//			{
//				ObjectPool.LogMessage("Pool is full. Destroying Object.", LogType.Log);
//				UnityEngine.Object.Destroy(unityObject);
//				return;
//			}
//			gameObjectToPush.transform.SetParent(null);
//			if (!ObjectPool.ShowPulledObjectsInHierarchy)
//			{
//				gameObjectToPush.hideFlags = HideFlags.HideInHierarchy;
//			}
//			gameObjectToPush.SetActive(false);
//			int instanceID = gameObjectToPush.transform.GetInstanceID();
//			int num = 0;
//			foreach (KeyValuePair<int, Stack<GameObject>> keyValuePair in ObjectPool.PulledObjects)
//			{
//				Stack<GameObject> value = keyValuePair.Value;
//				int count = value.Count;
//				for (int i = 0; i < count; i++)
//				{
//					if (value.Pop() == gameObjectToPush)
//					{
//						num = keyValuePair.Key;
//						break;
//					}
//				}
//			}
//			if (num != 0)
//			{
//				ObjectPool.AddPushedObjectToDict(num, gameObjectToPush, false);
//				return;
//			}
//			bool flag = false;
//			foreach (Type type in ObjectPool.PushedObjectsType.Keys)
//			{
//				if (type == typeof(T))
//				{
//                    IEnumerable<GameObject> source = ObjectPool.PushedObjectsType[type];
//                    Func<GameObject, bool> predicate;
//                    predicate = new Func<GameObject, bool>((GameObject o) => o != gameObjectToPush);

//                    if (source.All(predicate))
//                    {
//						if (ObjectPool.IsLoggingEnabled)
//						{
//							ObjectPool.LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
//						}
//						ObjectPool.PushedObjectsType[type].Push(gameObjectToPush);
//						ObjectPool.currentObjectsCount++;
//					}
//					else
//					{
//						ObjectPool.LogMessage(string.Format("Object you try to push is already in pool. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Warning);
//					}
//					flag = true;
//					break;
//				}
//			}
//			if (flag)
//			{
//				return;
//			}
//			Stack<GameObject> stack = new Stack<GameObject>();
//			stack.Push(gameObjectToPush);
//			ObjectPool.PushedObjectsType.Add(typeof(T), stack);
//			ObjectPool.currentObjectsCount++;
//		}

//		private static void AddPushedObjectToDict(int prefabKeyID, GameObject gameObjectToPush, bool unsafePush = false)
//		{
//			int instanceID = gameObjectToPush.transform.GetInstanceID();
//			if (ObjectPool.PushedObjects.ContainsKey(prefabKeyID))
//			{
//				if (!unsafePush)
//				{
//					foreach (KeyValuePair<int, GameObject> keyValuePair in ObjectPool.PushedObjects[prefabKeyID])
//					{
//						if (keyValuePair.Key == instanceID)
//						{
//							ObjectPool.LogMessage(string.Format("Object you try to push is already in pool. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Warning);
//							return;
//						}
//					}
//				}
//				if (ObjectPool.IsLoggingEnabled)
//				{
//					ObjectPool.LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
//				}
//				ObjectPool.PushedObjects[prefabKeyID].Push(new KeyValuePair<int, GameObject>(instanceID, gameObjectToPush));
//				ObjectPool.currentObjectsCount++;
//				return;
//			}
//			if (ObjectPool.IsLoggingEnabled)
//			{
//				ObjectPool.LogMessage(string.Format("Object Pushed. Object Name = {0}, ID = {1}", gameObjectToPush.name, gameObjectToPush.transform.GetInstanceID()), LogType.Log);
//			}
//			Stack<KeyValuePair<int, GameObject>> stack = new Stack<KeyValuePair<int, GameObject>>();
//			stack.Push(new KeyValuePair<int, GameObject>(instanceID, gameObjectToPush));
//			ObjectPool.PushedObjects.Add(prefabKeyID, stack);
//			ObjectPool.currentObjectsCount++;
//		}

//		private static void LogMessage(string msg, LogType logType = LogType.Log)
//		{
//			UnityEngine.Debug.unityLogger.Log(logType, "Object Pool: " + msg);
//		}

//		private static readonly Dictionary<Type, Stack<GameObject>> PushedObjectsType = new Dictionary<Type, Stack<GameObject>>();

//		private static readonly Dictionary<int, Stack<KeyValuePair<int, GameObject>>> PushedObjects = new Dictionary<int, Stack<KeyValuePair<int, GameObject>>>();

//		private static readonly Dictionary<int, Stack<GameObject>> PulledObjects = new Dictionary<int, Stack<GameObject>>();

//		private static readonly int MaxObjectsCount = 10000;

//		private static int currentObjectsCount;

//		private static readonly bool ShowPulledObjectsInHierarchy = false;

//		public static bool IsLoggingEnabled;
//	}
//}
