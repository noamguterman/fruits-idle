using System;
using System.Collections.Generic;
using Game.GameScreen;
using UI;
using UnityEngine;
using Utilities;

namespace Game.Camera
{
	public class CameraParams
	{
		public Transform Camera;

		public TouchArea TouchArea;

		public IReadOnlyList<GameScreenController> ReachableGameScreens;

		public Reference<float> CameraSpeed;

		public Reference<int> DirectionSign;

		public Vector2 CamPosMinMax;

		public Action<GameScreenController> ActiveScreenChange;

		public Action StartMoveCamera;
	}
}
