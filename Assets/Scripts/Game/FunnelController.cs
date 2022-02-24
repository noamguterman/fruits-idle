using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game
{
	public class FunnelController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private BoxCollider leftTrigger;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private BoxCollider rightTrigger;

        private CollisionEvent left;

        private CollisionEvent right;

        private int leftCollisionsCount;

        private int rightCollisionsCount;

        public Reference<bool> IsLeftTouching { get; private set; }

		public Reference<bool> IsRightTouching { get; private set; }

		private void Awake()
		{
			IsLeftTouching = new Reference<bool>(false);
			IsRightTouching = new Reference<bool>(false);
			leftTrigger.isTrigger = true;
			rightTrigger.isTrigger = true;
			left = leftTrigger.gameObject.AddComponent<CollisionEvent>();
			right = rightTrigger.gameObject.AddComponent<CollisionEvent>();
		}

		private void OnEnable()
		{
			left.TriggerEnter += OnLeftTriggerEnter;
			right.TriggerEnter += OnRightTriggerEnter;
			left.TriggerExit += OnLeftTriggerExit;
			right.TriggerExit += OnRightTriggerExit;
		}

		private void OnDisable()
		{
			left.TriggerEnter -= OnLeftTriggerEnter;
			right.TriggerEnter -= OnRightTriggerEnter;
			left.TriggerExit -= OnLeftTriggerExit;
			right.TriggerExit -= OnRightTriggerExit;
		}

		private void OnRightTriggerEnter(Collider other)
		{
			rightCollisionsCount++;
			IsRightTouching.Value = (rightCollisionsCount > 0);
		}

		private void OnLeftTriggerEnter(Collider other)
		{
			leftCollisionsCount++;
			IsLeftTouching.Value = (leftCollisionsCount > 0);
		}

		private void OnRightTriggerExit(Collider other)
		{
			rightCollisionsCount--;
			IsRightTouching.Value = (rightCollisionsCount > 0);
		}

		private void OnLeftTriggerExit(Collider other)
		{
			leftCollisionsCount--;
			IsLeftTouching.Value = (leftCollisionsCount > 0);
		}

	}
}
