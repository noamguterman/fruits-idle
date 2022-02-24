using System;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game
{
	public class Bottle : Costable, ISoundable
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private MeshRenderer meshRenderer;

        [SerializeField]
        [Required]
        private Material contentMaterial;

        public override float Cost { get; set; }

		public override MoneyType MoneyType
		{
			get
			{
				return MoneyType.Money;
			}
		}

		public SoundType SoundType
		{
			get
			{
				return SoundType.BarHitConveyour;
			}
		}

		private Transform Transform { get; set; }

		public Quaternion Rotation
		{
			get
			{
				return Transform.rotation;
			}
			set
			{
				Transform.rotation = value;
			}
		}

		private void Awake()
		{
			Transform = transform;
		}

		public void Initialize(float cost, Color contentCol)
		{
			Cost = cost;
            contentMaterial.color = contentCol;
            //meshRenderer.material = material;
            Rigidbody.velocity = Vector3.zero;
			Rigidbody.angularVelocity = Vector3.zero;
		}

		public override void Push(bool addMoney = false)
		{
			base.Push(addMoney);
			Position = Vector3.one * 100f;
			//base.StartCoroutine(CoroutineUtils.ExecuteAfter(new Action(base.Push<Bar>), new WaitForFixedUpdate()));
		}

	}
}
