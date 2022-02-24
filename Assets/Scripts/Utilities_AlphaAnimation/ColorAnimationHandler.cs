using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	public class ColorAnimationHandler : MonoBehaviour
    {
        private static ColorAnimationHandler instance;

        private Dictionary<Component, Coroutine> currentAlphaAnimations;

        private Dictionary<Component, Coroutine> currentColorAnimations;

        private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				gameObject.hideFlags = HideFlags.HideInHierarchy;
				DontDestroyOnLoad(gameObject);
				currentAlphaAnimations = new Dictionary<Component, Coroutine>();
				currentColorAnimations = new Dictionary<Component, Coroutine>();
				return;
			}
			Destroy(gameObject);
		}

		public void StopAlphaAnimations(Component component)
		{
			StopAnimations(component, currentAlphaAnimations);
		}

		public void StopColorAnimations(Component component)
		{
			StopAnimations(component, currentColorAnimations);
		}

		private void StopAnimations(Component component, Dictionary<Component, Coroutine> animations)
		{
			IEnumerable<Coroutine> enumerable = from e in animations
			where e.Key == component
			select e.Value;
			animations.Remove(component);
			foreach (Coroutine routine in enumerable)
			{
				StopCoroutine(routine);
			}
		}

		public Coroutine AnimateAlpha<T>(T target, float alpha, float duration, bool animateChildren, bool destroy, float initialDelay, AnimationCurve animationCurve, Action<T> action) where T : Component
		{
			if (currentAlphaAnimations.ContainsKey(target))
			{
				Coroutine routine;
				currentAlphaAnimations.TryGetValue(target, out routine);
				currentAlphaAnimations.Remove(target);
				StopCoroutine(routine);
			}
			Coroutine coroutine = StartCoroutine(AlphaAnimation<T>(target, alpha, duration, animateChildren, destroy, initialDelay, animationCurve, action));
			currentAlphaAnimations.Add(target, coroutine);
			return coroutine;
		}

		public Coroutine AnimateColor<T>(T target, Color color, float duration, bool animateChildren, bool destroy, float initialDelay, Action<T> action) where T : Component
		{
			if (currentColorAnimations.ContainsKey(target))
			{
				Coroutine routine;
				currentColorAnimations.TryGetValue(target, out routine);
				currentColorAnimations.Remove(target);
				StopCoroutine(routine);
			}
			Coroutine coroutine = StartCoroutine(ColorAnimation<T>(target, color, duration, animateChildren, destroy, initialDelay, null, action));
			currentColorAnimations.Add(target, coroutine);
			return coroutine;
		}

		private IEnumerator AlphaAnimation<T>(T target, float alpha, float duration, bool animateChildren, bool destroy, float initialDelay, AnimationCurve animationCurve, Action<T> action) where T : Component
		{
			if (initialDelay > 0f)
			{
				yield return new WaitForSeconds(initialDelay);
			}
			Renderable[] array;
			if (!animateChildren)
			{
				(array = new Renderable[1])[0] = target.GetComponent<Renderable>();
			}
			else
			{
				array = target.GetComponentsInChildren<Renderable>(true);
			}
			Renderable[] targets = array;
			float currentTime = 0f;
			List<Color> startColors = (from renderable in targets
			select renderable.Color).ToList<Color>();
			while (true)
			{
				currentTime += Time.deltaTime;
				currentTime = Mathf.Min(currentTime, duration);
				for (int i = 0; i < targets.Length; i++)
				{
					if (!targets[i].IsNull())
					{
						float num = (alpha > targets[i].UpperBound) ? targets[i].UpperBound : alpha;
						Color color = targets[i].Color;
						color.a = ((duration > 0f) ? Mathf.Lerp(startColors[i].a, num, (animationCurve != null) ? animationCurve.Evaluate(currentTime / duration) : (currentTime / duration)) : num);
						targets[i].Color = color;
					}
				}
				yield return null;
				if (target.IsNull() || !target.gameObject.activeSelf)
				{
					break;
				}
				if (currentTime >= duration)
				{
					goto Block_11;
				}
			}
			currentAlphaAnimations.Remove(target);
            if(action != null)
			    action(target);

			yield break;
			Block_11:
			if (destroy)
			{
				Destroy(target.gameObject);
			}
			currentAlphaAnimations.Remove(target);
            if (action != null)
                action(target);
			yield break;
		}

		private IEnumerator ColorAnimation<T>(T target, Color color, float duration, bool animateChildren, bool destroy, float initialDelay, AnimationCurve animationCurve, Action<T> action) where T : Component
		{
			if (initialDelay > 0f)
			{
				yield return new WaitForSeconds(initialDelay);
			}
			Renderable[] array;
			if (!animateChildren)
			{
				(array = new Renderable[1])[0] = target.GetComponent<Renderable>();
			}
			else
			{
				array = target.GetComponentsInChildren<Renderable>(true);
			}
			Renderable[] targets = array;
			float currentTime = 0f;
			List<Color> startColors = (from renderable in targets
			select renderable.Color).ToList<Color>();
			while (true)
			{
				currentTime += Time.deltaTime;
				currentTime = Mathf.Min(currentTime, duration);
				for (int i = 0; i < targets.Length; i++)
				{
					if (!targets[i].IsNull())
					{
						targets[i].Color = Color.Lerp(startColors[i].SetAlpha(targets[i].Color.a), color.SetAlpha(targets[i].Color.a), (duration > 0f) ? ((animationCurve != null) ? animationCurve.Evaluate(currentTime / duration) : (currentTime / duration)) : 1f);
					}
				}
				yield return null;
				if (target.IsNull() || !target.gameObject.activeSelf)
				{
					break;
				}
				if (currentTime >= duration)
				{
					goto Block_10;
				}
			}
			currentColorAnimations.Remove(target);
            if (action != null)
                action(target);

			yield break;
			Block_10:
			if (destroy)
			{
				Destroy(target.gameObject);
			}
			currentColorAnimations.Remove(target);
            if (action != null)
                action(target);
			
			yield break;
		}

	}
}
