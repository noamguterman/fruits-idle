using System;
using Game.GameScreen;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;

namespace Game
{
	public class BordersController : MonoBehaviour, IGameScreenComponent
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RectTransform moneyBackground;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float colliderDefaultWidth;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float bordersOffScreenHeight;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool isRightBorderPassableByPiecesOnConveyor;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool isLeftBorderPassableByPiecesOnConveyor;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private PhysicMaterial bordersPhysicMaterial;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        private BoxCollider rightBorder;

        private BoxCollider leftBorder;

        public Bounds GameScreenBounds { private get; set; }

		private void OnEnable()
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += OnSmeltersScreenUnlock;
		}

		private void OnDisable()
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased -= OnSmeltersScreenUnlock;
		}

		private void Start()
		{
			GenerateBorders();
		}

		private void OnSmeltersScreenUnlock()
		{
			rightBorder.gameObject.layer = LayerMask.NameToLayer("Collision With All Pieces");
		}

		private void GenerateBorders()
		{
			leftBorder = CreateColliderGameObject();
			leftBorder.transform.position = new Vector3(GameScreenBounds.min.x - colliderDefaultWidth / 2f, GameScreenBounds.center.y + bordersOffScreenHeight / 2f);
			leftBorder.size = new Vector3(colliderDefaultWidth, GameScreenBounds.size.y + bordersOffScreenHeight, 2f);
			leftBorder.gameObject.layer = LayerMask.NameToLayer(isLeftBorderPassableByPiecesOnConveyor ? "Collision With All Pieces" : "Border");
			rightBorder = CreateColliderGameObject();
			rightBorder.transform.position = new Vector3(GameScreenBounds.max.x + colliderDefaultWidth / 2f, GameScreenBounds.center.y + bordersOffScreenHeight / 2f);
			rightBorder.size = new Vector3(colliderDefaultWidth, GameScreenBounds.size.y + bordersOffScreenHeight, 2f);
			rightBorder.gameObject.layer = LayerMask.NameToLayer((isRightBorderPassableByPiecesOnConveyor || upgradeSettings.GetCurrentUpgrade(UpgradeType.SmeltersUnlock).Level == 0) ? "Collision With All Pieces" : "Border");
		}

		private BoxCollider CreateColliderGameObject()
		{
			BoxCollider boxCollider = new GameObject("Border").AddComponent<BoxCollider>();
			boxCollider.material = bordersPhysicMaterial;
			boxCollider.transform.SetParent(transform);
			return boxCollider;
		}

	}
}
