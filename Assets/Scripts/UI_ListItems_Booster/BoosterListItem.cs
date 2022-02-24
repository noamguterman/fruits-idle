using System;
using System.Linq;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;

namespace UI.ListItems.Booster
{
	[Serializable]
	public abstract class BoosterListItem : ListItem
	{
		public event Action<BoosterType, float, float, bool> ChangeBoosterTypeMutiplier;

		public override bool UseTimer
		{
			get
			{
				return true;
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
				return string.Format("{0}{1} for {2} sec", this.multiplier, this.ValueType, this.duration);
			}
		}

		public override bool IsAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsLocked
		{
			get
			{
				return false;
			}
		}

		public virtual void Initialize(BoostersSettings boostersSettings)
		{
			this.isActive = false;
			this.ChangeBoosterTypeMutiplier = null;
			boostersSettings.ChangeBoosterTypeMutiplier += delegate(BoosterType type, float f, float t, bool isActive)
			{
				if (type == this.boosterType)
				{
					this.isActive = isActive;
					base.InvokeUIRefresh();
				}
			};
		}

		protected void InvokeChangeBoosterTypeMutiplier(bool isEnabled)
		{
			Action<BoosterType, float, float, bool> changeBoosterTypeMutiplier = this.ChangeBoosterTypeMutiplier;
			if (changeBoosterTypeMutiplier == null)
			{
				return;
			}
			changeBoosterTypeMutiplier(this.boosterType, this.multiplier, this.duration, isEnabled);
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
			base.SetButtonSettings(buttonsSettings.ButtonItemsBoosters.FirstOrDefault((ButtonItemBooster b) => b.BoosterType == this.boosterType));
		}

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		protected BoosterType boosterType;

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		protected float multiplier;

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		protected float duration;

		protected bool isActive;
	}
}
