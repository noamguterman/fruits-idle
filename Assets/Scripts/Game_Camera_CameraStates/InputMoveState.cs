using System;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Camera.CameraStates
{
	public class InputMoveState : CameraState
	{
        private bool isStartMoveCameraInvoked;

        public InputMoveState(CameraParams cParams, CameraSettings cameraSettings) : base(cParams, cameraSettings)
		{
			cParams.TouchArea.Drag += OnDrag;
		}

		public override void Update()
		{
			camera.SetPositionX(Mathf.Clamp(camera.position.x + cParams.CameraSpeed.Value * cameraSettings.Sensitivity * (float)cParams.DirectionSign.Value, cParams.CamPosMinMax.x, cParams.CamPosMinMax.y));
		}

		private void OnDrag(Vector2 direction)
		{
			if (!isStartMoveCameraInvoked && direction.magnitude > 0.05f)
			{
				cParams.StartMoveCamera();
				isStartMoveCameraInvoked = true;
			}
			cParams.CameraSpeed.Value = Mathf.Abs(direction.x);
			cParams.DirectionSign.Value = ((direction.x < 0f) ? 1 : -1);
		}

		public override void Dispose()
		{
			base.Dispose();
			cParams.TouchArea.Drag -= OnDrag;
		}

	}
}
