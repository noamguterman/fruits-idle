using System;
using System.Collections;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game.Tube
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class TubeConnector : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private SpriteRenderer first;

        [SerializeField]
        [Required]
        private SpriteRenderer second;

        [SerializeField]
        [HideInInspector]
        private bool isVertical;

        [SerializeField]
        private float margin = 0;

        [UsedImplicitly]
		private string Name
		{
			get
			{
				if (!this.isVertical)
				{
					return "Horizontal";
				}
				return "Vectical";
			}
		}

		[Button("$Name")]
		private void SetMode()
		{
			this.isVertical = !this.isVertical;
		}

		private IEnumerator Start()
		{
			yield return null;
			SpriteRenderer component = base.GetComponent<SpriteRenderer>();
			if (this.isVertical)
			{
				float y = this.first.bounds.max.y - margin;
				float num = this.second.bounds.min.y - y;
				component.size = component.size.SetY(num / base.transform.localScale.y);
				base.transform.SetPositionY(y + num / 2f);
			}
			else
			{
				float x = this.first.bounds.max.x - margin;
				float num = this.second.bounds.min.x - x;
				component.size = component.size.SetX(num / base.transform.localScale.x);
				base.transform.SetPositionX(x + num / 2f);
			}
			yield break;
		}

	}
}
