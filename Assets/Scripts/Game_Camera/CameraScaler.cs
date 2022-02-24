using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game.Camera
{
	public class CameraScaler : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private List<Renderer> renderers;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private bool inheritX;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private bool inheritY;

        private void Awake()
		{
			if (!CameraUtils.MainCamera.orthographic)
			{
				return;
			}
			Vector2 vector = new Vector2(renderers.Max((Renderer s) => s.bounds.size.x), renderers.Max((Renderer s) => s.bounds.size.y));
			float a = vector.x / (CameraUtils.MainCamera.aspect * 2f);
			float num = vector.y / 2f;
			if (inheritX && inheritY)
			{
				CameraUtils.MainCamera.orthographicSize = Mathf.Max(a, num);
				return;
			}
			if (inheritX)
			{
				CameraUtils.MainCamera.orthographicSize = Mathf.Max(a, CameraUtils.MainCamera.orthographicSize);
				return;
			}
			if (inheritY)
			{
				CameraUtils.MainCamera.orthographicSize = Mathf.Max(num, CameraUtils.MainCamera.orthographicSize);
			}
		}

	}
}
