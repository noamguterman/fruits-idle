using System;
using UI.BottomList.Buttons;
using UI.ListItems.Buttons;
using Utilities;

namespace UI.ListItems
{
	public abstract class ListItem : ButtonItem
	{
		public event Action Refresh;

		public event Func<float, float, Timer> StartTimer;

		public ButtonBase Button { protected get; set; }

		public virtual bool IsEnabled
		{
            get
            {
                return true;
            }
		}

		public abstract string LevelText { get; }

		public abstract string ValueText { get; }

		public abstract string PriceText { get; }

		public abstract bool IsAvailable { get; }

		public abstract bool IsPurchasable { get; }

		public abstract bool IsLocked { get; }

		public bool IsLevelLocked
		{
			get
			{
				return GameData.CurrentLevel < this.CurrentLockLevel;
			}
		}

		public abstract int CurrentLockLevel { get; }

		public abstract string GetPrefsKey();

		public abstract void OnButtonClick();

		public abstract void SetButtonSettings(ButtonsSettings buttonsSettings);

		public void InvokeUIRefresh()
		{
			Action refresh = this.Refresh;
			if (refresh == null)
			{
				return;
			}
			refresh();
		}

		protected Timer InvokeUIStartTimer(float initialTime, float time)
		{
			Func<float, float, Timer> startTimer = this.StartTimer;
			if (startTimer == null)
			{
				return null;
			}
			return startTimer(initialTime, time);
		}

		protected Timer InvokeUIStartTimer(float time)
		{
			Func<float, float, Timer> startTimer = this.StartTimer;
			if (startTimer == null)
			{
				return null;
			}
			return startTimer(time, time);
		}

		private bool isEnabled;

		protected bool isBoolSet;
	}
}
