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
	public class VenderScreenLockedSpriteController : MonoBehaviour
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

        private UpgradeListItem venderUpgrade;

        private LockedScreenMessage lockedMessage;

        private void Awake()
		{
            venderUpgrade = upgradeSettings.GetUpgradeListItem(UpgradeType.VenderUnlock);
		}

		private void OnEnable()
		{
			gameScreensManager.GameScreensCreated += OnGameScreensCreated;
            venderUpgrade.LevelIncreased += OnVenderScreenUnlock;
			GameData.LevelChanged += OnLevelChanged;
		}

		private void OnDisable()
		{
			gameScreensManager.GameScreensCreated -= OnGameScreensCreated;
            venderUpgrade.LevelIncreased -= OnVenderScreenUnlock;
			GameData.LevelChanged -= OnLevelChanged;
		}

		private void OnLevelChanged(int level)
		{
			LockedScreenMessage lockedScreenMessage = lockedMessage;
			if (lockedScreenMessage == null)
			{
				return;
			}
			lockedScreenMessage.SetMessageText(venderUpgrade.CurrentLockLevel > level);
		}

		private void OnGameScreensCreated(IReadOnlyList<GameScreenController> gameScreens)
		{
            Debug.Log("CurrentLevelUpgrade.Level = " + venderUpgrade.CurrentLevelUpgrade.Level + " GameData.CurrentLevel = " + GameData.CurrentLevel);
			if (venderUpgrade.CurrentLevelUpgrade.Level > 0)
			{
				return;
			}
            
			background = backgroundPrefab.PullOrCreate<SpriteRenderer>().transform;
			background.SetParent(base.transform);

            Vector3 backgroundSize = Vector3.zero;
			List<GameScreenController> list = gameScreens.Reverse().Reverse().Reverse().Take(2).ToList();
			MonoBehaviour.print(string.Format("Count = {0}", list.Count));
			list.ForEach(delegate(GameScreenController s)
			{
				backgroundSize = new Vector3(backgroundSize.x + s.Size.x, s.Size.y, 1f);
			});
			background.localScale = backgroundSize * 100;
            //background.position = (gameScreens[toUnlockLevel * 2].Position + Vector2.left * (backgroundSize.x - gameScreens[toUnlockLevel * 2].Size.x) / 2f).SetZ(-8f);
            background.position = gameScreens[4].Position.SetZ(-8f);
            lockedMessage = lockedMessagePrefab.PullOrCreate<LockedScreenMessage>();
			Transform transform = lockedMessage.transform;
			transform.SetParent(background, true);
			transform.position = gameScreens[4].Position.SetZ(0f);
			lockedMessage.SetMessageText(venderUpgrade.CurrentLockLevel > GameData.CurrentLevel);
		}

		private void OnVenderScreenUnlock()
		{
            if (venderUpgrade.CurrentLevelUpgrade.Level < 1)
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
