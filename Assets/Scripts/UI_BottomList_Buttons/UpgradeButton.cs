using System;
using Sirenix.OdinInspector;
using TMPro;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI.BottomList.Buttons
{
    public class UpgradeButton : ButtonBase
	{
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Timer timer;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected TMP_Text upgradeName;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected TMP_Text level;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Image levelLockImage;

        //[FoldoutGroup("Sprites", 0)]
        [SerializeField]
        [Required]
        protected Sprite lockSprite;

        private bool isSetEnabled;
        public override void Initialize(ListItem listItem, TabController tabController)
		{
			base.Initialize(listItem, tabController);
			this.listItem.StartTimer += StartTimer;
		}

		private Timer StartTimer(float initialTime, float time)
		{
			upgradeButtonImage.gameObject.SetActive(false);
			priceContainer.gameObject.SetActive(false);
			timer.StartTimer(initialTime, listItem.TimerAdditionalOutputString, listItem.TimerOutputType);
			timer.CurrentTime = time;
			timer.Elapsed += delegate()
			{
				priceContainer.gameObject.SetActive(true);
				upgradeButtonImage.gameObject.SetActive(true);
			};
			return timer;
		}

		public override void Refresh()
		{
			base.Refresh();
			upgradeName.text = listItem.UpgradeName;
			level.text = listItem.LevelText;
			if (listItem.IsLevelLocked)
			{
				if (!levelLockImage.gameObject.activeSelf)
				{
					levelLockImage.gameObject.SetActive(true);
					levelLockImage.AnimateAlpha(1f, 0.1f, false, 0f, null);
				}
				priceImage.sprite = null;
				upgradePrice.text = string.Format("lvl {0}", listItem.CurrentLockLevel);
				upgradeValue.text = string.Empty;
			}
			else
			{
				if (levelLockImage.gameObject.activeSelf)
				{
					levelLockImage.AnimateAlpha(0f, 0.2f, delegate(Image img)
					{
						img.gameObject.SetActive(false);
					});
				}
				priceImage.sprite = listItem.PriceImage;
				upgradePrice.text = listItem.PriceText;
				upgradeValue.text = listItem.ValueText;
			}
			upgradeButtonImage.sprite = ((listItem.IsLocked || listItem.IsLevelLocked) ? lockSprite : listItem.ButtonImage);
			GameObject gameObject;
			(gameObject = upgradeButtonImage.gameObject).SetActive(upgradeButtonImage.sprite != null && !timer.IsActivated && listItem.IsAvailable);
			priceContainer.childAlignment = (gameObject.activeSelf ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter);
		}

	}
}
