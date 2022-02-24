using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI
{
	public class LockedScreenMessage : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TMP_Text lockedMassage;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private string lockedMessageText;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private string availableMessageText;

        public void SetMessageText(bool locked)
		{
			lockedMassage.SetText(locked ? lockedMessageText : availableMessageText);
		}

	}
}
