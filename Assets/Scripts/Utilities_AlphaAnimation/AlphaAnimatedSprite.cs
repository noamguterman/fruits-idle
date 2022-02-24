using System;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class AlphaAnimatedSprite : Renderable
	{
		public override Color Color
		{
			get
			{
				return this.SpriteRenderer.color;
			}
			set
			{
				this.SpriteRenderer.color = value;
			}
		}

		private SpriteRenderer SpriteRenderer
		{
			get
			{
				SpriteRenderer result;
				if ((result = this.spriteRenderer) == null)
				{
					result = (this.spriteRenderer = base.GetComponent<SpriteRenderer>());
				}
				return result;
			}
		}

		private SpriteRenderer spriteRenderer;
	}
}
