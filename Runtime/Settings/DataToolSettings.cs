using System.IO;

namespace EZ.DataTool.Settings
{
    public class DataToolSettings
    {
        public string DocumentsPath;
        public string NormalizedDocumentsPath { get; private set; }

        public DataToolSettings()
        {
            
        }

        public void Load()
        {
            DocumentsPath = UnityEngine.PlayerPrefs.GetString("UserSettings/DocumentsPath", string.Empty);
            OnLoaded();
        }

        private void OnLoaded()
        {
            if (false == string.IsNullOrEmpty(DocumentsPath))
            {
                NormalizedDocumentsPath = new DirectoryInfo(DocumentsPath)
                    .FullName;
            }
            else
                NormalizedDocumentsPath = string.Empty;
        }

        public void Save()
        {
            UnityEngine.PlayerPrefs.SetString("UserSettings/DocumentsPath", DocumentsPath);
        }
    }
}