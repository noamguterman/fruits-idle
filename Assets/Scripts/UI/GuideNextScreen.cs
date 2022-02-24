using System;
using Game;
using Game.Camera;
using Game.GameScreen;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI
{
	public class GuideNextScreen : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private Image warningImage;

        [SerializeField]
        [Required]
        private CameraController gameCamera;

        [SerializeField]
        [Required]
        private GameScreenController activeGameScreen;

        [SerializeField]
        [Required]
        private GameScreenController activeGameScreen_1;

        [SerializeField]
        private float moveLen;

        [SerializeField]
        private float moveTime;

        [SerializeField]
        private float appearTime;

        [SerializeField]
        private AnimationCurve moveCurve;

        private Vector3 initialPos;

        private Transform imageTransform;

        private MultipleCoroutine moveRoutine;

        private bool isHidden;

        private void OnEnable()
		{
			gameCamera.StartMoveCamera += OnCameraStartMove;
			gameCamera.ActiveScreenChange += OnActiveGameScreenChange;
		}

		private void OnDisable()
		{
			gameCamera.StartMoveCamera -= OnCameraStartMove;
			gameCamera.ActiveScreenChange -= OnActiveGameScreenChange;
		}

		private void Start()
		{
			isHidden = true;
			imageTransform = warningImage.transform;
			initialPos = imageTransform.position;
			warningImage.AnimateAlpha(0f, null);
		}

		private void ChangeWarningState(bool isEnabled)
		{
			if (isEnabled)
			{
				if (isHidden)
				{
					imageTransform.position = initialPos;
					warningImage.AnimateAlpha(0f, null);
				}
				warningImage.AnimateAlpha(1f, appearTime, delegate(Image img)
				{
					isHidden = false;
				});
				MultipleCoroutine multipleCoroutine = new MultipleCoroutine(this);
				multipleCoroutine.Add(() => imageTransform.MovePositionIEnumerator(initialPos + Vector3.right * moveLen, moveTime, moveCurve));
				multipleCoroutine.Add(() => imageTransform.MovePositionIEnumerator(initialPos, moveTime, moveCurve));
				MultipleCoroutine multipleCoroutine2 = multipleCoroutine;
				moveRoutine = multipleCoroutine;
				multipleCoroutine2.StartCoroutines(true);
				return;
			}
			warningImage.AnimateAlpha(0f, appearTime, delegate(Image img)
			{
				MultipleCoroutine multipleCoroutine3 = moveRoutine;
				if (multipleCoroutine3 != null)
				{
					multipleCoroutine3.Dispose();
				}
				isHidden = true;
			});
		}

		private void OnActiveGameScreenChange(GameScreenController gameScreen)
		{
            if(GameData.CurrentLevel == 20 || GameData.CurrentLevel == 25)
            {
                ChangeWarningState(gameScreen == activeGameScreen);
            }
            else if(GameData.CurrentLevel == 35 || GameData.CurrentLevel == 40)
			    ChangeWarningState(gameScreen == activeGameScreen || gameScreen == activeGameScreen_1);
		}

		private void OnCameraStartMove()
		{
			ChangeWarningState(false);
		}

		public void Show(int level)
		{
            if(level == 20 || level == 25)
            {
                if (gameCamera.CurrentGameScreen == activeGameScreen)
                {
                    ChangeWarningState(true);
                }
            }
            else if (level == 35 || level == 40)
            {
                if (gameCamera.CurrentGameScreen == activeGameScreen || gameCamera.CurrentGameScreen == activeGameScreen_1)
                {
                    ChangeWarningState(true);
                }
            }
			
		}

		public void Hide()
		{
			ChangeWarningState(false);
		}

	}
}
