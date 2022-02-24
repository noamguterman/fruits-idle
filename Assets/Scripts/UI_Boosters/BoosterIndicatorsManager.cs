using System;
using System.Collections.Generic;
using System.Linq;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems.Booster;
using UI.ListItems.Buttons;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI.Boosters
{
	[RequireComponent(typeof(RectTransform))]
	public class BoosterIndicatorsManager : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [AssetsOnly]
        private BoosterIndicator boosterIndicatorPrefab;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private ButtonsSettings buttonsSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float indicatorAppearTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float indicatorDissapearTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float indicatorAppearOffset;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float spacing;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private AnimationCurve indicatorAppearCurve;

        private RectTransform managerTransform;

        private List<IndicatorContainer> currentActiveBoosterIndicators;

        private void Awake()
		{
			currentActiveBoosterIndicators = new List<IndicatorContainer>();
			managerTransform = (RectTransform)transform;
		}

		private void OnEnable()
		{
			boostersSettings.ChangeBoosterTypeMutiplier += OnChangeBoosterTypeMutiplier;
		}

		private void OnDisable()
		{
			boostersSettings.ChangeBoosterTypeMutiplier -= OnChangeBoosterTypeMutiplier;
		}

		private void OnChangeBoosterTypeMutiplier(BoosterType boosterType, float multiplier, float time, bool isEnabled)
		{
			if (isEnabled)
			{
				if (currentActiveBoosterIndicators.All((IndicatorContainer i) => i.BoosterType != boosterType))
				{
					Sprite upgradeImage = buttonsSettings.ButtonItemsBoosters.First((ButtonItemBooster b) => b.BoosterType == boosterType).UpgradeImage;
					currentActiveBoosterIndicators.Add(new IndicatorContainer
					{
						Indicator = CreateBoosterIndicator(multiplier, time, upgradeImage),
						BoosterType = boosterType,
						PositionIndex = -1
					});
				}
			}
			else if (currentActiveBoosterIndicators.Any((IndicatorContainer i) => i.BoosterType == boosterType))
			{
				IndicatorContainer indicatorContainer = this.currentActiveBoosterIndicators.First((IndicatorContainer i) => i.BoosterType == boosterType);
				DisableBoosterIndicator(indicatorContainer.Indicator);
				currentActiveBoosterIndicators.Remove(indicatorContainer);
			}
			UpdatePositions();
		}

		private void DisableBoosterIndicator(BoosterIndicator boosterIndicator)
		{
			boosterIndicator.AnimateAlpha(0f, indicatorDissapearTime, delegate(BoosterIndicator i)
			{
				boosterIndicator.Push<BoosterIndicator>();
			});
		}

		private BoosterIndicator CreateBoosterIndicator(float multiplier, float time, Sprite boosterIcon)
		{
			BoosterIndicator boosterIndicator = boosterIndicatorPrefab.PullOrCreate<BoosterIndicator>();
			Transform transform = boosterIndicator.transform;
			transform.SetParent(managerTransform);
			transform.localScale = Vector3.one;
			boosterIndicator.StartBoosterIndicator(time, multiplier, boosterIcon, null);
			boosterIndicator.AnimateAlpha(0f, null);
			boosterIndicator.AnimateAlpha(1f, indicatorAppearTime, false, 0f, null);
			return boosterIndicator;
		}

		private void UpdatePositions()
		{
			for (int i = 0; i < currentActiveBoosterIndicators.Count; i++)
			{
				IndicatorContainer indicatorContainer = currentActiveBoosterIndicators[i];
				Transform transform = indicatorContainer.Indicator.transform;
				if (i != indicatorContainer.PositionIndex)
				{
					float num = (managerTransform.rect.size.y - indicatorContainer.Indicator.Size.y) / 2f - (indicatorContainer.Indicator.Size.y + spacing) * i;
					if (indicatorContainer.PositionIndex < 0)
					{
						transform.localPosition = new Vector3(0, num + indicatorAppearOffset, 0);
					}
					transform.MoveLocalPositionY(num, indicatorAppearTime, indicatorAppearCurve);
					indicatorContainer.PositionIndex = i;
				}
			}
		}

	}
}
