using System;
using System.Collections.Generic;
using System.Linq;
using UI.ListItems;
using UnityEngine;

namespace UI.BottomList
{
	[Serializable]
	public class TabContainer
    {
        private List<ListItem> listItems;

        private readonly Sprite tab;

        private readonly Sprite selectedTab;

        public List<ListItem> ListItems
		{
			get
			{
				return listItems;
			}
		}

		public Sprite Tab
		{
			get
			{
				return tab;
			}
		}

		public Sprite SelectedTab
		{
			get
			{
				return selectedTab;
			}
		}

		public TabContainer(List<ListItem> listItems, Sprite tab, Sprite selectedTab)
		{
			this.listItems = new List<ListItem>(listItems);
			this.tab = tab;
			this.selectedTab = selectedTab;
		}

		public void AddRange(IEnumerable<ListItem> add)
		{
			listItems.AddRange(add);
		}

		public void Order()
		{
			listItems = (from l in listItems
			orderby l.Order
			select l).ToList<ListItem>();
		}

	}
}
