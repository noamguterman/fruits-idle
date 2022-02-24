// dnSpy decompiler from Assembly-CSharp.dll class: DynamicParticle
using System;
using UnityEngine;

public class DynamicParticle1 : MonoBehaviour
{
    private Vector3 topCollider_Pos;

    private void Awake()
    {
        topCollider_Pos = GameObject.Find("Top Collider").transform.position;
    }

    private void FixedUpdate()
	{
        //if (transform.position.y < -3.5f)
        //    Destroy(gameObject);
	}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("OnCollisionEnter = " + collision.gameObject.name);
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("OnTriggerEnter = " + other.gameObject.name);
    //    if (other.gameObject.name == "Top Collider")
    //        Destroy(gameObject);
    //}

}
