using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace EZ.DataTool.Settings
{
	public class DataManifest
	{
		[System.Serializable]
		public class Entry
		{
			public string WorkbookPath;
			public string SheetName;

			public Entry(string path, string sheetName)
			{
				WorkbookPath = path;
				SheetName = sheetName;
			}

            public override string ToString()
            {
				return $"WorkbookPath: {WorkbookPath}, SheetName: {SheetName}";
            }
        }

		[SerializeField]
		public List<Entry> Workbooks;

		public int WorkbookCount => Workbooks.Count;

		public DataManifest()
		{
			Workbooks = new List<Entry>();
		}

		~DataManifest()
		{
			Workbooks.Clear();
		}

		public void OnValidate()
		{
			Debug.Log("OnValidate");			
			// UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(GetFilePath());
		}

		public int FindIndex(string sheetName)
		{
			return Workbooks.FindIndex(x => x.SheetName == sheetName);
		}

		public void AddEntries(IEnumerable<Entry> entries)
		{
			Workbooks.Clear();
			if (entries != null)
				Workbooks.AddRange(entries);
		}

		public void Log()
		{
			if (!(Workbooks == null || Workbooks.Count == 0))
			{
				var builder = new System.Text.StringBuilder();
				foreach (var entry in Workbooks)
				{
					builder.AppendLine($"{entry.WorkbookPath}\t{entry.SheetName}");
				}
				Debug.Log(builder.ToString());
			}
			else
				Debug.LogWarning("DataManifest is Empty");
		}

		public void Load()
        {
			var settings = PlayerPrefs.GetString("UserSettings/DataManifest", string.Empty);
			Workbooks.Clear();
			if (false == string.IsNullOrEmpty(settings))
            {
				using (var reader = new StringReader(settings))
                {
					while (reader.Peek() != -1)
                    {
						var pair = reader.ReadLine().Split('\t');
						if (pair != null && pair.Length == 2)
						{
							var entry = new Entry(pair[0], pair[1]);
							Workbooks.Add(entry);
						}
					}
				}
			}
			//Log();
		}

		public void Save()
		{
			var builder = new System.Text.StringBuilder();
			for (int i = 0; i < Workbooks.Count; i++)
			{
				var workbook = Workbooks[i];
				builder.AppendLine($"{workbook.WorkbookPath}\t{workbook.SheetName}");
			}

			var savedData = builder.ToString();
			PlayerPrefs.SetString("UserSettings/DataManifest", savedData);
		}
	}
}