using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private Transform camTransform;
    private Vector3 firstMyPos;
    private Vector3 firstCamPos;
    void Start()
    {
        camTransform = Camera.main.transform;
        firstCamPos = camTransform.position;
        firstMyPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, firstMyPos + (camTransform.position - firstCamPos) * 32 / 44, Time.deltaTime * 50);
    }
}
