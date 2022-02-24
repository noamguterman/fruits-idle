using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Ball Settings")]
	public class PieceSettings : ScriptableObject
    {
        [TitleGroup("Gold Piece", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        private Vector2 onPieceCreateForce;

        [SerializeField]
        [Range(0f, 1f)]
        private float piecesSpread;

        [TitleGroup("Piece Spawning", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        private int maxPieceTypesSpawn;

        [SerializeField]
        private int maxPiecesAmount;

        [SerializeField]
        private float spawnPiecesSpacing;

        [TitleGroup("Piece Destruction", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        [Range(0f, 1f)]
        private float pieceBreakThreshold;

        [TitleGroup("Piece Physics Materials", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        [Required]
        private PhysicMaterial defaultPieceMaterial;

        [SerializeField]
        [Required]
        private PhysicMaterial smallestPieceMaterial;

        public Vector2 OnPieceCreateForce
		{
			get
			{
				return onPieceCreateForce;
			}
		}

		public float PiecesSpread
		{
			get
			{
				return piecesSpread;
			}
		}

		public PhysicMaterial DefaultPieceMaterial
		{
			get
			{
				return defaultPieceMaterial;
			}
		}

		public PhysicMaterial SmallestPieceMaterial
		{
			get
			{
				return smallestPieceMaterial;
			}
		}

		public int MaxPieceTypesSpawn
		{
			get
			{
				return maxPieceTypesSpawn;
			}
		}

		public int MaxPiecesAmount
		{
			get
			{
				return maxPiecesAmount;
			}
		}

		public float SpawnPiecesSpacing
		{
			get
			{
				return spawnPiecesSpacing;
			}
		}

		public float PieceBreakThreshold
		{
			get
			{
				return pieceBreakThreshold;
			}
		}

	}
}
