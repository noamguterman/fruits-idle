using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class CameraUtils
	{
		public static Camera MainCamera
		{
			get
			{
				if (!CameraUtils.mainCamera)
				{
					return CameraUtils.mainCamera = Camera.main;
				}
				return CameraUtils.mainCamera;
			}
		}

		public static bool IsInMainCameraViewport(Vector3 pos)
		{
			Vector3 vector = CameraUtils.MainCamera.WorldToViewportPoint(pos);
			return vector.x < 1f && vector.x > 0f && vector.y > 0f && vector.y < 1f;
		}

		public static Bounds Bounds([NotNull] this Camera camera)
		{
			Bounds result = new Bounds
			{
				center = camera.transform.position,
				extents = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize)
			};
			result.max = result.center + result.extents;
			result.min = result.center - result.extents;
			result.size = result.extents * 2f;
			return result;
		}

		private static Camera mainCamera;
	}
}
