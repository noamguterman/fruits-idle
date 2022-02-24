using System;
using System.Collections.Generic;
using System.Linq;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems.Upgrades.Piece;
using UI.ListItems.Upgrades.Piece.Press;
using UnityEngine;

namespace Game.Press
{
	public class PressScaler : MonoBehaviour
	{
        [SerializeField]
        private List<ScalablePressPart> pressParts;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PressVariationsSettings pressVariationsSettings;

        private UpgradeListItem listItem;

        private void Start()
		{
			SetScale();
		}

		private void OnEnable()
		{
            (listItem = pressVariationsSettings.CurrentPressVariation.PressUpgrades.First((PressTypeListItem u) => u.UpgradeType == PressUpgradeType.PressWidth)).LevelIncreased += OnUpgradeLevelChanged;
		}

		private void OnDisable()
		{
			listItem.LevelIncreased -= OnUpgradeLevelChanged;
		}

		private void OnUpgradeLevelChanged()
		{
			SetScale();
		}

		private void SetScale()
		{
			Upgrade currentUpgrade = pressVariationsSettings.CurrentPressVariation.GetCurrentUpgrade(PressUpgradeType.PressWidth, false);
			if (currentUpgrade == null)
			{
				return;
			}
			float xScale = currentUpgrade.Value;
            Debug.Log(currentUpgrade.Level + "  " + currentUpgrade.PriceOfLevelUp + "  " + currentUpgrade.Value + "  " +xScale);
            foreach(ScalablePressPart p in pressParts)
			{
                Debug.Log(xScale + "  " + p.Min.XScale + "  " + p.Max.XScale);

				float num = Mathf.Clamp(xScale, p.Min.XScale, p.Max.XScale);
				p.Press.transform.localScale = new Vector3(num, p.Press.transform.localScale.y, p.Press.transform.localScale.z);
                Debug.Log("~~" + p.Press.transform.localScale);
				if (p.NeedToChangePos)
				{
					Vector2 vector = Vector2.Lerp(p.Min.Pos, p.Max.Pos, num / p.Max.XScale);
					p.Press.transform.localPosition = new Vector3(vector.x, vector.y, p.Press.transform.localPosition.z);
				}
			};
		}

	}
}
