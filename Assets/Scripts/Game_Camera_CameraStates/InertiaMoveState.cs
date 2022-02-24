using System;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Camera.CameraStates
{
	public class InertiaMoveState : CameraState
	{
		public InertiaMoveState(CameraParams cParams, CameraSettings cameraSettings) : base(cParams, cameraSettings)
		{
		}

		public override void Update()
		{
			this.cParams.CameraSpeed.Value = Mathf.Max(0f, this.cParams.CameraSpeed.Value - Time.deltaTime * this.cameraSettings.SpeedDecreaseRate);
			if (this.cParams.CameraSpeed.Value < this.cameraSettings.MinSpeedSnapThreshold)
			{
				base.NewState(CameraStateEnum.SnapToScreen);
				return;
			}
			this.camera.SetPositionX(Mathf.Clamp(this.camera.position.x + this.cParams.CameraSpeed.Value * this.cameraSettings.Sensitivity * (float)this.cParams.DirectionSign.Value, this.cParams.CamPosMinMax.x, this.cParams.CamPosMinMax.y));
		}
	}
}
