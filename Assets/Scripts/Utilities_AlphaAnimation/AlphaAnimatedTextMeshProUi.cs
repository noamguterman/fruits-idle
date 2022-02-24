using System;
using TMPro;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class AlphaAnimatedTextMeshProUi : Renderable
	{
		public override Color Color
		{
			get
			{
				return this.TextMesh.color;
			}
			set
			{
				this.TextMesh.color = value;
				if (value.a <= 0f && this.textMesh.enabled)
				{
					this.textMesh.enabled = false;
					base.DisabledFromAnimator = true;
					return;
				}
				if (base.DisabledFromAnimator && value.a > 0f && !this.textMesh.enabled)
				{
					this.textMesh.enabled = true;
				}
			}
		}

		private TextMeshProUGUI TextMesh
		{
			get
			{
				if (!this.textMesh)
				{
					return this.textMesh = base.GetComponent<TextMeshProUGUI>();
				}
				return this.textMesh;
			}
		}

		private TextMeshProUGUI textMesh;
	}
}
