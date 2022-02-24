using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class ParticleSystemUtils
	{
		public static void SetColor([NotNull] this ParticleSystem particleSystem, Color newColor)
		{
			ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particleSystem.colorOverLifetime;
			Gradient gradient = new Gradient();
			gradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(newColor, 0f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(0f, 1f)
			});
			colorOverLifetime.color = gradient;
			ParticleSystem.Particle[] array = new ParticleSystem.Particle[particleSystem.main.maxParticles];
			int particles = particleSystem.GetParticles(array);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].startColor = newColor;
			}
			particleSystem.SetParticles(array, particles);
		}
	}
}
