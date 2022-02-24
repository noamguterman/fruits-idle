using System;
using UnityEngine;

namespace Utilities
{
	public class CollisionEvent : MonoBehaviour
	{
		public event Action<Collision> CollisionEnter;

		public event Action<Collision2D> CollisionEnter2D;

		public event Action<Collider2D> TriggerEnter2D;

		public event Action<Collider> TriggerEnter;

		public event Action<Collider> TriggerExit;

		private void OnCollisionEnter(Collision other)
		{
			Action<Collision> collisionEnter = this.CollisionEnter;
			if (collisionEnter == null)
			{
				return;
			}
			collisionEnter(other);
		}

		private void OnTriggerEnter(Collider other)
		{
			Action<Collider> triggerEnter = this.TriggerEnter;
			if (triggerEnter == null)
			{
				return;
			}
			triggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			Action<Collider> triggerExit = this.TriggerExit;
			if (triggerExit == null)
			{
				return;
			}
			triggerExit(other);
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			Action<Collision2D> collisionEnter2D = this.CollisionEnter2D;
			if (collisionEnter2D == null)
			{
				return;
			}
			collisionEnter2D(other);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			Action<Collider2D> triggerEnter2D = this.TriggerEnter2D;
			if (triggerEnter2D == null)
			{
				return;
			}
			triggerEnter2D(other);
		}
	}
}
