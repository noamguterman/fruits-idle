using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class AnimatorUtils
	{
		public static float GetAnimationLength([NotNull] this Animator animator, string animationName)
		{
			if (animationName.Length != 0)
			{
				return animator.runtimeAnimatorController.animationClips.First((AnimationClip a) => a.name == animationName).length;
			}
			return 0f;
		}
	}
}
