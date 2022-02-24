using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Preloade_Text : MonoBehaviour
{
    public string[] logoString;
    public Transform bubble;
    public Text logoText;
    string logoStr;
    void Start()
    {
        bubble.localScale = Vector3.zero;
        logoText.text = "";
        transform.localScale = Vector3.one;

        PlayerPrefs.SetInt("PlayingTime", PlayerPrefs.GetInt("PlayingTime", 0) + 1);

        switch(PlayerPrefs.GetInt("PlayingTime", 0))
        {
            case 1:
                logoStr = logoString[0];
                break;
            case 2:
                logoStr = logoString[1];
                break;
            case 3:
                logoStr = logoString[2];
                break;
            default:
                int rnd = Random.Range(3, logoString.Length);
                logoStr = logoString[rnd];
                break;
        }

        StartCoroutine(ShowBubble());
    }

    IEnumerator ShowBubble()
    {
        yield return new WaitForSeconds(0.1f);
        bubble.DOScale(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(PlayText());
    }

    IEnumerator PlayText()
    {
        foreach (char c in logoStr)
        {
            logoText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

}
