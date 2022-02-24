using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game.GameScreen
{
	[Serializable]
	public class GameScreenSetting
    {
        [SerializeField]
        private GameScreenController gameScreen;

        [SerializeField]
        private bool inheritScreenHeight = true;

        [SerializeField]
        private bool inheritScreenWidth = true;

        [SerializeField]
        [HideIf("inheritScreenHeight", true)]
        private float gameScreenHeight;

        [SerializeField]
        [HideIf("inheritScreenWidth", true)]
        private float gameScreenWidth;

        [SerializeField]
        [Space(20f)]
        private bool isReachable = true;

        public Vector2 GameScreenSize
		{
			get
			{
				return new Vector2
				{
					x = (inheritScreenWidth ? (CameraUtils.MainCamera.orthographicSize * CameraUtils.MainCamera.aspect * 2f) : gameScreenWidth),
					y = (inheritScreenHeight ? (CameraUtils.MainCamera.orthographicSize * 2f) : gameScreenHeight)
				};
			}
		}

		public bool IsReachable
		{
			get
			{
				return isReachable;
			}
		}

		public GameScreenController GameScreen
		{
			get
			{
				return gameScreen;
			}
		}

	}
}
