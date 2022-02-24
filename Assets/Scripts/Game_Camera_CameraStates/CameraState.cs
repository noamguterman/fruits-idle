using System;
using UI.ListItems;
using UnityEngine;

namespace Game.Camera.CameraStates
{
	public abstract class CameraState : IDisposable
    {
        protected readonly CameraParams cParams;

        protected readonly CameraSettings cameraSettings;

        protected readonly Transform camera;

        public event Action<CameraStateEnum> SetNewState;

		protected CameraState(CameraParams cParams, CameraSettings cameraSettings)
		{
			this.cParams = cParams;
			this.cameraSettings = cameraSettings;
			camera = cParams.Camera;
		}

		protected void NewState(CameraStateEnum newStateEnum)
		{
			Action<CameraStateEnum> setNewState = SetNewState;
			if (setNewState == null)
			{
				return;
			}
			setNewState(newStateEnum);
		}

		public abstract void Update();

		public virtual void Dispose()
		{
			SetNewState = null;
		}

		protected void print(object msg)
		{
			Debug.Log(msg);
		}

	}
}
