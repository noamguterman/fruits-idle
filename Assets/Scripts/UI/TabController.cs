using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
	public class TabController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image image;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Button button;

        public Sprite tabSprite;

        public Sprite selectedTabSprite;

        public event Action ButtonClick;

		public int Index { get; private set; }

		private void OnDisable()
		{
			button.onClick.RemoveAllListeners();
		}

		public void Initialize(Sprite tabSprite, Sprite selectedTabSprite, int index)
		{
			this.tabSprite = tabSprite;
			this.selectedTabSprite = selectedTabSprite;
			image.sprite = tabSprite;
			Index = index;

            button.onClick.AddListener(delegate ()
            {
                InvokeButtonClick();
            });

        }

        protected void InvokeButtonClick()
        {
            Action buttonClick = ButtonClick;
            if (buttonClick == null)
            {
                return;
            }
            buttonClick();
        }

        public void SetSelectState(bool selected)
		{
			image.sprite = (selected ? selectedTabSprite : tabSprite);
		}
	}
}
