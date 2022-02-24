using System;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Camera Settings", fileName = "Camera Settings")]
	public class CameraSettings : ScriptableObject
    {
        [SerializeField]
        private float sensitivity;

        [SerializeField]
        private float minSpeedSnapThreshold;

        [SerializeField]
        private float speedDecreaseRate;

        [SerializeField]
        private float snapToScreenTime;

        [SerializeField]
        private AnimationCurve snapToScreenCurve;

        public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
		}

		public float MinSpeedSnapThreshold
		{
			get
			{
				return minSpeedSnapThreshold;
			}
		}

		public float SpeedDecreaseRate
		{
			get
			{
				return speedDecreaseRate;
			}
		}

		public float SnapToScreenTime
		{
			get
			{
				return snapToScreenTime;
			}
		}

		public AnimationCurve SnapToScreenCurve
		{
			get
			{
				return snapToScreenCurve;
			}
		}

	}
}
