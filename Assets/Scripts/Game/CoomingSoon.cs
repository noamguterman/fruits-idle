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
    public class CoomingSoon : MonoBehaviour
    {
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private GameScreensManager gameScreensManager;

        [SerializeField]
        [Required]
        [AssetsOnly]
        private SpriteRenderer backgroundPrefab;

        [SerializeField]
        [Required]
        [AssetsOnly]
        private LockedScreenMessage lockedMessagePrefab;

        private Transform background;

        private LockedScreenMessage lockedMessage;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            gameScreensManager.GameScreensCreated += OnGameScreensCreated;
        }

        private void OnDisable()
        {
            gameScreensManager.GameScreensCreated -= OnGameScreensCreated;
        }

        private void OnGameScreensCreated(IReadOnlyList<GameScreenController> gameScreens)
        {
            background = backgroundPrefab.PullOrCreate<SpriteRenderer>().transform;
            background.SetParent(base.transform);

            Vector3 backgroundSize = Vector3.zero;
            List<GameScreenController> list = gameScreens.Reverse().Reverse().Reverse().Reverse().Reverse().Take(2).ToList();
            //print(string.Format("Count = {0}", list.Count));
            list.ForEach(delegate (GameScreenController s)
            {
                backgroundSize = new Vector3(backgroundSize.x + s.Size.x, s.Size.y, 1f);
            });
            background.localScale = backgroundSize * 100;
            //background.position = (gameScreens[toUnlockLevel * 2].Position + Vector2.left * (backgroundSize.x - gameScreens[toUnlockLevel * 2].Size.x) / 2f).SetZ(-8f);
            background.position = gameScreens[6].Position.SetZ(-8f);
            lockedMessage = lockedMessagePrefab.PullOrCreate<LockedScreenMessage>();
            Transform transform = lockedMessage.transform;
            transform.SetParent(background, true);
            transform.position = gameScreens[6].Position.SetZ(0f);
        }
    }
}
