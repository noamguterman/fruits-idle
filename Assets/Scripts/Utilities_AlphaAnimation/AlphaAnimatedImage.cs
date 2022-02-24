using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(Image))]
	public class AlphaAnimatedImage : Renderable
	{
		public override Color Color
		{
			get
			{
				return (this.image ? this.image : (this.image = base.GetComponent<Image>())).color;
			}
			set
			{
				(this.image ? this.image : (this.image = base.GetComponent<Image>())).color = value;
				if (value.a <= 0f && this.image.enabled)
				{
					base.DisabledFromAnimator = true;
					this.image.enabled = false;
					return;
				}
				if (base.DisabledFromAnimator && value.a > 0f && !this.image.enabled)
				{
					this.image.enabled = true;
				}
			}
		}

		private Image image;
	}
}
