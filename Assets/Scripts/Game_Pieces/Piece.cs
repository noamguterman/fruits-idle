using System;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Pieces
{
	[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(Collider))]
	public class Piece : Costable, ISoundable
    {
        [SerializeField]
        private bool isBreakable;

        [SerializeField]
        [ShowIf("isBreakable", true)]
        private List<Piece> breakPieces;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PieceSettings pieceSettings;

        private MeshRenderer meshRenderer;

        private Collider pieceCollider;

        private float cost;

        public static readonly string BallToCrushTag = "Ball To Crush";

        public event Action<Piece, bool> CrossedPressureZone;

		public event Action<Piece> SmallestPieceTouchedConveyorPiece;

		public event Action<Piece> PieceDestroyed;

		private MeshRenderer MeshRenderer
		{
			get
			{
				if (!meshRenderer)
				{
					return meshRenderer = GetComponent<MeshRenderer>();
				}
				return meshRenderer;
			}
		}

		private Collider Collider
		{
			get
			{
				if (!pieceCollider)
				{
					return pieceCollider = GetComponent<Collider>();
				}
				return pieceCollider;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return Rigidbody.rotation;
			}
			set
			{
				Rigidbody.rotation = value;
			}
		}

		public Material Material
		{
			get
			{
				return MeshRenderer.sharedMaterial;
			}
			set
			{
				MeshRenderer.sharedMaterial = value;
			}
		}

		public float HealthMultiplier { get; set; }

		public Piece Parent { get; set; }

		public override float Cost
		{
			get
			{
				return cost;
			}
			set
			{
				cost = value;
				Health = value;
			}
		}

		public SoundType SoundType
		{
			get
			{
				return SoundType.OreHitConveyor;
			}
		}

		public override MoneyType MoneyType { get; set; }

		public float InitalCost
		{
			get
			{
				return InitialHealth / HealthMultiplier;
			}
		}

		public bool IsRare { get; set; }

		public int TypeID { get; set; }

		public float Health { get; set; }

		public float InitialHealth { get; set; }

		public List<Piece> BreakPieces
		{
			get
			{
				return breakPieces;
			}
		}

		public bool IsBreakable
		{
			get
			{
				return isBreakable && breakPieces.Count > 0;
			}
		}

		private static int DefaultPiecesLayer
		{
			get
			{
				return LayerMask.NameToLayer("Default Pieces");
			}
		}

		public static int SmallestPiecesLayer
		{
			get
			{
				return LayerMask.NameToLayer("Smallest Pieces");
			}
		}

		private static int PiecesOnConveyorLayer
		{
			get
			{
				return LayerMask.NameToLayer("Pieces On Conveyor");
			}
		}

		public bool ShouldCreateBreakPart(float cost, float health)
		{
			if (!gameObject.IsPrefab())
			{
				return false;
			}
			if (health <= 0f)
			{
				return !IsBreakable;
			}
			return cost * Parent.HealthMultiplier * pieceSettings.PieceBreakThreshold > health && IsBreakable;
		}

		public int GetLastPiecesAmount()
		{
			int num;
			if (!IsBreakable)
			{
				num = 1;
			}
			else
			{
				num = breakPieces.Count((Piece p) => !p.IsBreakable);
			}
			return num + (from p in breakPieces
			where p.IsBreakable
			select p).Sum((Piece p) => p.GetLastPiecesAmount());
		}

		public void GetChildren(ref List<Piece> childs)
		{
			GetChildren(this, ref childs);
		}

		private void GetChildren(Piece p, ref List<Piece> childs)
		{
			if (p.IsBreakable)
			{
				childs.AddRange(p.BreakPieces);
			}
			foreach (Piece piece in p.BreakPieces)
			{
				piece.Parent = p;
				GetChildren(piece, ref childs);
			}
		}

		private void OnEnable()
		{
			tag = Tag.Untagged;
			gameObject.layer = DefaultPiecesLayer;
			Collider.material = (IsBreakable ? pieceSettings.DefaultPieceMaterial : pieceSettings.SmallestPieceMaterial);
			Rigidbody.velocity = Vector3.zero;
			StartCoroutine(CoroutineUtils.LoopExecute(delegate()
			{
				if (Position.y < -30f)
				{
					Push(false);
				}
			}, new WaitForSeconds(1f)));
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			StopAllCoroutines();
			CrossedPressureZone = null;
			SmallestPieceTouchedConveyorPiece = null;
		}

		public override void Push(bool addMoney = false)
		{
			base.Push(addMoney);
			Action<Piece> pieceDestroyed = PieceDestroyed;
			if (pieceDestroyed == null)
			{
				return;
			}
			pieceDestroyed(this);
		}

		private void OnCollisionEnter(Collision other)
		{
			if (Rigidbody.collisionDetectionMode != CollisionDetectionMode.Discrete)
			{
				Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			if (gameObject.layer == SmallestPiecesLayer && other.gameObject.layer == PiecesOnConveyorLayer)
			{
				Action<Piece> smallestPieceTouchedConveyorPiece = SmallestPieceTouchedConveyorPiece;
				if (smallestPieceTouchedConveyorPiece == null)
				{
					return;
				}
				smallestPieceTouchedConveyorPiece(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Pressure Zone"))
			{
				Action<Piece, bool> crossedPressureZone = CrossedPressureZone;
				if (crossedPressureZone == null)
				{
					return;
				}
				crossedPressureZone(this, true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Pressure Zone"))
			{
				Action<Piece, bool> crossedPressureZone = CrossedPressureZone;
				if (crossedPressureZone == null)
				{
					return;
				}
				crossedPressureZone(this, false);
			}
		}

	}
}
