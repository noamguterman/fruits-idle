using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities.AlphaAnimation;

namespace UI
{
	public class RateWindow : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button rateButton;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button cancelButton;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private string googlePlayGameURL;

        [SerializeField]
        private string iOSPlayGameURL;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float showTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float hideTime;

        public void Show()
		{
			gameObject.SetActive(true);
			this.AnimateAlpha(0f, null);
			this.AnimateAlpha(1f, showTime, false, 0f, null);
			rateButton.onClick.AddListener(new UnityAction(OnRateButtonClick));
			rateButton.onClick.AddListener(new UnityAction(Hide));
			cancelButton.onClick.AddListener(new UnityAction(Hide));
		}

		private void Hide()
		{
			rateButton.onClick.RemoveListener(new UnityAction(OnRateButtonClick));
			rateButton.onClick.RemoveListener(new UnityAction(Hide));
			cancelButton.onClick.RemoveListener(new UnityAction(Hide));
			this.AnimateAlpha(0f, hideTime, delegate(RateWindow window)
			{
				window.gameObject.SetActive(false);
			});
		}

		private void OnRateButtonClick()
		{
#if UNITY_ANDROID
            Application.OpenURL(googlePlayGameURL);
#else
            Application.OpenURL(iOSPlayGameURL);
#endif
        }

	}
}
