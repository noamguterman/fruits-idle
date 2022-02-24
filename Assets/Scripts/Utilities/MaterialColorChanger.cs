using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
	[RequireComponent(typeof(MeshRenderer))]
	public class MaterialColorChanger : MonoBehaviour
	{
		private void Awake()
		{
			this.meshRenderer = base.GetComponent<MeshRenderer>();
		}

		private void OnEnable()
		{
			float t = 0f;
			base.StartCoroutine(CoroutineUtils.LoopExecute(delegate()
			{
				this.meshRenderer.material.SetColor("_Color", this.colorGradient.Evaluate(t = ((t >= 1f) ? 0f : Mathf.Min(t + Time.deltaTime * this.colorChangeSpeed, 1f))));
			}, null));
		}

		private void OnDisable()
		{
			base.StopAllCoroutines();
		}

		//[FoldoutGroup("Settings", 0)]
		[SerializeField]
		private Gradient colorGradient;

		//[FoldoutGroup("Settings", 0)]
		[SerializeField]
		private float colorChangeSpeed;

		private MeshRenderer meshRenderer;
	}
}
