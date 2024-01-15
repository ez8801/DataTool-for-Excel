using System.IO;
using System.Collections.Generic;

namespace EZ.DataTool
{
	public class ThreadSafeDocuments
	{
		private object _lockObject;
		private int _documentCount;
		private List<FileInfo> _documents;

		public float Ratio { get; private set; }

		public List<TableInfo> TableInfos { get; private set; }

		public ThreadSafeDocuments(List<FileInfo> fileInfos)
		{
			_lockObject = new object();
			_documents = new List<FileInfo>(fileInfos);
			_documentCount = _documents.Count;

			Ratio = 0f;

			TableInfos = new List<TableInfo>();
		}

		public FileInfo Pop()
		{
			lock (_lockObject)
			{
				if (_documents.Count > 0)
				{
					var doc = _documents[_documents.Count - 1];
					_documents.RemoveAt(_documents.Count - 1);
					Ratio = 1 - ((float)_documents.Count / _documentCount);
					return doc;
				}
			}
			return null;
		}

		public void AddTableInfos(List<TableInfo> infos)
		{
			if (infos == null || infos.Count == 0)
				return;

			lock (_lockObject)
			{
				TableInfos.AddRange(infos);
			}
		}
	}
}