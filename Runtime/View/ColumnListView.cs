using UnityEngine.UIElements;

namespace EZ.DataTool.View
{
    public class ColumnListView : CustomListView
    {
        public ColumnListView()
            : base()
        {
            showFoldoutHeader = true;
            showBoundCollectionSize = false;
            showBorder = true;
            headerTitle = "Columns";
        }

        public override void OnItemsSourceChanged()
        {
            base.OnItemsSourceChanged();
            headerTitle = $"Columns ({itemsSource.Count})";
        }

        public override void BindItem(VisualElement element, int index)
        {
            var item = (DbfColumn)itemsSource[index];
            var label = element as Label;
            if (item.Key == KeyType.PrimaryKey)
                label.text = $"{item.ValueTypeName} {item.Name} (PrimaryKey)";
            else
                label.text = $"{item.ValueTypeName} {item.Name}";
        }

        public override VisualElement MakeItem()
        {
            return new Label();
        }
    }
}