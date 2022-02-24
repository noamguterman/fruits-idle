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
	public class BufferFullWarning : MonoBehaviour, ISuspendable
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image warningImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private CameraController gameCamera;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private GameScreenController activeGameScreen;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float moveLen;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float moveTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float appearTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private AnimationCurve moveCurve;

        private Vector3 initialPos;

        private Transform imageTransform;

        private MultipleCoroutine moveRoutine;

        private bool isSuspended;

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
			ChangeWarningState(gameScreen == activeGameScreen && isSuspended);
		}

		private void OnCameraStartMove()
		{
			ChangeWarningState(false);
		}

		public void Suspend()
		{
			if (isSuspended)
			{
				return;
			}
			isSuspended = true;
			if (gameCamera.CurrentGameScreen == activeGameScreen && !gameCamera.IsMoving)
			{
				ChangeWarningState(true);
			}
		}

		public void Resume()
		{
			if (!isSuspended)
			{
				return;
			}
			isSuspended = false;
			ChangeWarningState(false);
		}

	}
}
