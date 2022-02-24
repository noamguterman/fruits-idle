using System;
using System.Collections.Generic;
using System.Linq;
using ToastPlugin;
using UI.Buttons;
using UI.ListItems;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Utilities;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Shop Settings", fileName = "Shop Settings")]
	public class ShopSettings : Tab, IStoreListener
    {
        [SerializeField]
        private RareOreRewardedListItem rareOreRewarded;

        [SerializeField]
        private List<ShopListItem> shopListItems;

        private static List<ShopListItem> ShopListItems;

        private IStoreController storeController;

        private IAppleExtensions appleExtensions;

        public override IReadOnlyList<ListItem> GetTabItems()
		{
            return shopListItems;
		}

		public bool IsInitialized { get; private set; }

		public bool IsPurchaseInProgress { get; private set; }

		public bool IsRestorePurchasesAllowed
		{
			get
			{
				return Application.platform == RuntimePlatform.Android;
			}
		}

		public static bool IsNoAdsPurchased
		{
			get
			{
				return PlayerPrefs.GetInt("IsNoAdsPurchased", 0) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("IsNoAdsPurchased", value ? 1 : 0);
				Action noAdsPurchased = NoAdsPurchased;
				if (noAdsPurchased == null)
				{
					return;
				}
				noAdsPurchased();
			}
		}

		public static event Action<string> ShopItemPurchased;

		public static event Action NoAdsPurchased;

		public static bool IsShopItemPurchased(string shopItemID)
		{
			return PlayerPrefs.GetInt(shopItemID, 0) == 1;
		}

		private static void SetShopItemPurchase(string shopItemID)
		{
			PlayerPrefs.SetInt(shopItemID, 1);
			ShopListItems.First((ShopListItem s) => s.ItemShopID == shopItemID).GetReward();
		}

		public void Initialize()
		{
            ShopSettings.ShopListItems = this.shopListItems;
            StandardPurchasingModule first = StandardPurchasingModule.Instance();
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(first, Array.Empty<IPurchasingModule>());
            this.shopListItems.ForEach(delegate (ShopListItem shopItem)
            {
                Debug.Log("##########" + shopItem.ItemShopID + "   " + shopItem.ProductType + "   " + shopItem.IsEnabled);

                builder.AddProduct(shopItem.ItemShopID, shopItem.ProductType);
            });
            this.rareOreRewarded.Initialize();
            UnityPurchasing.Initialize(this, builder);
        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            ToastHelper.ShowToast("Purchase error: " + p.ToString().AddSpaces() + ".", true);
            this.IsPurchaseInProgress = false;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("Shop = OnInitialized");
            this.storeController = controller;
            this.appleExtensions = extensions.GetExtension<IAppleExtensions>();
            this.IsInitialized = true;
            this.shopListItems.ForEach(delegate (ShopListItem shopListItem)
            {
                shopListItem.Initialize(this.storeController, this);
            });
        }

        public void OnInitializeFailed(InitializationFailureReason reason)
        {
            UnityEngine.Debug.Log("==============" + reason);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            this.IsPurchaseInProgress = false;
            ShopSettings.SetShopItemPurchase(args.purchasedProduct.definition.id);
            Action<string> shopItemPurchased = ShopSettings.ShopItemPurchased;
            if (shopItemPurchased != null)
            {
                shopItemPurchased(args.purchasedProduct.definition.id);
            }
            return PurchaseProcessingResult.Complete;
        }

        public void PurchaseShopItem(string shopItemID)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ToastHelper.ShowToast("Internet connection unavailable.", false);
                return;
            }
            if (!this.IsInitialized || this.IsPurchaseInProgress)
            {
                return;
            }
            this.IsPurchaseInProgress = true;
            this.storeController.InitiatePurchase(shopItemID);
        }

        public void RestorePurchases()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ToastHelper.ShowToast("Internet connection unavailable.", false);
                return;
            }
            if (this.IsRestorePurchasesAllowed)
            {
                IAppleExtensions appleExtensions = this.appleExtensions;
                if (appleExtensions == null)
                {
                    return;
                }
                appleExtensions.RestoreTransactions(delegate (bool success)
                {
                    ToastHelper.ShowToast(success ? "Purchases restored." : "Purchases restoration error.", false);
                });
            }
        }
    }
}
