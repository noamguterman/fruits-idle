using System;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class TouchArea : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
    {
        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        private bool wasDrag;

        private bool isPointerDown;

        public event Action<Vector3> MouseButtonClick;

		public event Action<Vector3> MouseButtonUpClick;

		public event Action<Vector3> MouseButtonDownClick;

		public event Action<Vector3> MouseButtonClickScreen;

		public event Action<Vector3> MouseButtonUpClickScreen;

		public event Action<Vector3> MouseButtonDownClickScreen;

		public event Action<Vector2> Drag;

		private Vector2 GetInputPos()
		{
			return Input.mousePosition / (float)Screen.width;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
            PlayerPrefs.SetInt("isTapped", 1);

                AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.ScreenTap);
			if (audioSource != null)
			{
				audioSource.Play(false);
			}
			if(MouseButtonClickScreen != null)
                MouseButtonClickScreen(Input.mousePosition);

			if (MouseButtonClick != null)
                MouseButtonClick(GetInputPos());

        }

        public void OnPointerDown(PointerEventData eventData)
		{
			isPointerDown = true;
			
            if(MouseButtonDownClickScreen != null)
                MouseButtonDownClickScreen(Input.mousePosition);
            if (MouseButtonDownClick != null)
                MouseButtonDownClick(GetInputPos());
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isPointerDown = false;
			if(MouseButtonUpClickScreen != null)
                MouseButtonUpClickScreen(Input.mousePosition);
			
			if (MouseButtonUpClick != null)
                MouseButtonUpClick(GetInputPos());

        }

        public void SimulateClick()
		{
			AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.ScreenTap);
			if (audioSource != null)
			{
				audioSource.Play(false);
			}
			
            if(MouseButtonClickScreen != null)
                MouseButtonClickScreen(Input.mousePosition);
			
			if (MouseButtonClick != null)
                MouseButtonClick(GetInputPos());
		}

		public void OnDrag(PointerEventData eventData)
		{
			wasDrag = true;
            if(Drag != null)
			    Drag(eventData.delta);
		}

		private void LateUpdate()
		{
			if (isPointerDown)
			{
				if (!wasDrag)
				{
					Drag(Vector2.zero);
				}
				wasDrag = false;
			}
		}

	}
}
