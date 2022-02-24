using System;
using Game.Pieces;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Game.Press
{
	[Serializable]
	public class PressPart
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [Required]
        private Transform part;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [Required]
        private Transform moveTarget;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private MoveDirection moveDirection;

        private Vector2 direction;

        private BoxCollider partCollider;

        private BoxCollider targetCollider;

        private CollisionEvent collisionEvent;

        public event Action<PressPart> PressPartCollidedWithOre;

		public bool IsMovable { get; private set; }

		[UsedImplicitly]
		private string Name
		{
			get
			{
				if (!(part == null))
				{
					return part.name;
				}
				return string.Empty;
			}
		}

		public Vector3 Position
		{
			get
			{
				return new Vector3(part.position.x, part.position.y, InitialPosition.z);
			}
			set
			{
				part.position = new Vector3(value.x, value.y, InitialPosition.z);
			}
		}

		public Vector3 TargetPosition
		{
			get
			{
				Vector3 b = (targetCollider == null) ? Vector3.zero : new Vector3(targetCollider.bounds.extents.x, targetCollider.bounds.extents.y, InitialPosition.z);
				Vector3 b2 = (partCollider == null) ? Vector3.zero :  new Vector3(partCollider.bounds.extents.x, partCollider.bounds.extents.y, InitialPosition.z);
				Vector3 b3 = (partCollider == null) ? Vector3.zero : (partCollider.bounds.center - partCollider.transform.position);
				Vector3 b4 = (targetCollider == null) ? Vector3.zero : (targetCollider.bounds.center - targetCollider.transform.position);
				b.Scale(-direction);
				b2.Scale(-direction);
				return moveTarget.position + b + b2 + b3 + b4;
			}
		}

		public Vector3 InitialPosition { get; private set; }

		public void Initialize()
		{
			InitialPosition = part.position;
			partCollider = part.GetComponentInChildren<BoxCollider>();
			targetCollider = moveTarget.GetComponentInChildren<BoxCollider>();
			IsMovable = true;
			switch (moveDirection)
			{
			case MoveDirection.Left:
				direction = Vector3.left;
				break;
			case MoveDirection.Up:
				direction = Vector3.up;
				break;
			case MoveDirection.Right:
				direction = Vector3.right;
				break;
			case MoveDirection.Down:
				direction = Vector3.down;
				break;
			case MoveDirection.None:
				IsMovable = false;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			part.GetComponentInChildren<CollisionEvent>().CollisionEnter += delegate(Collision other)
			{
				if (other.gameObject.CompareTag(Piece.BallToCrushTag))
				{
					Action<PressPart> pressPartCollidedWithOre = PressPartCollidedWithOre;
					if (pressPartCollidedWithOre == null)
					{
						return;
					}
					pressPartCollidedWithOre(this);
				}
			};
		}

		public float GetT(Vector2 pos)
		{
			if (moveDirection != MoveDirection.Up && moveDirection != MoveDirection.Down)
			{
				return (pos.x - InitialPosition.x) / (TargetPosition.x - InitialPosition.x);
			}
			return (pos.y - InitialPosition.y) / (TargetPosition.y - InitialPosition.y);
		}

	}
}
