using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game.GameScreen
{
	[RequireComponent(typeof(RectTransform))]
	public class GameScreensManager : MonoBehaviour
	{
		public event Action<IReadOnlyList<GameScreenController>> GameScreensCreated;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private List<GameScreenSetting> gameScreens;

        private RectTransform rTransform;

        public float IntermediateZoneWidth
		{
			get
			{
				return gameScreens.FindAll((GameScreenSetting g) => g.GameScreen.SortLayer == "Game Screen Intermediate").Sum((GameScreenSetting g) => g.GameScreenSize.x);
			}
		}

		private void Awake()
		{
			SetScreensSize();
        }

        [Button("Set Screens Size")]
		public void SetScreensSize()
		{
			rTransform = GetComponent<RectTransform>();
			RectTransform rectTransform = rTransform;
			Vector2 sizeDelta = default(Vector2);
			sizeDelta.x = gameScreens.Sum((GameScreenSetting s) => s.GameScreenSize.x);
			sizeDelta.y = gameScreens.Max((GameScreenSetting s) => s.GameScreenSize.y);
			rectTransform.sizeDelta = sizeDelta;
			rTransform.position = Vector3.zero;
			GameScreenController prev = null;
			gameScreens.ForEach(delegate(GameScreenSetting s)
			{
				s.GameScreen.Transform.localPosition = new Vector2
				{
					x = ((prev == null) ? (s.GameScreenSize.x / 2f - rTransform.sizeDelta.x / 2f) : (prev.Position.x + (prev.Size.x + s.GameScreenSize.x) / 2f)),
					y = 0f
				};
				s.GameScreen.Initialize(s);
				prev = s.GameScreen;
			});
			if (Application.isEditor)
			{
				CameraUtils.MainCamera.transform.position = gameScreens.First<GameScreenSetting>().GameScreen.Position.SetZ(-10f);
			}
			Action<IReadOnlyList<GameScreenController>> gameScreensCreated = GameScreensCreated;
			if (gameScreensCreated == null)
			{
				return;
			}
			gameScreensCreated((from s in gameScreens
			select s.GameScreen).ToList<GameScreenController>());
		}

	}
}
