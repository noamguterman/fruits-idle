using System;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(MeshRenderer))]
	public class AlphaAnimatedMesh : Renderable
	{
		public override Color Color
		{
			get
			{
				return (this.meshRenderer ? this.meshRenderer : (this.meshRenderer = base.GetComponent<MeshRenderer>())).material.GetColor("_Color");
			}
			set
			{
				(this.meshRenderer ? this.meshRenderer : (this.meshRenderer = base.GetComponent<MeshRenderer>())).material.SetColor("_Color", value);
				if (value.a <= 0f && this.meshRenderer.enabled)
				{
					this.meshRenderer.enabled = false;
					return;
				}
				if (value.a > 0f && !this.meshRenderer.enabled)
				{
					this.meshRenderer.enabled = true;
				}
			}
		}

		private MeshRenderer meshRenderer;
	}
}
