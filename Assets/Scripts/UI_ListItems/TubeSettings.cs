using System;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Tube Settings", fileName = "Tube Settings")]
	public class TubeSettings : ScriptableObject
	{
		public int BufferSize
		{
			get
			{
				return this.bufferSize;
			}
		}

		public float PiecesSpeedInTube
		{
			get
			{
				return this.piecesSpeedInTube;
			}
		}

		[SerializeField]
		private int bufferSize;

		[SerializeField]
		private float piecesSpeedInTube;
	}
}
