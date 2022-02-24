using System;
using UnityEngine;

namespace Utilities
{
	[RequireComponent(typeof(Camera))]
	public class NoFog : MonoBehaviour
	{
		private void Start()
		{
			this.isFogEnabled = RenderSettings.fog;
		}

		private void OnPreRender()
		{
			RenderSettings.fog = false;
		}

		private void OnPostRender()
		{
			RenderSettings.fog = this.isFogEnabled;
		}

		private bool isFogEnabled;
	}
}
