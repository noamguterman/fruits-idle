using System;
using System.Collections.Generic;
using System.Linq;
using Game.Pieces;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UI.ListItems.Booster;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;

namespace Game.Smelters
{
	public class SmeltersController : MonoBehaviour
    {
        public static SmeltersController Instance;
        //[FoldoutGroup("References", 0)]
        public List<Smelter> smelters;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private PiecesController pController;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Boostmeter boostmeter;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        private List<PieceListItem> activeTypes;

        private void Awake()
		{
            Instance = this;
            activeTypes = new List<PieceListItem>();
		}

		private void OnEnable()
		{
			pController.ActivePieceTypesChange += ActivePieceTypesChange;
			boostmeter.MultiplierChange += OnMultiplierChange;
			smelters.ForEach(delegate(Smelter s)
			{
				s.PieceEnter += OnPieceEnter;
			});
			boostersSettings.ChangeBoosterTypeMutiplier += OnBoosterMutiplierChanged;
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += OnSmeltersScreenUnlock;
		}

		private void OnDisable()
		{
			pController.ActivePieceTypesChange -= ActivePieceTypesChange;
			boostmeter.MultiplierChange -= OnMultiplierChange;
			smelters.ForEach(delegate(Smelter s)
			{
				s.PieceEnter -= OnPieceEnter;
			});
			boostersSettings.ChangeBoosterTypeMutiplier -= OnBoosterMutiplierChanged;
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased -= OnSmeltersScreenUnlock;
		}

		private void OnBoosterMutiplierChanged(BoosterType boosterType, float multiplier, float time, bool isEnabled)
		{
			if (boosterType == BoosterType.MultiplierIncome)
			{
				return;
			}
			smelters.ForEach(delegate(Smelter s)
			{
				s.SpeedMultiplier = multiplier;
			});
		}

		private void OnSmeltersScreenUnlock()
		{
			ActivePieceTypesChange((from t in activeTypes
			where pController.CountPieces(t.TypeID) > 0 || pController.ActivePieceTypes.Contains(t)
			select t).ToList<PieceListItem>());
		}

		private void ActivePieceTypesChange(IReadOnlyList<PieceListItem> activeTypes)
		{
			if (upgradeSettings.GetCurrentUpgrade(UpgradeType.SmeltersUnlock).Level == 0)
			{
				foreach (PieceListItem item in activeTypes)
				{
					if (!this.activeTypes.Contains(item))
					{
                        this.activeTypes.Add(item);
					}
				}
				return;
			}
			List<Smelter> list = new List<Smelter>(smelters);
			using (IEnumerator<PieceListItem> enumerator = activeTypes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PieceListItem type = enumerator.Current;
					using (List<Smelter>.Enumerator enumerator2 = new List<Smelter>(list).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Smelter smelter = enumerator2.Current;
							IEnumerable<Smelter> source = smelters;
							Func<Smelter, bool> predicate;
							predicate = new Func<Smelter, bool>((Smelter n) => n.HasTypeID(type.TypeID));

                            if (!source.Any(predicate) && !activeTypes.Any((PieceListItem t) => smelter.HasTypeID(t.TypeID)))
							{
								smelter.AddAllowedPieceTypeID(type.TypeID);
								list.Remove(smelter);
								break;
							}
						}
					}
				}
			}
		}

		private void OnMultiplierChange(float multiplier)
		{
			smelters.ForEach(delegate(Smelter s)
			{
				s.MeltTimeMultiplier = Mathf.Pow(multiplier, 5);
			});
		}

		private void OnPieceEnter(Smelter smelter, int pieceTypeID)
		{
			if (pController.CountPieces(pieceTypeID) == 0)
			{
				smelter.ForceMelting();
			}
		}

	}
}
