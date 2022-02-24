using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HandAnimation : MonoBehaviour
{
    private Color toColor = new Color(1, 1, 1, 1);
    public Text text;
    public string str;
    public float activeTime = 0;
    public float deactiveTime = 5;
    void Start()
    {
        Invoke("Action", activeTime);
    }

    private void Action()
    {
        if (PlayerPrefs.GetInt(str, 0) == 0)
        {
            GetComponent<Image>().DOColor(toColor, 0.5f).SetLoops(-1, LoopType.Yoyo);
            if (text != null)
                text.DOColor(Color.red, 0.5f);
        }
        else
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
            if (text != null)
                text.color = new Color(1, 0, 0, 0);
        }
    }

    private void Update()
    {
        if (Time.time < deactiveTime)
            return;

        //if (str != "isTapped")
        {
            PlayerPrefs.SetInt(str, 1);
        }

        if (PlayerPrefs.GetInt(str, 0) == 1)
        {
            this.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
