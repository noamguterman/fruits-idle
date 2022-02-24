using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Components
{
	[AddComponentMenu("Random Development/FPS Counter")]
	[RequireComponent(typeof(Canvas))]
	public class FpsCounter : MonoBehaviour
	{
		private void Awake()
		{
			GameObject gameObject = new GameObject("FPS Text", new Type[]
			{
				typeof(Text)
			});
			gameObject.transform.SetParent(base.transform);
			this.fpsText = gameObject.GetComponent<Text>();
			this.fpsText.raycastTarget = false;
			this.fpsText.text = "FPS";
			this.fpsText.resizeTextForBestFit = true;
			this.fpsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			this.fpsText.rectTransform.anchorMin = new Vector2(1f, 1f);
			this.fpsText.rectTransform.anchorMax = new Vector2(1f, 1f);
			this.fpsText.rectTransform.pivot = new Vector2(1f, 1f);
			this.fpsText.rectTransform.sizeDelta = new Vector2(120f, 100f);
			this.fpsText.rectTransform.localScale = new Vector2(1f, 1f);
			this.fpsText.resizeTextMaxSize = 160;
			GameObject gameObject2 = new GameObject("AVG FPS Text", new Type[]
			{
				typeof(Text)
			});
			gameObject2.transform.SetParent(base.transform);
			this.averageFpsText = gameObject2.GetComponent<Text>();
			this.averageFpsText.raycastTarget = false;
			this.averageFpsText.text = "AVG FPS";
			this.averageFpsText.resizeTextForBestFit = true;
			this.averageFpsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			this.averageFpsText.rectTransform.anchorMin = new Vector2(1f, 1f);
			this.averageFpsText.rectTransform.anchorMax = new Vector2(1f, 1f);
			this.averageFpsText.rectTransform.pivot = new Vector2(1f, 1f);
			this.averageFpsText.rectTransform.sizeDelta = new Vector2(120f, 100f);
			this.averageFpsText.rectTransform.localScale = new Vector2(1f, 1f);
			this.averageFpsText.resizeTextMaxSize = 160;
			this.averageFpsText.rectTransform.localPosition = new Vector2(this.fpsText.rectTransform.localPosition.x, this.fpsText.rectTransform.localPosition.y - (float)this.fpsText.resizeTextMaxSize);
		}

		private void Update()
		{
			if (this.msLoadSimulationOnFrame != 0)
			{
				Thread.Sleep(this.msLoadSimulationOnFrame);
			}
			int num = (int)(1f / Time.deltaTime);
			this.fpsSamples[this.lastWirttenFpsSampleIndex] = num;
			this.lastWirttenFpsSampleIndex++;
			if (this.lastWirttenFpsSampleIndex >= this.fpsSamples.Length)
			{
				this.lastWirttenFpsSampleIndex = 0;
				float num2 = 0f;
				for (int i = 0; i < this.fpsSamples.Length; i++)
				{
					num2 += (float)this.fpsSamples[i];
				}
				this.averageFpsText.text = (num2 / (float)this.fpsSamples.Length).ToString();
			}
			if (num >= this.alwaysDisplayFpsLowerThan && this.fpsSkipCounter < this.amountOfFramesToSkip)
			{
				this.fpsSkipCounter++;
				return;
			}
			this.fpsSkipCounter = 0;
			this.fpsText.text = num.ToString();
		}

		[SerializeField]
		private int amountOfFramesToSkip = 3;

		[SerializeField]
		private int alwaysDisplayFpsLowerThan = 21;

		[SerializeField]
		private int msLoadSimulationOnFrame;

		private Text fpsText;

		private int fpsSkipCounter;

		private Text averageFpsText;

		private int lastWirttenFpsSampleIndex;

		private int[] fpsSamples = new int[100];
	}
}
