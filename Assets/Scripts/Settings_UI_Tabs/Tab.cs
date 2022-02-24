using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;

namespace Settings.UI.Tabs
{
	public abstract class Tab : ScriptableObject, IEnumerable<ListItem>, IEnumerable
    {
        [SerializeField]
        private bool isEnabled;

        [SerializeField]
        private int tabID;

        [SerializeField]
        [Required]
        private Sprite tabSprite;

        [SerializeField]
        [Required]
        private Sprite selectedTabSprite;

        public int TabID
		{
			get
			{
				return tabID;
			}
		}

		public Sprite TabSprite
		{
			get
			{
				return tabSprite;
			}
		}

		public Sprite SelectedTabSprite
		{
			get
			{
				return selectedTabSprite;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
		}

		public abstract IReadOnlyList<ListItem> GetTabItems();

		public IEnumerator<ListItem> GetEnumerator()
		{
			return GetTabItems().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}
}
