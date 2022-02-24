using System;
using System.Linq;
using Game.GameScreen;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Camera.CameraStates
{
	public class SnapToScreenState : CameraState
	{
        //public SnapToScreenState(CameraParams cParams, CameraSettings cameraSettings)
        //{
        //	SnapToScreenState._003C_003Ec__DisplayClass1_0 CS_0024_003C_003E8__locals1 = new SnapToScreenState._003C_003Ec__DisplayClass1_0();
        //	CS_0024_003C_003E8__locals1.cParams = cParams;
        //	base..ctor(CS_0024_003C_003E8__locals1.cParams, cameraSettings);
        //	CS_0024_003C_003E8__locals1._003C_003E4__this = this;
        //	CS_0024_003C_003E8__locals1.cParams.CameraSpeed.Value = 0f;
        //	GameScreenController gameScreen = (from g in CS_0024_003C_003E8__locals1.cParams.ReachableGameScreens
        //	orderby (CS_0024_003C_003E8__locals1._003C_003E4__this.camera.position.SetZ(0f) - g.Position).sqrMagnitude
        //	select g).First<GameScreenController>();
        //	this.snapScreenRoutine = this.camera.MovePositionX(gameScreen.Position.x, cameraSettings.SnapToScreenTime, cameraSettings.SnapToScreenCurve, delegate()
        //	{
        //		CS_0024_003C_003E8__locals1.cParams.ActiveScreenChange(gameScreen);
        //		CS_0024_003C_003E8__locals1._003C_003E4__this.NewState(CameraStateEnum.None);
        //	});
        //}

        //public override void Update()
        //{
        //}

        //public override void Dispose()
        //{
        //	base.Dispose();
        //	if (this.snapScreenRoutine != null)
        //	{
        //		this.camera.StopCoroutine(this.snapScreenRoutine);
        //	}
        //}

        //private readonly Coroutine snapScreenRoutine;
        private readonly Coroutine snapScreenRoutine;

        public SnapToScreenState(CameraParams cParams, CameraSettings cameraSettings) : base(cParams, cameraSettings)
        {
            cParams.CameraSpeed.Value = 0f;
            Vector3 position = camera.position;
            snapScreenRoutine = camera.MovePosition((from g in cParams.ReachableGameScreens
                orderby (new Vector2(camera.position.x, camera.position.y) - g.Position).sqrMagnitude
                select g).First().Position.SetY(position.y).SetZ(position.z), cameraSettings.SnapToScreenTime, cameraSettings.SnapToScreenCurve, delegate ()
                {
                    NewState(CameraStateEnum.None);
                });
        }

        public override void Update()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            if (snapScreenRoutine != null)
            {
                camera.StopCoroutine(snapScreenRoutine);
            }
        }
    }
}
