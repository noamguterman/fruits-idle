using System;
using Sirenix.OdinInspector;
using TMPro;
using UI.Buttons;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BottomList.Buttons
{
	public class ShopButton : ButtonBase
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image percentValuebackground;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image hotSticker;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TMP_Text percentValueText;

        private bool usePercentText;

        private ShopListItem shopListItem;

        public override void Initialize(ListItem listItem, TabController tabController)
		{
			base.Initialize(listItem, tabController);
			shopListItem = (listItem as ShopListItem);
			if (shopListItem == null)
			{
				return;
			}
			hotSticker.gameObject.SetActive(shopListItem.UseHotSticker);
			priceContainer.childAlignment = TextAnchor.MiddleCenter;
			usePercentText = (shopListItem.PercentValue > 0);
			percentValueText.SetText(string.Format("+{0}%", shopListItem.PercentValue));
			percentValuebackground.gameObject.SetActive(usePercentText);
		}

		public override void Refresh()
		{
			upgradeValue.text = listItem.ValueText;
			upgradePrice.text = listItem.PriceText;
			upgradeValue.text = listItem.ValueText;
			base.Refresh();
		}

	}
}
