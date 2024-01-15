using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using EZ.DataTool.View;
using EZ.DataTool.Util;
using EZ.DataTool.Settings;
using EZ.DataTool.Model;
using EZ.Data;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace EZ.DataTool
{
	public class RootViewController : RootView.IListener, IToolbar, IProgressBar
	{
		private TableInfo _selectedTableInfo;
		private DataTableContext _loadedTable;

		private Dictionary<TableInfo, DataTableContext> _loadedTables;

		private RootView _rootView;
		private RootViewModel _rootViewModel;
		private DataManifest _dataManifest;
		private DataToolSettings _dataToolSettings;
		private DataTableCache _dataTableCache;

		private IProgressBar _progressBar;
		private ICloser _closer;

		public RootViewController(RootView rootView, ICloser closer)
        {
			_rootView = rootView;
			_rootView.SetListener(this);
			_rootView.SetToolbarListener(this);
			
			_closer = closer;

			_rootViewModel = new RootViewModel();
			_dataManifest = new DataManifest();
			_dataManifest.Load();
			_dataToolSettings = new DataToolSettings();
			_dataToolSettings.Load();
			_dataTableCache = new DataTableCache();

			_loadedTables = new Dictionary<TableInfo, DataTableContext>();
		}

		public void SetProgressBar(IProgressBar progressBar)
        {
			_progressBar = progressBar;
		}

		public void Refresh(bool forceUpdateManifest = false)
        {
			if (_dataManifest.WorkbookCount > 0 && !forceUpdateManifest)
			{
				var tables = _dataManifest.Workbooks
					.Where(x => File.Exists(x.WorkbookPath))
					.Select(x => new TableInfo(x.WorkbookPath, x.SheetName));

				//var tables = _dataManifest.Workbooks
				//	.Select(x =>
				//{
				//	var fullName = _dataToolSettings.NormalizedDocumentsPath + x.WorkbookPath;
				//	return (fullName, x.SheetName);
				//})
				//	.Where(x => File.Exists(x.fullName))
				//.Select(x => new TableInfo(x.fullName, x.SheetName));
				_rootViewModel.AddRange(tables);

				RefreshSheetItems();
			}
			else
			{
				var docsPath = _dataToolSettings.NormalizedDocumentsPath;
				if (false == string.IsNullOrEmpty(docsPath))
				{
					DisplayProgressBar(0f);
					UpdateManifest(docsPath);
				}
			}
		}

		public void RefreshSheetItems()
        {
			var itemSource = _rootViewModel.SheetNames;
			var sheetListView = _rootView.SheetListView;
			sheetListView.itemsSource = itemSource;
			sheetListView.RefreshItems();
		}

		public void UpdateManifest(string docsPath)
		{
			var sheetNameValidator = new SheetNameValidator();

			try
			{
				var documentsFinder = new DocumentsFinder(sheetNameValidator, FileUtils.IsSupportedExtension);
				var documents = documentsFinder.FindAllDocuments(docsPath);
				var threadSafeDocs = new ThreadSafeDocuments(documents);
				var sheetsFinder = new SheetsFinder(sheetNameValidator);

				async void LoadTables()
				{
					//var sw = System.Diagnostics.Stopwatch.StartNew();
					await LoadTablesAsync(threadSafeDocs);
					//sw.Stop();

					//Debug.Log("Done: " + sw.ElapsedMilliseconds);

					//
					var entries = threadSafeDocs.TableInfos
						.Select(x => new DataManifest.Entry(x.FullName, x.TableName));

					//	.Select(x => {
					//	var subPath = x.FullName.Replace(docsPath, string.Empty);
					//	return new DataManifest.Entry(subPath, x.TableName);
					//});
					_dataManifest.AddEntries(entries);
					_dataManifest.Save();

					_rootViewModel.AddRange(threadSafeDocs.TableInfos);
					RefreshSheetItems();
					ClearProgressBar();
				}
				LoadTables();

				async Task LoadTablesAsync(ThreadSafeDocuments threadSafeDocs)
				{
					while (true)
					{
						var document = threadSafeDocs.Pop();
						DisplayProgressBar(threadSafeDocs.Ratio);
						if (document != null)
						{
							await sheetsFinder.LoadAsync(document);
							_dataTableCache.Put(document.FullName, sheetsFinder.Tables);
							threadSafeDocs.AddTableInfos(sheetsFinder.ToSheets(document));
							
							//var results = await sheetsFinder.FindAllSheetsAsync(document);
							//threadSafeDocs.AddTableInfos(results);
						}
						else
							break;
					}
				}
			}
			catch (IOException e)
			{
				Debug.LogError(e.ToString());
				ClearProgressBar();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				ClearProgressBar();
			}
		}

		public void OnTableLoaded()
		{
			var firstRowNum = _loadedTable.FirstRowNum;
			var rowCount = _loadedTable.DataTable.Rows.Count - firstRowNum;
			var columnCount = _loadedTable.Columns.Count;
			var tableName = _loadedTable.DataTable.TableName;
			var @namespace = _selectedTableInfo.Namespace;
			//Debug.Log($"OnTableLoaded() Name: {tableName} Row: {rowCount}, Col: {columnCount}");

			var sheetNameField = _rootView.Q<TextField>("SheetName");
			sheetNameField.value = tableName;

			var workbookNameField = _rootView.Q<TextField>("WorkbookName");
			workbookNameField.value = @namespace;

			var rowField = _rootView.Q<TextField>("Row");
			rowField.value = rowCount.ToString();

			var columnField = _rootView.Q<TextField>("Column");
			columnField.value = columnCount.ToString();

			var columnsView = _rootView.ColumListView;
			columnsView.SetItemsSource(_loadedTable.Columns);

			var mclv = _rootView.MultiColumnListView;
			_rootView.MultiColumnListViewControl.SetEditMode();
			mclv.columns.Clear();
			foreach (var columnData in _loadedTable.Columns)
			{
				var title = columnData.Name;
				var column = new Column() { title = title };
				column.minWidth = Mathf.Max(title.Length * 10, 82);
				mclv.columns.Add(column);
			}

			mclv.Clear();
			var rowCollection = new List<System.Data.DataRow>();
			var builder = new System.Text.StringBuilder();
			for (var i = firstRowNum; i < _loadedTable.DataTable.Rows.Count; i++)
			{
				var row = _loadedTable.DataTable.Rows[i];
				var items = row.ItemArray.Where((value, index) =>
				{
					return _loadedTable.Columns.FindIndex(x => x.Ordinal == index + 1) != -1;
				}).Select(x => x).ToArray();
				row.ItemArray = items;
				builder.AppendLine($"id: {i}, itemLen: {row.ItemArray?.Length ?? 0}");

				rowCollection.Add(row);
			}
			//Debug.Log(builder.ToString());
			_rootView.MultiColumnListViewControl.DisableEditMode();
			mclv.itemsSource = rowCollection;

			columnsView.RefreshItems();
			// mclv.Rebuild();
			mclv.RefreshItems();

			var exportButton = _rootView.Q<Button>("Export");
			exportButton.style.display = DisplayStyle.Flex;

			var exploreButton = _rootView.Q<Button>("Explore");
			exploreButton.style.display = DisplayStyle.Flex;

			var openButton = _rootView.Q<Button>("Open");
			openButton.style.display = DisplayStyle.Flex;
		}

		private void SelectTable(TableInfo tableInfo)
        {
			if (_loadedTables.ContainsKey(tableInfo))
			{
				_selectedTableInfo = tableInfo;
				_loadedTable = _loadedTables[tableInfo];
				OnTableLoaded();
				return;
			}

			DisplayProgressBar();
			var loader = new Loader.WorkSheetLoader(tableInfo.FullName, tableInfo.TableName, _dataTableCache);
			loader.SetOnLoadListener((ctx) =>
			{
				if (ctx != null)
				{
					if (_loadedTables.ContainsKey(tableInfo))
						_loadedTables[tableInfo] = ctx;
					else
						_loadedTables.Add(tableInfo, ctx);
				}
				_selectedTableInfo = tableInfo;
				_loadedTable = ctx;
				OnTableLoaded();

				ClearProgressBar();
				loader = null;
			});

			try
			{
				loader.LoadSync();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.ToString());
				ClearProgressBar();
				return;
			}
		}

		public void DisplayProgressBar()
        {
			IProgressBar progressBar = _rootView;
			progressBar.DisplayProgressBar();
			_progressBar?.DisplayProgressBar();
		}

		public void DisplayProgressBar(float progress)
        {
			IProgressBar progressBar = _rootView;
			progressBar.DisplayProgressBar(progress);
			_progressBar?.DisplayProgressBar(progress);
		}

		public void ClearProgressBar()
        {
			IProgressBar progressBar = _rootView;
			progressBar.ClearProgressBar();
			_progressBar?.ClearProgressBar();
		}

		void RootView.IListener.OnKeywordChanged(ChangeEvent<string> evt)
		{
			_rootViewModel.SetSearchKeyword(evt.newValue);
			if (evt.previousValue != evt.newValue)
            {
				RefreshSheetItems();
            }
		}

		void RootView.IListener.OnSelectedIndicesChange(int index)
		{
			var tableInfo = _rootViewModel.GetTableInfo(index);
			if (tableInfo != null)
				SelectTable(tableInfo);
		}

		void RootView.IListener.OnClickOpenButton()
		{
			var path = _selectedTableInfo.FullName;
			if (File.Exists(path))
			{
				System.Diagnostics.Process.Start(path);
			}
		}

		void RootView.IListener.OnClickExploreButton()
		{
			var path = _selectedTableInfo.FullName;
			if (File.Exists(path))
			{
				System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{path}\"");
			}
		}

		void RootView.IListener.OnClickExportButton()
		{
			var @namespace = _selectedTableInfo.Namespace;
			var tableName = _selectedTableInfo.TableName;
			GenerateScript(@namespace, tableName);
			ExportTSV(@namespace, tableName);

#if UNITY_EDITOR
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			//var scriptPath = $"Scripts/Data/Runtime/{@namespace}/{tableName}DbfRecord.cs";
			//var obj = AssetDatabase.LoadMainAssetAtPath(scriptPath);
			//if (obj != null)
			//	EditorGUIUtility.PingObject(obj);

			//var tsvPath = Dbf.GetTsvPath(tableName);
			//obj = AssetDatabase.LoadMainAssetAtPath(tsvPath);
			//if (obj != null)
			//	EditorGUIUtility.PingObject(obj);
#endif

			void GenerateScript(string @namespace, string tableName)
			{
				TextTemplate.TextTemplate textTemplate = new();
				var text = textTemplate.TransformText(new TextTemplate.Args()
				{
					Context = _loadedTable
				});

				var fileName = PathUtils.GetDataRecordScriptPath(@namespace, tableName);
				var fileInfo = new FileInfo(Path.Combine(Application.dataPath, fileName));
				if (false == fileInfo.Directory.Exists)
					fileInfo.Directory.Create();
				File.WriteAllText(fileInfo.FullName, text, System.Text.Encoding.UTF8);
			}

			string MakeTsvText(IEnumerable<string> values)
			{
				return string.Join('\t', values);
			}

			void ExportTSV(string @namespace, string tableName)
			{
				var fileName = PathUtils.GetTsvPath(tableName);
				var fileInfo = new FileInfo(Path.Combine(Application.dataPath, fileName));
				var firstRowNum = _loadedTable.FirstRowNum;
				var builder = new System.Text.StringBuilder();

				builder.AppendLine(MakeTsvText(_loadedTable.Columns.Select(x => x.Name)));
				for (int i = firstRowNum; i < _loadedTable.DataTable.Rows.Count; i++)
				{
					var row = _loadedTable.DataTable.Rows[i];
					var items = row.ItemArray.Where((value, index) =>
					{
						return _loadedTable.Columns.FindIndex(x => x.Ordinal == index + 1) != -1;
					}).Select(x => x.ToString());

					// var items = row.ItemArray.Select(x => x.ToObject<string>());
					builder.AppendLine(MakeTsvText(items));
				}

				if (false == fileInfo.Directory.Exists)
					fileInfo.Directory.Create();
				File.WriteAllText(fileInfo.FullName, builder.ToString(), System.Text.Encoding.UTF8);
			}
		}

		void OnOpenDocumentsFolder()
		{
			var docsPath = _dataToolSettings.DocumentsPath;			
			System.Diagnostics.Process.Start(docsPath);
		}

		void IToolbar.OnClickHelp()
		{
#if UNITY_EDITOR
			_dataManifest.Log();
#endif
			if (false == _rootView.RuntimeOnly)
            {
#if UNITY_EDITOR
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("About"), false, () =>
				{
					var message = $"Version: {Application.version}\nDbf Version: {DbfVersion.Version}";
					EditorUtility.DisplayDialog("Data Tool", message, "OK");
				});
				menu.AddItem(new GUIContent("Release Notes"), false, null);
				
				// display the menu
				var helpButton = _rootView.Q<ToolbarButton>("Help");
				menu.DropDown(helpButton.worldBound);
#endif
			}
			else
            {
				var helpButton = _rootView.Q<Button>("Help");

				var dropdownMenu = new GenericDropdownMenu();
				dropdownMenu.AddItem("About", false, () => 
				{
					var popupView = new PopupView(_rootView.RootVisualElement);
					var message = $"Version: {Application.version}\nDbf Version: {DbfVersion.Version}";
					_rootView.RootVisualElement.Q("toolbar").style.display = DisplayStyle.None;
					_rootView.RootVisualElement.Q("contentView").style.display = DisplayStyle.None;
					popupView.Show("About", message);

					//_rootView.RootVisualElement.Q("toolbar").style.display = DisplayStyle.Flex;
					//_rootView.RootVisualElement.Q("contentView").style.display = DisplayStyle.Flex;
					//_rootView.RootVisualElement.Remove(popup);
				});

				dropdownMenu.AddItem("Release Notes", false, () =>
				{

				});

				dropdownMenu.DropDown(helpButton.worldBound, _rootView.RootVisualElement);
			}
		}

		private void OpenFileEditorMenu()
        {
#if UNITY_EDITOR
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Open/Documents Folder"), false, OnOpenDocumentsFolder);

			menu.AddSeparator(string.Empty);

			menu.AddItem(new GUIContent("Preferences/General"), false, () =>
			{
				SettingsService.OpenUserPreferences("Preferences/Data Tool");
			});

			menu.AddSeparator(string.Empty);

			menu.AddItem(new GUIContent("Refresh"), false, () =>
			{
				Refresh(true);
			});
			menu.AddItem(new GUIContent("Exit"), false, () =>
			{
				_closer?.Close();
			});

			// display the menu
			var fileButton = _rootView.Q<Button>("File");
			menu.DropDown(fileButton.worldBound);
#endif
		}

		private void OpenFIleMenu()
        {
			var fileButton = _rootView.Q<Button>("File");

			var dropdownMenu = new GenericDropdownMenu();
			dropdownMenu.AddItem("Open/Documents Folder", false, OnOpenDocumentsFolder);

			dropdownMenu.AddSeparator(string.Empty);

			dropdownMenu.AddItem("Preferences/General", false, () => 
			{
				
			});

			dropdownMenu.AddSeparator(string.Empty);

			dropdownMenu.AddItem("Refresh", false, () => { });
			dropdownMenu.AddItem("Exit", false, () => 
			{
				_closer?.Close();
			});
			dropdownMenu.DropDown(fileButton.worldBound, _rootView.RootVisualElement);
		}

		void IToolbar.OnClickFile()
		{
#if UNITY_EDITOR
			if (false == _rootView.RuntimeOnly)
				OpenFileEditorMenu();
#endif

			if (_rootView.RuntimeOnly)
				OpenFIleMenu();
		}
	}
}