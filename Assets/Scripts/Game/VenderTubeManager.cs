using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VenderTubeManager : MonoBehaviour
{
    public GameObject[] tubes;
    public float animSpeed;
    public Color fromColor;
    public Color toColor;

    private bool isTurnon = false;
    private DG.Tweening.Sequence[] seq = new Sequence[2];
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < tubes.Length; i++)
        {
            seq[i] = DOTween.Sequence();
        }
    }

    public void PlayTubeAnim()
    {
        for (int i = 0; i < tubes.Length; i++)
        {
            seq[i].Append(tubes[i].GetComponent<SpriteRenderer>().DOColor(toColor, 1 / animSpeed)).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void PlayOnceAnim()
    {
        if (isTurnon == true)
            return;

        isTurnon = true;

        StartCoroutine(ChangeTubeColor());
    }

    IEnumerator ChangeTubeColor()
    {
        Color targetColor = Color.white;
        while (true)
        {
            targetColor = LiquidColors.Instance.fruitColors[Random.Range(0, LiquidColors.Instance.fruitColors.Length)];
            for (int i = 0; i < tubes.Length; i++)
            {
                tubes[i].GetComponent<SpriteRenderer>().DOColor(targetColor, 0.5f);
            }
            yield return new WaitForSeconds(5);
        }
    }

    public void ChangeTubeAnimSpeed(float speed)
    {
        for (int i = 0; i < tubes.Length; i++)
        {
            seq[i].duration = 1 / speed;
        }
    }
}