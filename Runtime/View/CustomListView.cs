using UnityEngine.UIElements;
using System.Collections;

namespace EZ.DataTool.View
{
    public abstract class CustomListView : ListView
    {
        public CustomListView()
        {
            makeItem = MakeItem;
            bindItem = BindItem;
            unbindItem = UnbindItem;
            itemsSourceChanged += OnItemsSourceChanged;
        }

        public void SetItemsSource(IList itemsSource) => this.itemsSource = itemsSource;

        public virtual void OnItemsSourceChanged()
        {

        }

        public abstract VisualElement MakeItem();

        public abstract void BindItem(VisualElement element, int index);

        public virtual void UnbindItem(VisualElement element, int index)
        {

        }

        public virtual void DestroyItem(VisualElement element)
        {

        }
    }
}