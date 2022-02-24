using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameScreen;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace Game
{
	public class BottleValueUnlockController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private GameScreensManager gameScreensManager;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [AssetsOnly]
        private SpriteRenderer backgroundPrefab;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [AssetsOnly]
        private LockedScreenMessage lockedMessagePrefab;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float dissapearTime;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float dissapearDelay;

        private Transform background;

        private UpgradeListItem bottleUpgrade;

        private LockedScreenMessage lockedMessage;
        private void Awake()
		{
            bottleUpgrade = upgradeSettings.GetUpgradeListItem(UpgradeType.BottleValue);
		}

		private void OnEnable()
		{
			GameData.LevelChanged += OnLevelChanged;
		}

		private void OnDisable()
		{
			GameData.LevelChanged -= OnLevelChanged;
		}

		private void OnLevelChanged(int level)
		{
			LockedScreenMessage lockedScreenMessage = lockedMessage;
			if (lockedScreenMessage == null)
			{
				return;
			}
			lockedScreenMessage.SetMessageText(bottleUpgrade.CurrentLockLevel > level);
		}

		private void OnSmeltersScreenUnlock()
		{
            if (bottleUpgrade.CurrentLevelUpgrade.Level < 1)
                return;

            Transform transform = background;
			if (transform == null)
			{
				return;
			}
			transform.AnimateAlpha(0f, dissapearTime, dissapearDelay, null);
		}

	}
}
