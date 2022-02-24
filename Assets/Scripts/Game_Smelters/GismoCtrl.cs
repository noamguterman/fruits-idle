using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GismoCtrl : MonoBehaviour
{
    public GameObject[] lightObjs;
    void Start()
    {
        TurnOffLight();
    }
    public void TurnOnLight(int curVal, int maxVal)
    {
        float percent = (float)curVal / (float)maxVal;
        percent = Mathf.Min(1, percent);
        int openLightCnt = (int)(lightObjs.Length * percent);

        TurOnLights(openLightCnt);
    }

    public void TurnOffLight()
    {
        for (int i = 0; i < lightObjs.Length; i++)
            lightObjs[i].SetActive(false);
    }

    private void TurOnLights(int lightCnt)
    {
        for(int i = 0; i < lightObjs.Length; i++)
        {
            if (i < lightCnt)
            {
                lightObjs[i].SetActive(true);
            }
            else
            {
                lightObjs[i].SetActive(false);
            }
        }
    }
    

}
