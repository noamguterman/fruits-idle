using System;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Smelters Settings", fileName = "Smelters Settings")]
	public class SmeltersSettings : ScriptableObject
	{
		public float StartGlowTime
		{
			get
			{
				return this.startGlowTime;
			}
		}

		public float EndGlowTime
		{
			get
			{
				return this.endGlowTime;
			}
		}

		public AnimationCurve GlowCurve
		{
			get
			{
				return this.glowCurve;
			}
		}

		[SerializeField]
		private float startGlowTime;

		[SerializeField]
		private float endGlowTime;

		[SerializeField]
		private AnimationCurve glowCurve;
	}
}
