using System;
using Game.Audio;
using Sirenix.OdinInspector;
using TMPro;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.BottomList.Buttons
{
	public abstract class ButtonBase : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Button upgradeButton;

        [SerializeField]
        [Required]
        protected Button upgradeButton_Listener;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected RectTransform buttonRect;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected HorizontalLayoutGroup priceContainer;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected TMP_Text upgradeValue;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected TMP_Text upgradePrice;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Image priceImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Image upgradeButtonImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        protected Image upgradeImage;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        protected SoundSettings soundSettings;

        private bool isSetEnabled;

        protected ListItem listItem;

        protected Vector2 initialButtonImagePos;

        protected Vector2 initialButtonImageAnchorMin;

        protected Vector2 initialButtonImageAnchorMax;

        protected Vector2 initialButtonImagePivot;

        private MultipleCoroutine buttonPulseScaleRoutine;

        private Transform tr;

        public Image UpgradeImage
		{
			get
			{
				return upgradeImage;
			}
		}

		public event Action<ButtonBase, bool> ButtonStateChange;

		public event Action ButtonClick;

		private Transform Transform
		{
			get
			{
				if (!tr)
				{
					return tr = transform;
				}
				return tr;
			}
		}

		public TabController TabController { get; protected set; }

		public Vector2 Size
		{
			get
			{
				return new Vector2
				{
					x = buttonRect.rect.size.x * Transform.localScale.x,
					y = buttonRect.rect.size.y * Transform.localScale.y
				};
			}
		}

		public RectTransform ButtonRect
		{
			get
			{
				return buttonRect;
			}
			set
			{
				buttonRect = value;
			}
		}

		public Vector2 AnchoredPosition
		{
			get
			{
				return buttonRect.anchoredPosition;
			}
			set
			{
				buttonRect.anchoredPosition = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				return Transform.position;
			}
			set
			{
				Transform.position = value;
			}
		}

		private void Awake()
		{
			RectTransform rectTransform = upgradeButtonImage.rectTransform;
			initialButtonImagePos = rectTransform.anchoredPosition;
			initialButtonImageAnchorMin = rectTransform.anchorMin;
			initialButtonImageAnchorMax = rectTransform.anchorMax;
			initialButtonImagePivot = rectTransform.pivot;
		}

		protected void InvokeButtonClick()
		{
			Action buttonClick = ButtonClick;
			if (buttonClick == null)
			{
				return;
			}
			buttonClick();
		}

		protected void InvokeButtonStateChange(bool isEnabled)
		{
			Action<ButtonBase, bool> buttonStateChange = ButtonStateChange;
			if (buttonStateChange == null)
			{
				return;
			}
			buttonStateChange(this, isEnabled);
		}

		public virtual void Initialize(ListItem listItem, TabController tabController)
		{
            //Debug.Log(listItem.UpgradeName + "  " + listItem.IsEnabled);
			this.listItem = listItem;
			TabController = tabController;
			gameObject.name = listItem.UpgradeName;
            if(gameObject.name == "Mixer Time" || gameObject.name == "Marketing")
            {
                gameObject.GetComponent<Image>().color = new Color(1, 220f / 255, 240f / 255, 1);
            }
			gameObject.SetActive(isSetEnabled = listItem.IsEnabled);
			this.listItem = listItem;
			this.listItem.Refresh += Refresh;
			this.listItem.Button = this;
			Refresh();
			upgradeButton_Listener.onClick.AddListener(delegate()
			{
				listItem.OnButtonClick();
				Refresh();
				InvokeButtonClick();
                upgradeButton.GetComponent<Coffee.UIExtensions.UIShiny>().Play();
				AudioSourceListener audioSource = soundSettings.GetAudioSource(listItem.ButtonClickSound);

                for (int i = 0; i < LiquidColors.Instance.FruitName.Length; i++)
                {
                    if (listItem.UpgradeName.Contains(LiquidColors.Instance.FruitName[i]))
                    {
                        audioSource = soundSettings.GetAudioSource(SoundType.UIBuyFruitClick);
                        break;
                    }
                }

                if (audioSource != null)
				{
					audioSource.PlayOneShot();
				}
				if (SoundSettings.IsVibrationOn)
				{
					iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
				}
			});
			if (listItem.UpgradeImage == null)
			{
				return;
			}
			upgradeImage.sprite = listItem.UpgradeImage;
			if (listItem.UpgradeImage)
			{
				upgradeImage.preserveAspect = true;
			}
		}

		public virtual void Refresh()
		{
			if (isSetEnabled && !listItem.IsEnabled)
			{
				isSetEnabled = false;
				InvokeButtonStateChange(false);
				return;
			}
			if (!isSetEnabled)
			{
				if (!listItem.IsEnabled)
				{
					return;
				}
				isSetEnabled = true;
				InvokeButtonStateChange(true);
			}
			if (string.IsNullOrEmpty(listItem.PriceText))
			{
				RectTransform rectTransform = upgradeButtonImage.rectTransform;
				rectTransform.anchorMin = Vector2.one * 0.5f;
				rectTransform.anchorMax = Vector2.one * 0.5f;
				rectTransform.pivot = Vector2.one * 0.5f;
				upgradeButtonImage.rectTransform.anchoredPosition = initialButtonImagePos.SetX(0f);
			}
			else
			{
				RectTransform rectTransform2 = upgradeButtonImage.rectTransform;
				rectTransform2.anchorMin = initialButtonImageAnchorMin;
				rectTransform2.anchorMax = initialButtonImageAnchorMax;
				rectTransform2.pivot = initialButtonImagePivot;
				rectTransform2.anchoredPosition = initialButtonImagePos;
			}
			if (!listItem.IsAvailable)
			{
				upgradeButtonImage.gameObject.SetActive(false);
			}

            upgradeButton_Listener.interactable = (listItem.IsPurchasable && listItem.IsAvailable && !listItem.IsLevelLocked);
            upgradeButton.interactable = (listItem.IsPurchasable && listItem.IsAvailable && !listItem.IsLevelLocked);
			if (listItem.PulseButtonWhenAvailable)
			{
				if (upgradeButton.interactable)
				{
					if (buttonPulseScaleRoutine == null)
					{
						MultipleCoroutine multipleCoroutine = new MultipleCoroutine(this);
						multipleCoroutine.Add(() => TransformUtils.LerpScale(upgradeButton.transform, Vector3.one * 1.1f, 0.2f, null, null));
						multipleCoroutine.Add(() => TransformUtils.LerpScale(upgradeButton.transform, Vector3.one * 1f, 0.2f, null, null));
						MultipleCoroutine multipleCoroutine2 = multipleCoroutine;
						buttonPulseScaleRoutine = multipleCoroutine;
						multipleCoroutine2.StartCoroutines(true);
					}
				}
				else if (buttonPulseScaleRoutine != null)
				{
					MultipleCoroutine multipleCoroutine3 = buttonPulseScaleRoutine;
					if (multipleCoroutine3 != null)
					{
						multipleCoroutine3.Dispose();
					}
					buttonPulseScaleRoutine = null;
					upgradeButton.transform.MoveScale(Vector3.one, 0.1f, null);
				}
			}
			priceImage.sprite = listItem.PriceImage;
			priceImage.gameObject.SetActive(priceImage.sprite != null);
		}

	}
}
