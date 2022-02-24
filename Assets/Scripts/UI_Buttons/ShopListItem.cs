using System;
using JetBrains.Annotations;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using ToastPlugin;
using UI.ListItems;
using UI.ListItems.Buttons;
using UnityEngine;
using UnityEngine.Purchasing;


namespace UI.Buttons
{
	[Serializable]
	public sealed class ShopListItem : ListItem
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private ShopItemType shopItemType;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private int percentValue;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private bool useHotSticker;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("ShopItemIsMoney", true)]
        private MoneyType moneyType;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("ShopItemIsMoney", true)]
        private int amount;

        [SerializeField]
        private ProductType productType;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private string itemShopID;

        private IStoreController storeController;

        private ShopSettings shopSettings;

        [UsedImplicitly]
		private bool ShopItemIsMoney
		{
			get
			{
				return shopItemType == ShopItemType.Money;
			}
		}

		public bool UseHotSticker
		{
			get
			{
				return useHotSticker;
			}
		}

		public override string LevelText
		{
			get
			{
				return string.Empty;
			}
		}

		public override string ValueText
		{
			get
			{
				if (amount != 0)
				{
					return amount.ToString();
				}
				return string.Empty;
			}
		}

		public override string PriceText
		{
			get
			{
                IStoreController storeController = this.storeController;
                string text;
                if (storeController == null)
                {
                    text = null;
                }
                else
                {
                    Product product = storeController.products.WithID(this.itemShopID);
                    text = ((product != null) ? product.metadata.localizedPriceString : null);
                }
                string text2 = text;
                if (!string.IsNullOrWhiteSpace(text2))
                {
                    return text2;
                }
                return "Buy";
			}
		}

		public override bool IsAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsPurchasable
		{
			get
			{
				return !ShopSettings.IsShopItemPurchased(itemShopID) || this.productType != ProductType.NonConsumable;
			}
		}

		public override bool IsLocked
		{
			get
			{
				return false;
			}
		}

		public override bool IsEnabled
		{
			get
			{
                //Debug.LogError("ShowSetting - IsEnabled");
                return true;// shopSettings != null && shopSettings.IsInitialized;
			}
		}

		public int PercentValue
		{
			get
			{
				return percentValue;
			}
		}
        public ProductType ProductType
        {
            get
            {
                return this.productType;
            }
        }

        public string ItemShopID
		{
			get
			{
				return itemShopID;
			}
		}

		public ShopItemType ShopItemType
		{
			get
			{
				return shopItemType;
			}
		}

		public override int CurrentLockLevel
		{
			get
			{
				return 0;
			}
		}

		public override string GetPrefsKey()
		{
			return string.Empty;
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
		}

		public void GetReward()
		{
			ShopItemType shopItemType = this.shopItemType;
			if (shopItemType == ShopItemType.Money)
			{
				GameData.IncreaseMoney(amount, moneyType, true);
				return;
			}
			if (shopItemType == ShopItemType.NoAds)
			{
				ShopSettings.IsNoAdsPurchased = true;
				return;
			}
			throw new ArgumentOutOfRangeException();
		}

		public override void OnButtonClick()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
                ToastHelper.ShowToast("Internet connection unavailable.", false);
            }
			ShopSettings shopSettings = this.shopSettings;
			if (shopSettings == null)
			{
				return;
			}

            shopSettings.PurchaseShopItem(this.itemShopID);
        }

        public void Initialize(IStoreController storeController, ShopSettings shopSettings)
        {
            this.storeController = storeController;
            this.shopSettings = shopSettings;
            ShopSettings.ShopItemPurchased += delegate (string itemID)
            {
                if (itemID == this.itemShopID)
                {
                    base.InvokeUIRefresh();
                }
            };
            base.InvokeUIRefresh();
        }
    }
}
