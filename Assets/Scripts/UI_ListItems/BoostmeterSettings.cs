using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Boostmeter Settings")]
	public class BoostmeterSettings : ScriptableObject
	{
		[Header("1.0f = default speed")]
		public float MaxSpeedMultiplier = 2f;

		[FormerlySerializedAs("DecreasePerSecByPercent")]
		public AnimationCurve PercentDecreasePerSecByPercent;

		[FormerlySerializedAs("IncreasePerClickByPercent")]
		public AnimationCurve PercentIncreasePerClickByPercent;

		public float ArrowMaxOffsetFromCenterInDegrees;

		public Gradient ColorChangeOfPercentTextGradient;

		public float DelayBeforeDisappear;

		public float AlphaLerpTime;
	}
}
