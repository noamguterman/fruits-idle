using System;
using System.Collections;
using Game.Press;
using Sirenix.OdinInspector;
using TMPro;
using UI.ListItems;
using UIControllers;
using UnityEngine;
using UnityEngine.UI;
using Settings.UI.Tabs;
using UI.ListItems.Booster;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI
{
	public class Boostmeter : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private BoostmeterSettings boostmeterSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        [SerializeField]
        [Required]
        private TouchArea touchArea;

        [SerializeField]
        [Required]
        private PressManager pressManager;

        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private BonusView bonusView;

        [SerializeField]
        [Required]
        private Image arrow;

        [SerializeField]
        [Required]
        private Text percentText;

        private Coroutine rawProgressDecreaseCoroutine;

        private Coroutine disappearingWithDelayCoroutine;

        private float targetAlpha;

        private float speedMultiplier;

        private bool isBonusEnabled;

        public event Action<float> MultiplierChange;

		private float LinearPercentFromMaxBoost
		{
			get
			{
				return (SpeedMultiplier - 1f) / (boostmeterSettings.MaxSpeedMultiplier - 1f);
			}
		}

		private float SpeedMultiplier
		{
			get
			{
				return speedMultiplier;
			}
			set
			{
				speedMultiplier = Mathf.Clamp(value, 1f, boostmeterSettings.MaxSpeedMultiplier);
				float linearPercentFromMaxBoost = LinearPercentFromMaxBoost;
				arrow.transform.eulerAngles = new Vector3
				{
					x = 0f,
					y = 0f,
					z = boostmeterSettings.ArrowMaxOffsetFromCenterInDegrees + -boostmeterSettings.ArrowMaxOffsetFromCenterInDegrees * 2f * linearPercentFromMaxBoost
				};
				percentText.text = ((int)(LinearPercentFromMaxBoost * 100f) + "%");
				//percentText.color = boostmeterSettings.ColorChangeOfPercentTextGradient.Evaluate(LinearPercentFromMaxBoost);
				pressManager.OnMultiplierChange(SpeedMultiplier);
				Action<float> multiplierChange = MultiplierChange;
				if (multiplierChange == null)
				{
					return;
				}
				multiplierChange(SpeedMultiplier);
			}
		}

		private void Awake()
		{
			targetAlpha = 0f;
			this.AnimateAlpha(targetAlpha, 0f, false, 0f, null);
		}

		private void OnEnable()
		{
			touchArea.MouseButtonClick += OnTouchAreaPointerClick;
			bonusView.BonusStateChange += OnBonusStateChange;
            boostersSettings.ChangeBoosterTypeMutiplier += OnBoosterMutiplierChanged;
            rawProgressDecreaseCoroutine = StartCoroutine(ProgressDecreasing());
		}

		private void OnDisable()
		{
			touchArea.MouseButtonClick -= OnTouchAreaPointerClick;
			bonusView.BonusStateChange -= OnBonusStateChange;
            boostersSettings.ChangeBoosterTypeMutiplier -= OnBoosterMutiplierChanged;
            StopCoroutine(rawProgressDecreaseCoroutine);
		}

		private void OnBonusStateChange(Bonus b, bool isEnabled)
		{
            Debug.Log("@@@@@@@@@@@@@@@@@@");
			if (b.BonusType != BonusType.SpeedBoost)
			{
				return;
			}
			isBonusEnabled = isEnabled;
			SpeedMultiplier = boostmeterSettings.MaxSpeedMultiplier;
			if (isEnabled)
			{
				this.AnimateAlpha(targetAlpha = 1f, boostmeterSettings.AlphaLerpTime, false, 0f, null);
			}
		}

        

        private void OnBoosterMutiplierChanged(BoosterType boosterType, float multiplier, float time, bool isEnabled)
        {
            if (boosterType == BoosterType.MultiplierIncome)
            {
                return;
            }
            isBonusEnabled = isEnabled;
            SpeedMultiplier = boostmeterSettings.MaxSpeedMultiplier;
            if (isEnabled)
            {
                this.AnimateAlpha(targetAlpha = 1f, boostmeterSettings.AlphaLerpTime, false, 0f, null);
            }
        }


        private void OnTouchAreaPointerClick(Vector3 pos)
		{
			if (isBonusEnabled)
			{
				return;
			}
			if (disappearingWithDelayCoroutine != null)
			{
				StopCoroutine(disappearingWithDelayCoroutine);
				disappearingWithDelayCoroutine = null;
				this.StopAllAlphaAnimations();
			}
			if (targetAlpha != 1f)
			{
				targetAlpha = 1f;
				this.StopAllAlphaAnimations();
				this.AnimateAlpha(targetAlpha, boostmeterSettings.AlphaLerpTime, false, 0f, null);
			}
			SpeedMultiplier += boostmeterSettings.PercentIncreasePerClickByPercent.Evaluate(LinearPercentFromMaxBoost) * boostmeterSettings.MaxSpeedMultiplier;
		}

		private IEnumerator ProgressDecreasing()
		{
			while (true)
			{
				if (!isBonusEnabled)
				{
					SpeedMultiplier -= boostmeterSettings.PercentDecreasePerSecByPercent.Evaluate(LinearPercentFromMaxBoost) * Time.deltaTime;
					if (Math.Abs(SpeedMultiplier - 1f) < 0.005f && targetAlpha != 0f)
					{
						targetAlpha = 0f;
						disappearingWithDelayCoroutine = StartCoroutine(CoroutineUtils.Delay(boostmeterSettings.DelayBeforeDisappear, delegate()
						{
							this.StopAllAlphaAnimations();
							this.AnimateAlpha(targetAlpha, boostmeterSettings.AlphaLerpTime, delegate(Boostmeter ignored)
							{
								disappearingWithDelayCoroutine = null;
							});
						}));
					}
				}
				yield return null;
			}
		}
	}
}
