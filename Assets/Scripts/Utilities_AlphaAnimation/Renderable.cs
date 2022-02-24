using System;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	public abstract class Renderable : MonoBehaviour
	{
		public abstract Color Color { get; set; }

		public bool DisabledFromAnimator { get; set; }

		[SerializeField]
		[Range(0f, 1f)]
		public float UpperBound = 1f;
	}
}
