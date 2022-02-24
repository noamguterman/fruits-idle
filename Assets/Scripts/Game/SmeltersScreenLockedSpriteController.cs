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
	public class SmeltersScreenLockedSpriteController : MonoBehaviour
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

        private UpgradeListItem smeltersUpgrade;

        private LockedScreenMessage lockedMessage;
        private void Awake()
		{
			smeltersUpgrade = upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock);
		}

		private void OnEnable()
		{
			gameScreensManager.GameScreensCreated += OnGameScreensCreated;
			smeltersUpgrade.LevelIncreased += OnSmeltersScreenUnlock;
			GameData.LevelChanged += OnLevelChanged;
		}

		private void OnDisable()
		{
			gameScreensManager.GameScreensCreated -= OnGameScreensCreated;
			smeltersUpgrade.LevelIncreased -= OnSmeltersScreenUnlock;
			GameData.LevelChanged -= OnLevelChanged;
		}

		private void OnLevelChanged(int level)
		{
			LockedScreenMessage lockedScreenMessage = lockedMessage;
			if (lockedScreenMessage == null)
			{
				return;
			}
			lockedScreenMessage.SetMessageText(smeltersUpgrade.CurrentLockLevel > level);
		}

		private void OnGameScreensCreated(IReadOnlyList<GameScreenController> gameScreens)
		{
            Debug.Log("CurrentLevelUpgrade.Level = " + smeltersUpgrade.CurrentLevelUpgrade.Level + " GameData.CurrentLevel = " + GameData.CurrentLevel);
			if (smeltersUpgrade.CurrentLevelUpgrade.Level > 0)
			{
				return;
			}
            
			background = backgroundPrefab.PullOrCreate<SpriteRenderer>().transform;
			background.SetParent(base.transform);
            Debug.Log(background.name);
            Debug.Log(backgroundPrefab.name);

            Vector3 backgroundSize = Vector3.zero;
			List<GameScreenController> list = gameScreens.Reverse().Take(2).ToList();
			MonoBehaviour.print(string.Format("Count = {0}", list.Count));
            int i = 0;
			list.ForEach(delegate(GameScreenController s)
			{
                if(i < 2)
				    backgroundSize = new Vector3(backgroundSize.x + s.Size.x, s.Size.y, 1f);
                i++;
			});
			background.localScale = backgroundSize * 100;
            //background.position = (gameScreens[toUnlockLevel * 2].Position + Vector2.left * (backgroundSize.x - gameScreens[toUnlockLevel * 2].Size.x) / 2f).SetZ(-8f);
            background.position = gameScreens[2].Position.SetZ(-8f);
            lockedMessage = lockedMessagePrefab.PullOrCreate<LockedScreenMessage>();
			Transform transform = lockedMessage.transform;
			transform.SetParent(background, true);
			transform.position = gameScreens[2].Position.SetZ(0f);
			lockedMessage.SetMessageText(smeltersUpgrade.CurrentLockLevel > GameData.CurrentLevel);
		}

		private void OnSmeltersScreenUnlock()
		{
            if (smeltersUpgrade.CurrentLevelUpgrade.Level < 1)
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
