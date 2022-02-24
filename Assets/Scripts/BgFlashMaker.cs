using System;
using System.Collections;
using UnityEngine;

public class BgFlashMaker : MonoBehaviour
{
    [SerializeField]
    private Color flashColor = Color.white;

    [SerializeField]
    private float flashAppearTime = 0.05f;

    [SerializeField]
    private float flashDisappearTime = 0.5f;

    private Color defaultColor;

    private Coroutine flashingCoroutine;

    [Header("Assign one of this fields:")]
    [SerializeField]
    private Camera cameraComponent;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Color BgColor
	{
		get
		{
			if (!cameraComponent)
			{
				return spriteRenderer.color;
			}
			return cameraComponent.backgroundColor;
		}
		set
		{
			if (cameraComponent)
			{
				cameraComponent.backgroundColor = value;
				return;
			}
			spriteRenderer.color = value;
		}
	}

	private void Awake()
	{
		defaultColor = BgColor;
	}

	public void MakeFlash()
	{
		if (flashingCoroutine != null)
		{
			StopCoroutine(flashingCoroutine);
		}
		flashingCoroutine = StartCoroutine(FlashColorLerping(1f));
	}

	public void MakeFlash(float colorMultiplier)
	{
		if (flashingCoroutine != null)
		{
			StopCoroutine(flashingCoroutine);
		}
		flashingCoroutine = StartCoroutine(FlashColorLerping(colorMultiplier));
	}

	private IEnumerator FlashColorLerping(float colorMultiplier = 1f)
	{
		float timeElapsed = 0f;
		while (timeElapsed < flashAppearTime)
		{
			timeElapsed += Time.deltaTime;
			float t = timeElapsed / flashAppearTime;
			BgColor = Color.Lerp(defaultColor, flashColor * colorMultiplier, t);
			yield return null;
		}
		BgColor = flashColor * colorMultiplier;
		yield return null;
		timeElapsed = 0f;
		while (timeElapsed < flashDisappearTime)
		{
			timeElapsed += Time.deltaTime;
			float t2 = timeElapsed / flashDisappearTime;
			BgColor = Color.Lerp(flashColor * colorMultiplier, defaultColor, t2);
			yield return null;
		}
		BgColor = defaultColor;
		yield break;
	}

}
