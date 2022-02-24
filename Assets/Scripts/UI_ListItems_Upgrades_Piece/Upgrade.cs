using System;
using UnityEngine;

namespace UI.ListItems.Upgrades.Piece
{
	[Serializable]
	public class Upgrade : IEquatable<Upgrade>
	{
		public int Level
		{
			get
			{
				return this.level;
			}
		}

		public float Value
		{
			get
			{
				return this.value;
			}
		}

		public float PriceOfLevelUp
		{
			get
			{
				return this.priceOfLevelUp;
			}
			set
			{
				this.priceOfLevelUp = value;
			}
		}

		public Upgrade(int level, float priceOfLevelUp, float value)
		{
			this.level = level;
			this.priceOfLevelUp = priceOfLevelUp;
			this.value = value;
		}

		public Upgrade()
		{
		}

		public bool Equals(Upgrade other)
		{
			return other != null && this.level == other.level && this.priceOfLevelUp == other.priceOfLevelUp && this.value == other.value;
		}

		[SerializeField]
		private int level;

		[SerializeField]
		private float priceOfLevelUp;

		[SerializeField]
		private float value;
	}
}
