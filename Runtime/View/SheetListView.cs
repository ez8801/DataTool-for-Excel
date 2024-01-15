using UnityEngine.UIElements;

namespace EZ.DataTool.View
{
    public class SheetListView : CustomListView
    {
        public SheetListView()
            : base()
        {
            showFoldoutHeader = true;
            showBoundCollectionSize = false;
            showBorder = true;
            headerTitle = "Sheets";
        }

        public override void OnItemsSourceChanged()
        {
            base.OnItemsSourceChanged();
            headerTitle = $"Sheets ({itemsSource.Count})";
        }

        public override VisualElement MakeItem()
        {
            var view = new VisualElement();
            view.Add(new Label());
            return view;
        }

        public override void BindItem(VisualElement element, int index)
        {
            var sheetName = itemsSource[index] as string;
            element.Q<Label>().text = sheetName;
        }

        public override void UnbindItem(VisualElement element, int index)
        {
            
        }

        public override void DestroyItem(VisualElement element)
        {
            
        }
    }
}