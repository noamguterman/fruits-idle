using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities.AlphaAnimation;

namespace UI.BottomList.Buttons
{
	public class SmelterButton : ButtonBase
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected TMP_Text upgradeName;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Image levelLockImage;

        //[FoldoutGroup("Sprites", 0)]
        [SerializeField]
        [Required]
        protected Sprite lockSprite;

        public override void Refresh()
		{
			base.Refresh();
			upgradeName.text = listItem.UpgradeName;
			if (listItem.IsLevelLocked)
			{
				if (!levelLockImage.gameObject.activeSelf)
				{
					levelLockImage.gameObject.SetActive(true);
					levelLockImage.AnimateAlpha(1f, 0.1f, false, 0f, null);
				}
				upgradePrice.text = string.Format("lvl {0}", listItem.CurrentLockLevel);
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
				upgradePrice.text = listItem.PriceText;
			}
			upgradeButtonImage.sprite = ((listItem.IsLocked || listItem.IsLevelLocked) ? lockSprite : listItem.ButtonImage);
			GameObject gameObject;
			(gameObject = upgradeButtonImage.gameObject).SetActive(upgradeButtonImage.sprite != null);
            priceContainer.childAlignment = (gameObject.activeSelf ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter);
        }

	}
}
