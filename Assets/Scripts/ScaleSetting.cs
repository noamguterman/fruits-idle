using System;
using UnityEngine;

namespace Game.Press
{
	[Serializable]
	public struct ScaleSetting
    {
        [SerializeField]
        private Vector2 pos;

        [SerializeField]
        private float xScale;

        public Vector2 Pos
		{
			get
			{
				return pos;
			}
		}

		public float XScale
		{
			get
			{
				return xScale;
			}
		}

	}
}
