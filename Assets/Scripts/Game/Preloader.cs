using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;
using DG.Tweening;

namespace Game
{
	public class Preloader : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image preloaderImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RateWindow rateWindow;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private GameSettings gameSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private ShopSettings shopSettings;

        public GameObject progressBar;

        public static bool IsFirstApplicationLaunch
		{
			get
			{
				return PlayerPrefs.GetInt("IsFirstApplicationLaunch", 1) != 0;
			}
			private set
			{
				PlayerPrefs.SetInt("IsFirstApplicationLaunch", value ? 1 : 0);
			}
		}

		public static event Action LogoHided;

		private void Awake()
		{
			Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.ScriptOnly);
			Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
			preloaderImage.AnimateAlpha(1f, null);
            progressBar.GetComponent<Image>().DOFillAmount(1, gameSettings.LogoShowTime - 0.5f);

            preloaderImage.AnimateAlpha(0f, gameSettings.LogoAnimationTime, gameSettings.LogoShowTime, delegate(Image logo)
			{
				preloaderImage.gameObject.SetActive(false);
				Action logoHided = LogoHided;
				if (logoHided == null)
				{
					return;
				}
				logoHided();
			});
			bool flag = Application.platform == RuntimePlatform.IPhonePlayer;
			ObjectPool.IsLoggingEnabled = gameSettings.IsLoggingEnabled;
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = -1;

			if (!ReviewGameController.IsReviewGameWindowShowed)
			{
				ReviewGameController.Initialize(rateWindow);
			}

            if (flag)
			{
				iOSHapticFeedback.Create();
			}
		}

		private void Start()
		{
			shopSettings.Initialize();
		}

		private void OnDisable()
		{
			IsFirstApplicationLaunch = false;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
		}

		[UsedImplicitly]
		public void AddMoney()
		{
			GameData.SetMoney(GameData.GetMoney(MoneyType.Money) * 2f, MoneyType.Money, true);
		}

		[UsedImplicitly]
		public void RequestReview()
		{
			ReviewGameController.RequestReview();
		}

	}
}
