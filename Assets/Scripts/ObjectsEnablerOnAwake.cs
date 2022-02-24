using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsEnablerOnAwake : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToEnableOnAwake;

    private void Awake()
	{
		this.objectsToEnableOnAwake.ForEach(delegate(GameObject o)
		{
			o.SetActive(true);
		});
	}

}
