using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class AnimationCurveUtils
	{
		public static float EvaluateTime([NotNull] this AnimationCurve initialCurve, float value)
		{
			float num = initialCurve.keys.First<Keyframe>().time;
			float num2 = 0.01f;
			float num3 = 0.005f;
			float time = initialCurve.keys.Last<Keyframe>().time;
			while (Mathf.Abs(num - time) > num2)
			{
				num = Mathf.Min(time, num + num3);
				if (Mathf.Abs(initialCurve.Evaluate(num) - value) < num2)
				{
					return num;
				}
			}
			return 0f;
		}
	}
}
