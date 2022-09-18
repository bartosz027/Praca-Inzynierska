using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientApp.Resources.CustomControls
{
    public class ScrollingListView : ListView
    {
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                int newItemCount = e.NewItems.Count;

                if (newItemCount > 0)
                    this.ScrollIntoView(e.NewItems[newItemCount - 1]);

                base.OnItemsChanged(e);
            }
        }
    }

    public class ScrollingListBox : ListBox
    {
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            MessageBox.Show("Huj hehe XD");
            base.OnSelectionChanged(e);
        }
    }
}
