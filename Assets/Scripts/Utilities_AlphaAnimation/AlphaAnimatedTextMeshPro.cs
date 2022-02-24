using System;
using TMPro;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(TextMeshPro))]
	public class AlphaAnimatedTextMeshPro : Renderable
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
			}
		}

		private TextMeshPro TextMesh
		{
			get
			{
				TextMeshPro result;
				if ((result = this.textMesh) == null)
				{
					result = (this.textMesh = base.GetComponent<TextMeshPro>());
				}
				return result;
			}
		}

		private TextMeshPro textMesh;
	}
}
