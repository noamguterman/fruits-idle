using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Tube
{
	public class PositionInheritor : MonoBehaviour
	{
		public event Action PositionInherited;

		[Button("Set Offset")]
		private void SettOffset()
		{
			this.offset = base.transform.position - this.target.position;
		}

		private IEnumerator Start()
		{
			yield return null;
			PositionInheritor component = this.target.GetComponent<PositionInheritor>();
			if (component)
			{
				if (component.isPositionInherited)
				{
					this.InheritPosition();
				}
				else
				{
					component.PositionInherited += this.InheritPosition;
				}
			}
			else
			{
				this.InheritPosition();
			}
			yield break;
		}

		private void InheritPosition()
		{
			Transform transform = base.transform;
			Vector3 vector = transform.position;
			Vector3 position = this.target.position;
			vector = new Vector3
			{
				x = (this.inheritX ? position.x : vector.x),
				y = (this.inheritY ? position.y : vector.y),
				z = (this.inheritZ ? position.z : vector.z)
			};
			transform.position = vector + this.offset;
			this.isPositionInherited = true;
			Action positionInherited = this.PositionInherited;
			if (positionInherited == null)
			{
				return;
			}
			positionInherited();
		}

		[SerializeField]
		[Required]
		private Transform target;

		[SerializeField]
		private Vector3 offset;

		[Space(25f)]
		[SerializeField]
		private bool inheritX;

		[SerializeField]
		private bool inheritY;

		[SerializeField]
		private bool inheritZ;

		private bool isPositionInherited;
	}
}
