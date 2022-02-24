using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
	public class TransformRatioController : MonoBehaviour
	{
		public float CurrentSizeNormalized { get; set; }

		private void OnEnable()
		{
			Transform parent = base.transform.parent;
			List<Transform> list = new List<Transform>();
			while (parent != null)
			{
				list.Add(parent);
				parent = parent.parent;
			}
			this.allParents = list.ToArray();
			this.CurrentSizeNormalized = 1f;
		}

		private void Update()
		{
			this.CorrectAspectRatio();
		}

		private Vector2 GetParentWorldScale()
		{
			Vector2 one = Vector2.one;
			for (int i = 0; i < this.allParents.Length; i++)
			{
				if (this.allParents[i])
				{
					one.Scale(this.allParents[i].localScale);
				}
			}
			return one;
		}

		private void CorrectAspectRatio()
		{
			this.parentScale = this.GetParentWorldScale();
			Vector2 vector = (this.parentScale.x > this.parentScale.y) ? new Vector2(this.parentScale.y / this.parentScale.x, 1f) : new Vector2(1f, this.parentScale.x / this.parentScale.y);
			base.transform.localScale = new Vector2(this.InitialLocalScale.x * vector.x * this.CurrentSizeNormalized, this.InitialLocalScale.y * vector.y * this.CurrentSizeNormalized);
		}

		private Vector2 parentScale = Vector2.one;

		public Vector2 InitialLocalScale;

		private Transform[] allParents;
	}
}
