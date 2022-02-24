using System;
using Game;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace UI
{
	public class LevelRow : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Image levelBarInner;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Image levelBarOuter;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private EarningsDialog earningsDialog;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Text currentLevelText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Button levelIncreaseButton;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private LevelsSettings levelsSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private GameSettings gameSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        [SerializeField]
        [Required]
        private Button gemIncreaseButton;

        private Level currentLevel;

        private bool isEarningsDialogOpened;

        private bool isLevelButtonActive;

        private MultipleCoroutine levelButtonScaleRoutine;

        private bool CouldIncreaseLevel
		{
			get
			{
				return GameData.CurrentLevelXp >= currentLevel.EarnedMoneyToUnclock;
			}
		}

		public event Action LevelUpAvailable;

        private void Awake()
        {
            gemIncreaseButton.onClick.AddListener(delegate ()
            {
                AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
                if (audioSource == null)
                {
                    return;
                }
                audioSource.Play(false);
            });

        }
        private void OnEnable()
		{
			Preloader.LogoHided += Initialize;
			levelIncreaseButton.onClick.AddListener(new UnityAction(OnLevelIncreaseButtonClick));
		}

		private void OnDisable()
		{
			Preloader.LogoHided -= Initialize;
			levelIncreaseButton.onClick.RemoveListener(new UnityAction(OnLevelIncreaseButtonClick));
		}

		private void Initialize()
		{
			currentLevel = levelsSettings.GetCurrentLevel();
			currentLevelText.text = levelsSettings.GetCurrentLevel().LevelNumber.ToString();
			levelIncreaseButton.interactable = false;
			GameData.MoneyChange += OnMoneyChange;
			OnMoneyChange(GameData.GetMoney(MoneyType.Money), MoneyType.Money);


            this.gemIncreaseButton.onClick.AddListener(delegate ()
            {
                AdsManager.RewardedClosed += this.OnRewardedClosed;
                AdsManager.Instance.ShowRewardedVideo();
            });
        }

		private void SetLevelBar()
		{
            levelBarInner.transform.GetComponent<Image>().fillAmount = Mathf.Min(GameData.CurrentLevelXp / currentLevel.EarnedMoneyToUnclock, 1f);

            //levelBarInner.rectTransform.anchoredPosition = new Vector2(levelBarOuter.rectTransform.rect.width * (Mathf.Min(GameData.CurrentLevelXp / currentLevel.EarnedMoneyToUnclock, 1f) - 1f), levelBarInner.rectTransform.anchoredPosition.y);
		}

		private void OnMoneyChange(float money, MoneyType moneyType)
		{
			if (moneyType != MoneyType.Money)
			{
				return;
			}
			if (CouldIncreaseLevel && !isEarningsDialogOpened && !isLevelButtonActive)
			{
				levelIncreaseButton.interactable = (isLevelButtonActive = true);
				if (levelButtonScaleRoutine == null)
				{
                    AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIReadyLevelUp);
                    if (audioSource != null)
                    {
                        audioSource.Play(false);
                    }

                    MultipleCoroutine multipleCoroutine = new MultipleCoroutine(this);
					multipleCoroutine.Add(() => TransformUtils.LerpScale(levelIncreaseButton.transform, Vector3.one * 1.15f, 0.2f, null, null));
					multipleCoroutine.Add(() => TransformUtils.LerpScale(levelIncreaseButton.transform, Vector3.one * 1f, 0.2f, null, null));
					MultipleCoroutine multipleCoroutine2 = multipleCoroutine;
					levelButtonScaleRoutine = multipleCoroutine;
					multipleCoroutine2.StartCoroutines(true);
				}
				base.StartCoroutine(CoroutineUtils.Delay(0.2f, delegate()
				{
                    Action levelUpAvailable = LevelUpAvailable;
					if (levelUpAvailable == null)
					{
						return;
					}
					levelUpAvailable();
				}));
			}
			SetLevelBar();
		}

		public void SimulateClick()
		{
			OnLevelIncreaseButtonClick();
		}

		private void OnLevelIncreaseButtonClick()
		{
			levelIncreaseButton.interactable = (isLevelButtonActive = false);
			MultipleCoroutine multipleCoroutine = levelButtonScaleRoutine;
			if (multipleCoroutine != null)
			{
				multipleCoroutine.Dispose();
			}
			levelButtonScaleRoutine = null;
			levelIncreaseButton.transform.MoveScale(Vector3.one, 0.1f, null);
			if (SoundSettings.IsVibrationOn)
			{
				iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
			}
			AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UILevelUpClick);
			if (audioSource != null)
			{
				audioSource.Play(false);
			}
			earningsDialog.EarningsDialogVisibilityChange += OnEarningsDialogVisibilityChange;
			earningsDialog.Show(gameSettings.OnLevelIncreaseReardText, currentLevel.RewardedMoney, "levelup_click");
		}

		private void OnEarningsDialogVisibilityChange(bool isVisible)
		{
			isEarningsDialogOpened = isVisible;
			if (isVisible)
			{
				return;
			}
			earningsDialog.EarningsDialogVisibilityChange -= OnEarningsDialogVisibilityChange;
			GameData.IncreaseLevel();
			currentLevel = levelsSettings.GetCurrentLevel();
			currentLevelText.text = levelsSettings.GetCurrentLevel().LevelNumber.ToString();
			SetLevelBar();
		}


        private void OnRewardedClosed(bool isReward)
        {
            AdsManager.RewardedClosed -= this.OnRewardedClosed;

            if (!isReward)
            {
                return;
            }

            GameData.SetMoney(GameData.GetMoney(MoneyType.Gem) + 4f, MoneyType.Gem, true);
        }


	}
}
