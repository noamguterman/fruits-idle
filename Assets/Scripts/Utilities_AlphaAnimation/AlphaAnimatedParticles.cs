using System;
using UnityEngine;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(ParticleSystem))]
	public class AlphaAnimatedParticles : Renderable
	{
		private ParticleSystem ParticleEffect
		{
			get
			{
				ParticleSystem result;
				if ((result = this.particleEffect) == null)
				{
					result = (this.particleEffect = base.GetComponent<ParticleSystem>());
				}
				return result;
			}
		}

		public override Color Color
		{
			get
			{
				return new Color
				{
					r = this.ParticleEffect.colorOverLifetime.color.color.r,
					g = this.ParticleEffect.colorOverLifetime.color.color.g,
					b = this.ParticleEffect.colorOverLifetime.color.color.b,
					a = this.ParticleEffect.colorOverLifetime.color.gradient.alphaKeys[1].alpha
				};
			}
			set
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime = this.ParticleEffect.colorOverLifetime;
				ParticleSystem.MinMaxGradient color = colorOverLifetime.color;
				GradientAlphaKey[] alphaKeys = color.gradient.alphaKeys;
				for (int i = 1; i < alphaKeys.Length - 1; i++)
				{
					alphaKeys[i].alpha = value.a;
				}
				color.gradient.alphaKeys = alphaKeys;
				color.gradient.mode = GradientMode.Blend;
				colorOverLifetime.color = color;
			}
		}

		private ParticleSystem particleEffect;
	}
}
