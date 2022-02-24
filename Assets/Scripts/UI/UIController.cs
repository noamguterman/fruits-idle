using System;
using Game;
using Game.Audio;
using JetBrains.Annotations;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace UI
{
	public class UIController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TouchArea touchArea;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private OptionsPopUpMenu optionsPopUpMenu;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private OfflineTimeTracker offlineTimeTracker;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private EarningsDialog earningsDialog;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private CanvasScaler canvasScaler;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RectTransform bottomPanelView;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RectTransform topView;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RectTransform saveArea;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Toggle optionsToggle;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Toggle vibrationToggle;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Toggle soundToggle;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button restorePurchasesButton;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private GameSettings gameSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private ShopSettings shopSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private IOSScreenSettings iosScreenSettings;

        private float offlineEarnings;

        private float initialCameraYPos;

        private bool isPromoWindowVideoLoaded;

        private bool isOfflineDialogShowing;

        private void Awake()
		{
			canvasScaler.matchWidthOrHeight = ((CameraUtils.MainCamera.aspect > 0.5625f) ? 1 : 0);
			restorePurchasesButton.gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
			initialCameraYPos = CameraUtils.MainCamera.transform.position.y;
			Preloader.LogoHided += OnLogoHided;
			vibrationToggle.gameObject.SetActive(iOSHapticFeedback.Instance.IsSupported());
			touchArea.MouseButtonClick += delegate(Vector3 mousePos)
			{
				optionsPopUpMenu.Close();
			};
		}

		private void OnLogoHided()
		{
            //offlineEarnings = upgradeSettings.GetCurrentUpgrade(UpgradeType.OfflineIncome).Value * Mathf.Min(offlineTimeTracker.ApplicationOfflineTimeHours, upgradeSettings.MaxOfflineDirationHours);
            offlineEarnings = 100 * Mathf.Min(offlineTimeTracker.ApplicationOfflineTimeHours, upgradeSettings.MaxOfflineDirationHours);
            print(string.Format("Application Offline Time Hours = {0}", offlineTimeTracker.ApplicationOfflineTimeHours));
			//print(string.Format("Upgrade = {0}", upgradeSettings.GetCurrentUpgrade(UpgradeType.OfflineIncome).Value));
			print(string.Format("offline Earnings = {0}", offlineEarnings));
			if (offlineEarnings < upgradeSettings.MinOfflineEarnings)
			{
				return;
			}
			isOfflineDialogShowing = true;
			//earningsDialog.Show(gameSettings.OfflineEarningsText, offlineEarnings, "offline_click", false);
		}

		private void Start()
		{
			vibrationToggle.isOn = SoundSettings.IsVibrationOn;
			soundToggle.isOn = SoundSettings.IsSoundOn;
		}

		private void OnEnable()
		{
            //Ad.BannerStateChanged += this.OnBannerStateChange;
            earningsDialog.EarningsDialogVisibilityChange += OnEarningsDialogVisibilityChange;
			vibrationToggle.onValueChanged.AddListener(new UnityAction<bool>(OnVibrationToggleValueChanged));
			optionsToggle.onValueChanged.AddListener(delegate(bool iOn)
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			});
			soundToggle.onValueChanged.AddListener(new UnityAction<bool>(OnSoundToggleValueChanged));
			restorePurchasesButton.onClick.AddListener(new UnityAction(OnRestorePurchasesButtonClick));
		}

		private void OnDisable()
		{
            //Ad.BannerStateChanged -= this.OnBannerStateChange;
            earningsDialog.EarningsDialogVisibilityChange -= OnEarningsDialogVisibilityChange;
			vibrationToggle.onValueChanged.RemoveListener(new UnityAction<bool>(OnVibrationToggleValueChanged));
			soundToggle.onValueChanged.RemoveListener(new UnityAction<bool>(OnSoundToggleValueChanged));
			restorePurchasesButton.onClick.RemoveListener(new UnityAction(OnRestorePurchasesButtonClick));
		}

		private void OnEarningsDialogVisibilityChange(bool isVisible)
		{
			if (!isVisible && isOfflineDialogShowing)
			{
				isOfflineDialogShowing = false;
			}
		}

        public void OnBannerStateChange(bool enabled)
        {
            //int num = Ad.BannerSize.y * (Screen.width / Ad.BannerSize.x);
            int num = 90 * (Screen.width / Screen.width);
            Vector3 localScale = this.canvasScaler.transform.localScale;
            this.saveArea.offsetMin = new Vector2(0f, enabled ? ((float)num / localScale.y) : 0f);
            CameraUtils.MainCamera.transform.SetPositionY(enabled ? (this.initialCameraYPos - (float)num / localScale.x / this.canvasScaler.referencePixelsPerUnit) : this.initialCameraYPos);
        }

        private void OnSoundToggleValueChanged(bool isOn)
		{
			SoundSettings.IsSoundOn = isOn;
			if (isOn)
			{
				AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
				if (audioSource == null)
				{
					return;
				}
				audioSource.Play(false);
			}
		}

		private void OnVibrationToggleValueChanged(bool isOn)
		{
			AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
			if (audioSource != null)
			{
				audioSource.Play(false);
			}
			SoundSettings.IsVibrationOn = isOn;
		}

		private void OnRestorePurchasesButtonClick()
		{
			AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.UIElementClick);
			if (audioSource != null)
			{
				audioSource.Play(false);
			}
		}

		[UsedImplicitly]
		public void AddMoneyButton()
		{
			GameData.IncreaseMoney(1E+07f, MoneyType.Money, true);
		}

	}
}
