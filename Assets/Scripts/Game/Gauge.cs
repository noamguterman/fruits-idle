using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game
{
	public class Gauge : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SpriteRenderer gaugeInner;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private Lamp lamp;

        private float maxYPosition;

        private float minYPosition;

        private Transform gaugeInnerTransform;

        public int MaxCount { private get; set; }

		public Color Color
		{
			set
			{
				gaugeInner.color = value;
			}
		}

		public int CurrentCount
		{
			set
			{
				gaugeInnerTransform.MoveLocalPositionY(Mathf.Lerp(minYPosition, maxYPosition, (float)value / (float)MaxCount), 0.2f);
				if (!lamp)
				{
					return;
				}
				if (value >= MaxCount)
				{
					if (!lamp.IsFlickering)
					{
						lamp.StartFlashing();
						return;
					}
				}
				else if (lamp.IsFlickering)
				{
					lamp.Stop();
				}
			}
		}

		private void Awake()
		{
			gaugeInnerTransform = gaugeInner.transform;
			maxYPosition = gaugeInnerTransform.localPosition.y;
			minYPosition = maxYPosition - gaugeInnerTransform.localScale.y;
			gaugeInnerTransform.SetLocalPositionY(minYPosition);
		}

	}
}
