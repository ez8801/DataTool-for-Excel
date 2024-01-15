using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace EZ.DataTool.View
{
    public class RootView : VisualElement, IProgressBar
	{
		public static readonly string ussClassName = "ez-root-view";

		public interface IListener
        {
			void OnClickExportButton();
			void OnClickExploreButton();
			void OnClickOpenButton();
			void OnKeywordChanged(ChangeEvent<string> evt);
			void OnSelectedIndicesChange(int index);
		}

		private IListener _listener;
		private VisualElement _rootVisualElement;

		public VisualElement RootVisualElement => _rootVisualElement;
		public MultiColumnListView MultiColumnListView { get; private set; }
		public MyMultiColumnListViewController MultiColumnListViewControl { get; private set; }
		public ColumnListView ColumListView { get; private set; }
		public SheetListView SheetListView { get; private set; }
		public ProgressBar ProgressBar { get; private set; }
		public bool RuntimeOnly { get; private set; }

		public RootView(VisualElement rootVisualElement, bool runtimeOnly)
        {
			AddToClassList(ussClassName);

			var splitView = new TwoPaneSplitView(0, 300, TwoPaneSplitViewOrientation.Horizontal)
			{
				name = "contentView"
			};
			var splitView2 = new TwoPaneSplitView(1, 300, TwoPaneSplitViewOrientation.Horizontal);

			var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal)
			{
				name = "contentPane"
			};
			scrollView.style.height = new StyleLength(Length.Percent(100));
			scrollView.Add(GenerateContentView());
			splitView2.Add(scrollView);

			var fixedPane = CreateInspectorPane();
			splitView2.Add(fixedPane);

			var flexedPane = new VisualElement();
			//flexedPane.Add(CreateTopView());
			flexedPane.Add(splitView2);
			flexedPane.Add(CreateBottomView());

			flexedPane.style.minWidth = 200;

			splitView.Add(CreateBoardView());
			splitView.Add(flexedPane);

			if (runtimeOnly)
			{
				var toolbar = new RuntimeToolbar();
				toolbar.name = "toolbar";
				Add(toolbar);
			}
			else
			{
#if UNITY_EDITOR
				var toolbar = new EditorToolbar();
				toolbar.name = "toolbar";
				Add(toolbar);
#endif
			}
			RuntimeOnly = runtimeOnly;
			style.width = new StyleLength(new Length(100, LengthUnit.Percent));
			style.height = new StyleLength(new Length(100, LengthUnit.Percent));
			style.flexDirection = FlexDirection.Column;
			Add(splitView);

			rootVisualElement.Add(this);
			_rootVisualElement = rootVisualElement;
		}

		public T Q<T>() where T : VisualElement
        {
			return _rootVisualElement.Q<T>();
        }

		public T Q<T>(string name) where T : VisualElement
        {
			return _rootVisualElement.Q<T>(name);
        }

		public void SetListener(IListener l)
        {
			_listener = l;
        }

		public void SetToolbarListener(IToolbar l)
        {
			var runtimeToolbar = Q<RuntimeToolbar>();
			if (runtimeToolbar != null)
				runtimeToolbar.SetListener(l);
#if UNITY_EDITOR
			var editorToolbar = Q<EditorToolbar>();
			if (editorToolbar != null)
				editorToolbar.SetListener(l);
#endif
		}

		private VisualElement CreateTopView()
		{
			VisualElement topView = new();

			return topView;
		}

		private VisualElement GenerateContentView()
		{
			VisualElement contentView = new();

			var columns = new Columns();
			MultiColumnListView = new(columns);
			MultiColumnListView.style.flexGrow = 0;

			SortColumnDescriptions sortColumnDescriptions = new();
			List<SortColumnDescription> sortColumns = new();
			MultiColumnListViewControl = new MyMultiColumnListViewController(columns, sortColumnDescriptions, sortColumns);
			MultiColumnListView.SetViewController(MultiColumnListViewControl);
			MultiColumnListView.style.flexGrow = 1;

			contentView.Add(MultiColumnListView);
			return contentView;
		}

		private VisualElement CreateBottomView()
		{
			VisualElement bottomView = new();
			ProgressBar = new ProgressBar();
			ProgressBar.style.display = DisplayStyle.None;
			ProgressBar.lowValue = 0f;
			ProgressBar.highValue = 1f;
			ProgressBar.value = 0f;
			bottomView.Add(ProgressBar);
			return bottomView;
		}

		private VisualElement CreateBoardView()
		{
			var boardView = new VisualElement() { name = "boardPane" };

#if UNITY_EDITOR
			if (RuntimeOnly)
			{
				var textField = new TextField();
				textField.RegisterValueChangedCallback(OnKeywordChanged);
				boardView.Add(textField);
			}
			else
			{
				var searchField = new ToolbarSearchField();
				searchField.RegisterValueChangedCallback(OnKeywordChanged);
				boardView.Add(searchField);
			}
#else
			var textField = new TextField();
			textField.RegisterValueChangedCallback(OnKeywordChanged);
			boardView.Add(textField);
#endif
			SheetListView = new SheetListView();
			SheetListView.selectionType = SelectionType.Single;
			SheetListView.selectedIndicesChanged += OnSelectedIndicesChanged;			
			boardView.Add(SheetListView);
			return boardView;
		}

		private void OnSelectedIndicesChanged(IEnumerable<int> indices)
        {
			_listener?.OnSelectedIndicesChange(SheetListView.selectedIndex);
        }

		private VisualElement CreateInspectorPane()
		{
			var inspectorPane = new VisualElement();
			var inspectorView = new VisualElement();
			{
				var sheetNameField = new TextField("Sheet Name") { name = "SheetName" };
				sheetNameField.isReadOnly = true;

				var workbookNameField = new TextField("Workbook Name") { name = "WorkbookName" };
				workbookNameField.isReadOnly = true;

				var rowField = new TextField("Row") { name = "Row" };
				rowField.isReadOnly = true;

				var columnField = new TextField("Column") { name = "Column" };
				columnField.isReadOnly = true;

				inspectorView.Add(sheetNameField);
				inspectorView.Add(workbookNameField);
				inspectorView.Add(rowField);
				inspectorView.Add(columnField);

				//var foldout = new Foldout() { text = "Columns" };
				ColumListView = new ColumnListView();

				//foldout.Add(ColumListView);
				inspectorView.Add(ColumListView);
			}
			inspectorView.style.flexGrow = 1;
			inspectorPane.Add(inspectorView);

			var bottomView = new VisualElement();
			{
				var exportButton = new Button(OnClickExportButton) { text = "Export", name = "Export" };
				exportButton.style.display = DisplayStyle.None;

				var exporeButton = new Button(OnClickExploreButton) { text = "Show in Explorer", name = "Explore" };
				exporeButton.style.display = DisplayStyle.None;

				var openButton = new Button(OnClickOpenButton) { text = "Open in Excel", name = "Open" };
				openButton.style.display = DisplayStyle.None;

				bottomView.Add(exportButton);
				bottomView.Add(exporeButton);
				bottomView.Add(openButton);
			}
			bottomView.style.flexDirection = FlexDirection.Row;
			inspectorPane.Add(bottomView);

			return inspectorPane;
		}

		void IProgressBar.DisplayProgressBar()
        {
			ProgressBar.style.display = DisplayStyle.Flex;
			ProgressBar.value = 0.05f;
        }

		void IProgressBar.DisplayProgressBar(float progress)
        {
			ProgressBar.style.display = DisplayStyle.Flex;
			ProgressBar.value = progress;
		}

		void IProgressBar.ClearProgressBar()
        {
			ProgressBar.style.display = DisplayStyle.None;
		}

		#region UIActions

		private void OnClickExportButton()
        {
			_listener?.OnClickExportButton();
        }

		private void OnClickExploreButton()
		{
			_listener?.OnClickExploreButton();
		}

		private void OnClickOpenButton()
		{
			_listener?.OnClickOpenButton();
		}

		private void OnKeywordChanged(ChangeEvent<string> evt)
        {
			_listener?.OnKeywordChanged(evt);
		}

#endregion UIActions
	}
}