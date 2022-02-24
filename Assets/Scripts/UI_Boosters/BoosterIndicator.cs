using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Boosters
{
	[RequireComponent(typeof(RectTransform))]
	public class BoosterIndicator : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image iconImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Text multiplierText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Timer timer;

        private RectTransform boosterIndicatorTransform;

        public Vector3 Position
		{
			get
			{
				return boosterIndicatorTransform.anchoredPosition;
			}
			set
			{
				boosterIndicatorTransform.anchoredPosition = value;
			}
		}

		public Vector2 Size
		{
			get
			{
				return boosterIndicatorTransform.rect.size;
			}
		}

		private void Awake()
		{
			boosterIndicatorTransform = (RectTransform)transform;
		}

		private void OnDisable()
		{
			timer.StopTimer();
		}

		public void StartBoosterIndicator(float time, float multiplier, Sprite icon, Action callback = null)
		{
			timer.StartTimer(time, Timer.OutputType.All);
			timer.Elapsed += delegate()
			{
				Action callback2 = callback;
				if (callback2 == null)
				{
					return;
				}
				callback2();
			};
			multiplierText.text = string.Format("{0}x", multiplier);
			iconImage.sprite = icon;
		}

	}
}
