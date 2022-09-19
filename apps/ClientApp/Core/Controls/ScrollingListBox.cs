using ClientApp.MVVM.ViewModel.Contacts.Chat;

using System.Collections;
using System.Collections.Specialized;

using System.Linq;
using System.Windows.Controls;

namespace ClientApp.Core.Controls {

    public class ScrollingListView : ListView {
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (newValue != null) {
                var enumerable = newValue.Cast<MessageInfo>();
                var count = enumerable.Count();

                if (count > 0) {
                    this.ScrollIntoView(enumerable.Last());
                }
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e) {
            base.OnItemsChanged(e);
            
            if (e.NewItems != null) {
                if (e.NewItems.Count > 0) {
                    this.ScrollIntoView(e.NewItems[e.NewItems.Count - 1]);
                }
            }
        }
    }

}