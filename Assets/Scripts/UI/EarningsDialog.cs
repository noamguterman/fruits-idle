using System;
using Game.Audio;
using Sirenix.OdinInspector;
using TMPro;
using UI.ListItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;
using Settings.UI.Tabs;

namespace UI
{
	public class EarningsDialog : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Text earningsText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Text earningsAmountText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [SceneObjectsOnly]
        private Text gemAmountText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Text rewardedMultiplierText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Image innerImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Button getMoney;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Button getMoneyRewarded;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [SceneObjectsOnly]
        private Button getMoneyGem;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private int gemAmount;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private int rewardedMultiplier;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float collectButtonAppearDelay;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private AnimationCurve getMoneyButtonAppearCurve;

        private float earnedMoney;

        public bool IsShowed;

        private bool shouldShowInterstitial;

        private string flurryOnRewardEventName;

        public event Action<bool> EarningsDialogVisibilityChange;

		private void Awake()
		{
			this.AnimateAlpha(0f, null);
			getMoney.onClick.AddListener(delegate()
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UICollectMoneyClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			});
			getMoneyRewarded.onClick.AddListener(delegate()
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UICollectMoneyRewardedClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			});
			if (getMoneyGem != null)
			{
				getMoneyGem.onClick.AddListener(delegate()
				{
					AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UICollectMoneyRewardedClick);
					if (audioSource == null)
					{
						return;
					}
					audioSource.Play(false);
				});
			}
		}

		private void Start()
		{
			gameObject.SetActive(false);
		}

        int cnt = 0;
		public void Show(string earningsText, float earnedMoney, string flurryOnRewardEventName, bool shouldShowInterstitial = false)
		{
			this.flurryOnRewardEventName = flurryOnRewardEventName;
            cnt++;
            if (cnt % 3 == 0)
                shouldShowInterstitial = true;
            if(ShopSettings.IsNoAdsPurchased == true)
                shouldShowInterstitial = false;

            Show(earningsText, earnedMoney, shouldShowInterstitial);
		}

		public void Show(string earningsText, float earnedMoney, bool shouldShowInterstitial = false)
		{
			if (IsShowed)
			{
				return;
			}
			this.shouldShowInterstitial = shouldShowInterstitial;
			gameObject.SetActive(true);
			getMoney.transform.localScale = Vector3.zero;
			innerImage.transform.localScale = Vector3.zero;
			innerImage.transform.MoveScale(Vector3.one, 0.15f, null);
			this.AnimateAlpha(1f, 0.15f, delegate(EarningsDialog d)
			{
				StartCoroutine(CoroutineUtils.Delay(collectButtonAppearDelay, delegate()
				{
					getMoney.transform.MoveScale(Vector3.one, 0.4f, getMoneyButtonAppearCurve, null);
				}));
			});
			getMoney.onClick.AddListener(new UnityAction(OnGetMoneyButtonClick));
			getMoneyRewarded.onClick.AddListener(new UnityAction(OnGetMoneyRewardedButtonClick));
			Button button = getMoneyGem;
			if (button != null)
			{
				button.onClick.AddListener(new UnityAction(OnGetMoneyGemButtonClick));
			}
			IsShowed = true;

			if (getMoneyGem != null)
			{
				getMoneyGem.interactable = ((float)gemAmount <= GameData.GetMoney(MoneyType.Gem));
			}

            IsShowed = true;
            //getMoneyRewarded.interactable = Ad.IsRewardedLoaded;
            //Ad.RewardedLoaded += this.OnRewardedLoaded;
            if (getMoneyGem != null)
            {
                getMoneyGem.interactable = (gemAmount <= GameData.GetMoney(MoneyType.Gem));
            }


            Text tmp_Text = gemAmountText;
			if (tmp_Text != null)
			{
				tmp_Text.text = gemAmount.ToString();
			}
			this.earningsText.text = earningsText;
			Text tmp_Text2 = earningsAmountText;
			string str = "+";
			this.earnedMoney = earnedMoney;
			tmp_Text2.text = str + earnedMoney.AbbreviateNumber(true);
			rewardedMultiplierText.text = string.Format("Collect x{0}", rewardedMultiplier);
			Action<bool> earningsDialogVisibilityChange = EarningsDialogVisibilityChange;
			if (earningsDialogVisibilityChange == null)
			{
				return;
			}
			earningsDialogVisibilityChange(true);
		}

		private void OnRewardedLoaded()
		{
			getMoneyRewarded.interactable = true;
		}

		public void SimulateClick(bool rewarded)
		{
			if (rewarded)
			{
				OnGetMoneyRewardedButtonClick();
				return;
			}
			OnGetMoneyButtonClick();
		}

		private void Hide()
		{
            //Ad.RewardedLoaded -= this.OnRewardedLoaded;

            IsShowed = false;
			this.AnimateAlpha(0f, 0.15f, delegate(EarningsDialog d)
			{
				d.gameObject.SetActive(false);
			});
			getMoney.onClick.RemoveListener(new UnityAction(OnGetMoneyButtonClick));
			getMoneyRewarded.onClick.RemoveListener(new UnityAction(OnGetMoneyRewardedButtonClick));
			Button button = getMoneyGem;
			if (button != null)
			{
				button.onClick.RemoveListener(new UnityAction(OnGetMoneyGemButtonClick));
			}
			Action<bool> earningsDialogVisibilityChange = EarningsDialogVisibilityChange;
			earningsDialogVisibilityChange(false);

            flurryOnRewardEventName = string.Empty;

            if(gameObject.name == "Earnings Dialog Level Up" && (GameData.CurrentLevel == 20 || GameData.CurrentLevel == 25))
            {
                GameObject.Find("GuideNextScreen").GetComponent<GuideNextScreen>().Show(GameData.CurrentLevel);
            }
            else if (gameObject.name == "Earnings Dialog Level Up" && (GameData.CurrentLevel == 35 || GameData.CurrentLevel == 40))
            {
                GameObject.Find("GuideNextScreen").GetComponent<GuideNextScreen>().Show(GameData.CurrentLevel);
            }
        }

		private void OnGetMoneyButtonClick()
		{
            if (this.shouldShowInterstitial)
            {
                AdsManager.Instance.ShowInterstitial();
            }

            GameData.IncreaseMoney(earnedMoney, MoneyType.Money, false);
			Hide();
		}

		private void OnGetMoneyGemButtonClick()
		{
			GameData.IncreaseMoney((float)(-gemAmount), MoneyType.Gem, true);
			GameData.IncreaseMoney(earnedMoney * 3f, MoneyType.Money, false);
			Hide();
		}

		private void OnGetMoneyRewardedButtonClick()
		{
            AdsManager.RewardedClosed += this.OnRewardedClosed;
            AdsManager.Instance.ShowRewardedVideo();

        }

        private void OnRewardedClosed(bool isReward)
		{
            AdsManager.RewardedClosed -= this.OnRewardedClosed;

            if (!isReward)
			{
				return;
			}
			GameData.IncreaseMoney(earnedMoney * (float)rewardedMultiplier, MoneyType.Money, false);
			Hide();
		}

	}
}
