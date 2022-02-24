using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class CoroutineUtils
	{
        private static CoroutineHandler coroutineHandler;

        public static CoroutineHandler CoroutineHandler
		{
			get
			{
				if (!coroutineHandler)
				{
					return coroutineHandler = (GameObject.FindObjectOfType<CoroutineHandler>() ?? new GameObject().AddComponent<CoroutineHandler>());
				}
				return coroutineHandler;
			}
		}

		public static Coroutine StartCoroutine(IEnumerator coroutine)
		{
			return CoroutineHandler.StartCoroutine(coroutine);
		}

		public static void StopCoroutine(Coroutine coroutine)
		{
			CoroutineHandler.StopCoroutine(coroutine);
		}

		public static IEnumerator Delay(float time, Action postExecute)
		{
			if (time > 0f)
			{
				yield return new WaitForSeconds(time);
			}
			postExecute();
			yield break;
		}

		public static IEnumerator Delay(float time, Reference<float> multiplier, Action postExecute)
		{
			float t = 0f;
			while (t < time)
			{
				t = Mathf.Min(t + Time.deltaTime * multiplier.Value, time);
				yield return null;
			}
			postExecute();
			yield break;
		}

		public static IEnumerator Delay(float time)
		{
			yield return new WaitForSeconds(time);
			yield break;
		}

		public static IEnumerator LerpFloat(Reference<float> a, float b, float time)
		{
			float currentTime = 0f;
			float initialValue = a.Value;
			while (currentTime < time)
			{
				currentTime = Mathf.Min(time, currentTime + Time.deltaTime);
				a.Value = Mathf.Lerp(initialValue, b, currentTime / time);
				yield return null;
			}
			yield break;
		}

		public static IEnumerator LerpFloat(float a, float b, float time, Action<float> predicate)
		{
			float currentTime = 0f;
			while (currentTime < time)
			{
				currentTime = Mathf.Min(time, currentTime + Time.deltaTime);
				predicate(Mathf.Lerp(a, b, currentTime / time));
				yield return null;
			}
			yield break;
		}

		public static IEnumerator Delay(YieldInstruction yieldInstruction, Action postExecute)
		{
			yield return yieldInstruction;
			postExecute();
			yield break;
		}

		public static void StopCoroutineIfRunning([NotNull] this MonoBehaviour monoBehaviour, ref Coroutine coroutine)
		{
			if (coroutine == null)
			{
				return;
			}
			monoBehaviour.StopCoroutine(coroutine);
			coroutine = null;
		}

		public static void StopCoroutineIfRunning([NotNull] this MonoBehaviour monoBehaviour, Coroutine coroutine)
		{
			if (coroutine == null)
			{
				return;
			}
			monoBehaviour.StopCoroutine(coroutine);
		}

		public static IEnumerator LoopExecute(Action execute, YieldInstruction yieldInstruction = null)
		{
			while (true)
			{
				execute();
				yield return yieldInstruction;
			}
		}

		public static IEnumerator LoopExecute(Action execute, YieldInstruction yieldInstruction, Func<bool> stopFunc, Action postExecute = null)
		{
			while (!stopFunc())
			{
				execute();
				yield return yieldInstruction;
			}
			postExecute();
			yield break;
		}

		public static IEnumerator ExecuteAfter(Action executeAction, YieldInstruction yieldInstruction = null)
		{
			yield return yieldInstruction;
			executeAction();
			yield break;
		}

		public static IEnumerator ExecuteAfter(Action executeAction, CustomYieldInstruction yieldInstruction)
		{
			yield return yieldInstruction;
			executeAction();
			yield break;
		}

	}
}
