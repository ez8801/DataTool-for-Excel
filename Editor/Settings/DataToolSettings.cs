using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EZ.DataTool.Settings
{
    //[FilePath("UserSettings/DataToolSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    //public class DataToolSettings : ScriptableSingleton<DataToolSettings>
    //{
    //    public string DocumentsPath;
    //
    //    public void Save() 
    //    { 
    //        Save(true); 
    //    }
    //}

    public class DataManifestView : View.CustomListView
    {
        private DataManifest _dataManifest;
        
        public DataManifestView(DataManifest dataManifest)
            : base()
        {
            _dataManifest = dataManifest;

            headerTitle = "Entries";
            showBoundCollectionSize = true;
            showBorder = true;
            showAddRemoveFooter = true;
            showFoldoutHeader = true;

            fixedItemHeight = 46;

            itemsAdded += OnItemsAdded;
            itemsRemoved += OnItemsRemoved;
        }

        private void OnItemsAdded(IEnumerable<int> items)
        {
            itemsSource[items.FirstOrDefault()] = new DataManifest.Entry(string.Empty, string.Empty);
            RefreshItem(items.FirstOrDefault());
        }

        private void OnItemsRemoved(IEnumerable<int> items)
        {
            _dataManifest.Save();
        }

        public override void BindItem(VisualElement element, int index)
        {
            element.name = index.ToString();
            var workbookField = element.Q<TextField>("Workbook");
            workbookField.RegisterValueChangedCallback(OnWorkbookChanged);

            var sheetField = element.Q<TextField>("Sheet");
            sheetField.RegisterValueChangedCallback(OnSheetChanged);

            if (itemsSource[index] is DataManifest.Entry entryData)
            {
                workbookField.value = entryData.WorkbookPath;
                sheetField.value = entryData.SheetName;
            }
            else
            {
                workbookField.value = string.Empty;
                sheetField.value = string.Empty;
            }
        }

        private void OnSheetChanged(ChangeEvent<string> evt)
        {
            if (evt.currentTarget is VisualElement view
                && evt.previousValue != evt.newValue)
            {
                if (int.TryParse(view.parent.name, out var index))
                {
                    _dataManifest.Workbooks[index].SheetName = evt.newValue;
                    _dataManifest.Save();
                }
            }
        }

        private void OnWorkbookChanged(ChangeEvent<string> evt)
        {
            if (evt.currentTarget is VisualElement view
                && evt.previousValue != evt.newValue)
            {
                if (int.TryParse(view.parent.name, out var index))
                {
                    _dataManifest.Workbooks[index].WorkbookPath = evt.newValue;
                    _dataManifest.Save();
                }
            }
        }

        public override void UnbindItem(VisualElement element, int index)
        {
            base.UnbindItem(element, index);
            var workbookField = element.Q<TextField>("Workbook");
            workbookField.UnregisterValueChangedCallback(OnWorkbookChanged);
            workbookField.value = string.Empty;

            var sheetField = element.Q<TextField>("Sheet");
            sheetField.UnregisterValueChangedCallback(OnSheetChanged);
            sheetField.value = string.Empty;
        }

        public override VisualElement MakeItem()
        {
            var view = new VisualElement();
            var workbookField = new TextField("Workbook") { name = "Workbook" };            
            view.Add(workbookField);

            var sheetField = new TextField("Sheet") { name = "Sheet" };            
            view.Add(sheetField);
            return view;
        }
    }

    sealed class DataToolSettingsProvider : SettingsProvider
    {
        private VisualElement _rootElement;
        private DataToolSettings _dataToolSettings;
        private DataManifest _dataManifest;

        public DataToolSettingsProvider()
          : base("Preferences/Data Tool", SettingsScope.User) 
        {
            _dataToolSettings = new DataToolSettings();
            _dataManifest = new DataManifest();            
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            _dataToolSettings.Load();
            _dataManifest.Load();

            if (rootElement.Q("viewport") != null)
            {
                Debug.Log("!!!!");
            }

            var settingsView = new VisualElement();
            {
                settingsView.style.marginBottom = 8;
                settingsView.style.marginTop = 2;
                settingsView.style.marginLeft = 8;
                settingsView.style.marginRight = 8;

                var titleLabel = GetBoldLabel("DataTool");
                titleLabel.style.fontSize = 20;
                settingsView.Add(titleLabel);

                var viewport = new VisualElement() { name = "viewport"};
                {
                    viewport.Add(GetBoldLabel("Properties"));

                    var docsPath = _dataToolSettings.DocumentsPath;
                    var pathField = MakePathField("Documents Path", docsPath, "Select Documents Folder", (path) =>
                    {
                        var dirty = false;
                        if (_dataToolSettings.DocumentsPath != path)
                        {
                            _dataToolSettings.DocumentsPath = path;
                            _dataToolSettings.Save();

                            dirty = true;
                        }

                        if (dirty)
                            DataToolWindow.CreateWindow().Initialize(true);
                        else if (EditorWindow.HasOpenInstances<DataToolWindow>())
                            DataToolWindow.Window.Initialize();
                    });
                    viewport.Add(pathField);

                    /*
                    var view = new VisualElement();
                    view.style.flexDirection = FlexDirection.Row;
                    view.style.flexShrink = 0;

                    var textField = new TextField("Documents Path") { name = "DocumentsPath" };
                    textField.value = _dataToolSettings.NormalizedDocumentsPath;
                    textField.style.flexGrow = 1;

                    view.Add(textField);
                    view.Add(new Button(OnBrowse) { name = "BrowseButton", text = "Browse" });
                    viewport.Add(view);
                    */

                    viewport.Add(GetBoldLabel("Data Manifest"));

                    var dataManifestView = new DataManifestView(_dataManifest);
                    dataManifestView.itemsSource = _dataManifest.Workbooks;
                    viewport.Add(dataManifestView);
                }
                settingsView.Add(viewport);
            };

            rootElement.Add(settingsView);
            _rootElement = rootElement;
        }

        public Label GetBoldLabel(string text)
        {
            var label = new Label(text);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            return label;
        }

        private VisualElement MakePathField(string label, string value, string panelTitle, System.Action<string> callback)
        {
            void OnBrowse()
            {
                string path = EditorUtility.OpenFolderPanel(panelTitle, string.Empty, string.Empty);
                if (Directory.Exists(path))
                {
                    var textField = _rootElement.Q<TextField>(label);
                    textField.value = path;

                    callback?.Invoke(path);                    
                }
            }

            var pathField = new VisualElement();
            {
                pathField.style.flexDirection = FlexDirection.Row;
                pathField.style.flexShrink = 0;

                var textField = new TextField(label) { name = label };
                textField.value = value;
                textField.style.flexGrow = 1;
                pathField.Add(textField);

                var browseButton = new Button(OnBrowse) { name = "BrowseButton", text = "Browse" };
                pathField.Add(browseButton);
            }
            return pathField;
        }

        private void OnBrowse()
        {
            string path = EditorUtility.OpenFolderPanel($"Select Documents Folder", string.Empty, string.Empty);
            if (Directory.Exists(path))
            {                
                var textField = _rootElement.Q<TextField>("DocumentsPath");
                textField.value = path;

                _dataToolSettings.DocumentsPath = path;
                _dataToolSettings.Save();

                if (EditorWindow.HasOpenInstances<DataToolWindow>())
                    DataToolWindow.Window.Initialize();
            }
        }
    }

    static class DataToolSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
          => new DataToolSettingsProvider();
    }
}