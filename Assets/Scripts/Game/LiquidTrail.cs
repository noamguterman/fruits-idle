using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidTrail : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ShowTrail", 0.1f);
    }

    void ShowTrail()
    {
        transform.GetComponent<TrailRenderer>().enabled = true;
    }
}
