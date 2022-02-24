using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HandAnimation1 : MonoBehaviour
{
    public string str;
    public float activeTime = 0;
    public float deactiveTime = 5;
    public bool isUp;
    private float initYPos;
    void Start()
    {
        Invoke("Action", activeTime);
        initYPos = transform.position.y;
    }

    private void Action()
    {
        if (PlayerPrefs.GetInt(str, 0) == 0)
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 1);
            if (isUp)
            {
                transform.DOMoveY(initYPos + 40, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                transform.DOMoveY(initYPos - 40, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
        }
        else
        {
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
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
