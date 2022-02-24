using System;
using System.Collections.Generic;
using System.Linq;
using Game.Camera.CameraStates;
using Game.GameScreen;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;

namespace Game.Camera
{
	public class CameraController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private GameScreensManager gameScreensManager;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private TouchArea touchArea;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private CameraSettings cameraSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        private CameraParams cameraParams;

        private CameraState currentState;

        private bool isScrollEnabled;

        public GameScreenController CurrentGameScreen { get; private set; }

		public event Action<GameScreenController> ActiveScreenChange;

		public event Action StartMoveCamera;

		public bool IsMoving { get; private set; }

		private void OnEnable()
		{
			gameScreensManager.GameScreensCreated += OnGameScreensCreated;
			touchArea.MouseButtonUpClick += OnMouseButtonUpClick;
			touchArea.MouseButtonDownClick += OnMouseButtonDownClick;
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += OnSmeltersScreenUnlock;
		}

		private void OnDisable()
		{
			gameScreensManager.GameScreensCreated -= OnGameScreensCreated;
			touchArea.MouseButtonUpClick -= OnMouseButtonUpClick;
			touchArea.MouseButtonDownClick -= OnMouseButtonDownClick;
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased -= OnSmeltersScreenUnlock;
			StartMoveCamera = null;
			ActiveScreenChange = null;
		}

		private void OnGameScreensCreated(IReadOnlyList<GameScreenController> gameScreens)
		{
			isScrollEnabled = true;
			List<GameScreenController> list = (from g in gameScreens
			where g.IsReachable
			select g).ToList<GameScreenController>();
			GameScreenController gameScreenController = CurrentGameScreen = list.First<GameScreenController>();
			transform.position = gameScreenController.Position.SetZ(-10f);
			Vector2 camPosMinMax = new Vector2(list.First<GameScreenController>().Position.x, list.Last<GameScreenController>().Position.x);
			cameraParams = new CameraParams
			{
				Camera = transform,
				TouchArea = touchArea,
				ReachableGameScreens = list,
				CameraSpeed = new Reference<float>(0f),
				DirectionSign = new Reference<int>(0),
				CamPosMinMax = camPosMinMax,
				ActiveScreenChange = delegate(GameScreenController screen)
				{
					CurrentGameScreen = screen;
					IsMoving = false;
					Action<GameScreenController> activeScreenChange = ActiveScreenChange;
					if (activeScreenChange == null)
					{
						return;
					}
					activeScreenChange(screen);
				},
				StartMoveCamera = delegate()
				{
					IsMoving = true;
					Action startMoveCamera = this.StartMoveCamera;
					if (startMoveCamera == null)
					{
						return;
					}
					startMoveCamera();
				}
			};
			SetNewState(CameraStateEnum.None);
		}

		private void OnSmeltersScreenUnlock()
		{
			isScrollEnabled = true;
			SetNewState(CameraStateEnum.ForceToScreen);
		}

		private void SetNewState(CameraStateEnum cameraStateEnum)
		{
			CameraState cameraState = this.currentState;
			if (cameraState != null)
			{
				cameraState.Dispose();
			}
			switch (cameraStateEnum)
			{
			case CameraStateEnum.InputMove:
				currentState = new InputMoveState(cameraParams, cameraSettings);
				break;
			case CameraStateEnum.InertiaMove:
				currentState = new InertiaMoveState(cameraParams, cameraSettings);
				break;
			case CameraStateEnum.SnapToScreen:
				currentState = new SnapToScreenState(cameraParams, cameraSettings);
				break;
			case CameraStateEnum.None:
				currentState = null;
				break;
			case CameraStateEnum.ForceToScreen:
				currentState = new ForceToScreen(cameraParams, cameraSettings);
				break;
			default:
				throw new ArgumentOutOfRangeException("cameraStateEnum", cameraStateEnum, null);
			}
			if (currentState != null)
			{
				currentState.SetNewState += SetNewState;
			}
		}

		private void LateUpdate()
		{
			CameraState cameraState = currentState;
			if (cameraState == null)
			{
				return;
			}
			cameraState.Update();
		}

		private void OnMouseButtonDownClick(Vector3 mousePos)
		{
			if (isScrollEnabled)
			{
				SetNewState(CameraStateEnum.InputMove);
			}
		}

		private void OnMouseButtonUpClick(Vector3 mousePos)
		{
			if (isScrollEnabled)
			{
				SetNewState(CameraStateEnum.InertiaMove);
			}
		}

	}
}
