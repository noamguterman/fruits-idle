using System;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;
using Settings.UI.Tabs;

namespace UI.Boosters
{
	public class BoosterDialog : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Transform inner;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Text boosterText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Text collectText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Text collectRewardedText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button collect;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button collectRewarded;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float appearTime;

        [SerializeField]
        private float collectButtonAppearDelay;

        [SerializeField]
        private AnimationCurve getMoneyButtonAppearCurve;

        private Action<bool> callback;

        private void Awake()
		{
			this.AnimateAlpha(0f, null);
			this.collect.onClick.AddListener(delegate()
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			});
			collectRewarded.onClick.AddListener(delegate()
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			});
		}

		private void Start()
		{
			gameObject.SetActive(false);
		}

		public void Show(Bonus bonus, Action<bool> callback = null)
		{
			this.callback = callback;
			boosterText.text = bonus.BonusText;

            //collectRewarded.interactable = Ad.IsRewardedLoaded;
            //Ad.RewardedLoaded += this.OnRewardedLoaded;

            collectText.text = string.Format("Boost for {0} min", Mathf.RoundToInt(bonus.BonusDuration / 60f));
			collectRewardedText.text = string.Format("Boost for {0} min", Mathf.RoundToInt(bonus.BonusRewardedDuration / 60f));
			gameObject.SetActive(true);
            collect.transform.localScale = Vector3.zero;

            this.AnimateAlpha(0f, null);
            //this.AnimateAlpha(1f, appearTime, false, 0f, null);
            this.AnimateAlpha(1f, appearTime, delegate (BoosterDialog d)
            {
                StartCoroutine(CoroutineUtils.Delay(collectButtonAppearDelay, delegate ()
                {
                    collect.transform.MoveScale(Vector3.one, 0.4f, getMoneyButtonAppearCurve, null);
                }));
            });

            inner.localScale = Vector3.zero;
			inner.MoveScale(Vector3.one, appearTime, null);
			collect.onClick.AddListener(delegate()
			{
				Action<bool> action = this.callback;
				action(false);

                if (UnityEngine.Random.value < 0.5f && ShopSettings.IsNoAdsPurchased == false)
                {
                    AdsManager.Instance.ShowInterstitial();
                }

                Hide();
			});

            this.collectRewarded.onClick.AddListener(delegate ()
            {
                AdsManager.RewardedClosed += this.OnRewardedClosed;
               AdsManager.Instance.ShowRewardedVideo();
            });
        }

		private void OnRewardedClosed(bool isReward)
		{
            AdsManager.RewardedClosed -= this.OnRewardedClosed;

            Action<bool> action = callback;
			action(isReward);

			Hide();
		}

		private void OnRewardedLoaded()
		{
			collectRewarded.interactable = true;
		}

		private void Hide()
		{
            //Ad.RewardedLoaded -= this.OnRewardedLoaded;

            inner.MoveScale(Vector3.zero, appearTime, null);
			this.AnimateAlpha(0f, appearTime, delegate(BoosterDialog t)
			{
				gameObject.SetActive(false);
			});
			collect.onClick.RemoveAllListeners();
			collectRewarded.onClick.RemoveAllListeners();
		}

	}
}
