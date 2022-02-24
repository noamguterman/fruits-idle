using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Press
{
	[Serializable]
	public struct ScalablePressPart
    {
        [SerializeField]
        [Required]
        private SpriteRenderer press;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool needToChangePos;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private ScaleSetting min;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private ScaleSetting max;

        public SpriteRenderer Press
		{
			get
			{
				return press;
			}
		}

		public bool NeedToChangePos
		{
			get
			{
				return needToChangePos;
			}
		}

		public ScaleSetting Min
		{
			get
			{
				return min;
			}
		}

		public ScaleSetting Max
		{
			get
			{
				return max;
			}
		}

	}
}
