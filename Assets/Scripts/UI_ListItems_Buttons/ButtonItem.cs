using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UI.BottomList.Buttons;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	public abstract class ButtonItem
	{
        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected string upgradeName;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected string valueType;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected Sprite upgradeImage;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected Sprite priceImage;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected Sprite buttonImage;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected bool pulseButtonWhenAvailable;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        protected bool useCustomButtonPrefab;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        [ShowIf("useCustomButtonPrefab", true)]
        [AssetsOnly]
        protected ButtonBase buttonPrefab;

        [SerializeField]
        [ShowIf("UseTimer", true)]
        private Timer.OutputType timerOutputType = Timer.OutputType.Seconds;

        [SerializeField]
        [ShowIf("UseTimer", true)]
        private string timerAdditionalOutputString;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        private SoundType buttonClickSound = SoundType.UIBuyItemClick;

        [SerializeField]
        [ShowIf("ShowButtonOptions", true)]
        private int order;

        private readonly string valueIdentifier = "[VALUE]";

        [UsedImplicitly]
		public virtual string Name
		{
			get
			{
				return this.upgradeName;
			}
		}

		[UsedImplicitly]
		public virtual bool ShowButtonOptions
		{
			get
			{
				return true;
			}
		}

		[UsedImplicitly]
		public virtual bool ShowOptionsButton
		{
			get
			{
				return true;
			}
		}

		[UsedImplicitly]
		public virtual bool UseTimer
		{
			get
			{
				return false;
			}
		}

		public virtual string UpgradeName
		{
			get
			{
                return this.upgradeName;
			}
		}

		public virtual string ValueType
		{
			get
			{
				return this.valueType;
			}
		}

		public virtual Sprite UpgradeImage
		{
			get
			{
				return this.upgradeImage;
			}
		}

		public virtual Sprite ButtonImage
		{
			get
			{
				return this.buttonImage;
			}
		}

		public virtual Sprite PriceImage
		{
			get
			{
				return this.priceImage;
			}
		}

		public virtual SoundType ButtonClickSound
		{
			get
			{
				return this.buttonClickSound;
			}
		}

		public virtual int Order
		{
			get
			{
				return this.order;
			}
		}

		public virtual Timer.OutputType TimerOutputType
		{
			get
			{
				return this.timerOutputType;
			}
		}

		public bool PulseButtonWhenAvailable
		{
			get
			{
				return this.pulseButtonWhenAvailable;
			}
		}

		public bool UseCustomButtonPrefab
		{
			get
			{
				return this.useCustomButtonPrefab;
			}
		}

		public ButtonBase ButtonPrefab
		{
			get
			{
				return this.buttonPrefab;
			}
		}

		public string TimerAdditionalOutputString
		{
			get
			{
				return this.timerAdditionalOutputString;
			}
		}

		protected void SetButtonSettings(ButtonItem buttonItem)
		{
			this.upgradeName = buttonItem.upgradeName;
            this.valueType = buttonItem.valueType;
			this.upgradeImage = buttonItem.upgradeImage;
			this.buttonImage = buttonItem.buttonImage;
			this.buttonClickSound = buttonItem.buttonClickSound;
            //Debug.Log("------"+upgradeName + "   " + valueType);
           
			this.order = buttonItem.order;
			this.pulseButtonWhenAvailable = buttonItem.pulseButtonWhenAvailable;
			this.useCustomButtonPrefab = buttonItem.useCustomButtonPrefab;
			this.buttonPrefab = buttonItem.buttonPrefab;
		}

		
	}
}
