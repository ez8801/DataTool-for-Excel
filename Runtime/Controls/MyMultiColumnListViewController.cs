using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZ.DataTool
{
	public class MyMultiColumnListViewController : MultiColumnListViewController
	{
		private MultiColumnListView _mclv;
		private bool _editMode;
        private LabelPool _labelPool;

        public MyMultiColumnListViewController(Columns columns,
			SortColumnDescriptions sortDescriptions,
			List<SortColumnDescription> sortedColumns)
			: base(columns, sortDescriptions, sortedColumns)
		{
			_editMode = false;
			_labelPool = new LabelPool();

        }

		public void SetEditMode()
        {
			_editMode = true;
		}

		public void DisableEditMode()
        {
			_editMode = false;
        }

		protected override void PrepareView()
		{
			base.PrepareView();
			_mclv = view as MultiColumnListView;
		}

		protected override void BindItem(VisualElement element, int index)
		{
			base.BindItem(element, index);
			if (_editMode)
				return;

			element.name = index.ToString();

			var row = itemsSource[index];
			for (int i = 0; i < _mclv.columns.Count; i++)
			{
				var label = element[i] as Label;
				var rowData = row as System.Data.DataRow;

				try
				{
					label.text = rowData[i].ToString();
				} 
				catch (System.IndexOutOfRangeException e)
                {
					Debug.LogError(e.ToString());
					Debug.Log($"id: {index}, itemLen: {rowData.ItemArray?.Length ?? 0}");
                }
			}
			//Debug.Log($"BindItem({element.name})");
		}

		protected override VisualElement MakeItem()
		{
			VisualElement collectionViewItem = new();
			collectionViewItem.style.flexDirection = FlexDirection.Row;
			for (int i = 0; i < _mclv.columns.Count; i++)
			{
				//var view = new VisualElement() { name = i.ToString() };
				//view.AddToClassList("Item");

				//if (ColorUtility.TryParseHtmlString("232323", out var color))
				//	view.style.borderRightColor = color;
				//view.style.borderRightWidth = 1f;

				var label = _labelPool.Get();
                collectionViewItem.Add(label);
			}
			//Debug.Log("MakeItem");
			return collectionViewItem;
		}

		protected override void UnbindItem(VisualElement element, int index)
		{
			base.UnbindItem(element, index);
			//if (element is Label)
			//	Debug.Log($"UnbindItem(Label: {element.name}, {index})");
			//else
			//	Debug.Log($"UnbindItem({element.name}, {index})");
			//Release(element);
		}

		protected override void DestroyItem(VisualElement element)
		{
			base.DestroyItem(element);
			//if (element is Label)
			//	Debug.Log($"DestroyItem(Label: {element.name})");
			//else
			//	Debug.Log($"DestroyItem({element.name})");

			Release(element);
			//_mclv.Remove(element);
		}

		private void Release(VisualElement element)
		{
			foreach (var child in element.Children())
			{
				if (child is Label label)
				{
					if (label.style.display == DisplayStyle.Flex)
					{
						_labelPool.Release(label);
					}
				}
			}
		}
	}
}