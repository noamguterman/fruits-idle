using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	public static class ColorAnimation
    {
        private static ColorAnimationHandler colorAnimationHandler;

        private static ColorAnimationHandler ColorAnimationHandler
		{
			get
			{
				if (!colorAnimationHandler)
				{
					return colorAnimationHandler = (GameObject.FindObjectOfType<ColorAnimationHandler>() ?? new GameObject().AddComponent<ColorAnimationHandler>());
				}
				return colorAnimationHandler;
			}
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, float duration, bool destroy = false, float initialDelay = 0f, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, duration, true, destroy, initialDelay, null, postExecuteAction);
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, 0f, true, false, 0f, null, postExecuteAction);
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, float duration, Action<T> postExecuteAction) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, duration, true, false, 0f, null, postExecuteAction);
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, float duration, bool animateChildren, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, duration, animateChildren, false, 0f, null, postExecuteAction);
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, float duration, float initialDelay, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, duration, true, false, initialDelay, null, postExecuteAction);
		}

		public static Coroutine AnimateAlpha<T>([NotNull] this T target, float destinationAlpha, float duration, AnimationCurve curve, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateAlpha<T>(target, destinationAlpha, duration, true, false, 0f, curve, postExecuteAction);
		}

		public static Coroutine AnimateColor<T>([NotNull] this T target, Color targetColor, float duration, bool animateChildren, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateColor<T>(target, targetColor, duration, animateChildren, false, 0f, postExecuteAction);
		}

		public static Coroutine AnimateColor<T>([NotNull] this T target, Color targetColor, float duration, Action<T> postExecuteAction = null) where T : Component
		{
			return ColorAnimationHandler.AnimateColor<T>(target, targetColor, duration, true, false, 0f, postExecuteAction);
		}

		public static void StopAllAlphaAnimations([NotNull] this Component component)
		{
			ColorAnimationHandler.StopAlphaAnimations(component);
		}

		public static void StopAllColorAnimations([NotNull] this Component component)
		{
			ColorAnimationHandler.StopColorAnimations(component);
		}

	}
}
