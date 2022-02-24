using System;

namespace Game.Camera.CameraStates
{
	public enum CameraStateEnum
	{
		InputMove,
		InertiaMove,
		SnapToScreen,
		None,
		ForceToScreen
	}
}
