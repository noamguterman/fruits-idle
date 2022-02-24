using System;
using UnityEngine;
using Utilities;

namespace Game
{
	[RequireComponent(typeof(Rigidbody))]
	public abstract class Costable : MonoBehaviour
    {
        private Rigidbody costableRigidbody;

        public event Action<Costable> AddMoney;

		public virtual float Cost { get; set; }

		public virtual MoneyType MoneyType { get; set; }

		public Rigidbody Rigidbody
		{
			get
			{
				if (!costableRigidbody)
				{
					return costableRigidbody = GetComponent<Rigidbody>();
				}
				return costableRigidbody;
			}
		}

		public Vector3 Position
		{
			get
			{
				return transform.position;
			}
			set
			{
				transform.position = value;
			}
		}

		public virtual void Push(bool addMoney = false)
		{
			if (addMoney)
			{
				Action<Costable> addMoney2 = AddMoney;
				addMoney2(this);
			}
			Position = Vector3.one * 100f;
			StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
			{
				this.Push<Costable>();
			}, new WaitForFixedUpdate()));
		}

		protected virtual void OnDisable()
		{
			AddMoney = null;
		}

	}
}
