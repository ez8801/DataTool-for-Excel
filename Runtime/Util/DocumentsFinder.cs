using System;
using System.IO;
using System.Collections.Generic;
using EZ.DataTool.Util;

namespace EZ.DataTool
{
    public class DocumentsFinder
    {
		private SheetNameValidator _sheetNameValidator;
		private Func<string, bool> _isSupportedExtension;

		public DocumentsFinder(SheetNameValidator sheetNameValidator, Func<string, bool> isSupportedExtension)
        {
			_sheetNameValidator = sheetNameValidator;
			_isSupportedExtension = isSupportedExtension;
		}

		public List<FileInfo> FindAllDocuments(string path)
		{
			if (false == Directory.Exists(path))
				throw new DirectoryNotFoundException(path);

			path = path.Replace("\\", "/");

			var results = new List<FileInfo>();
			var di = new DirectoryInfo(path);
			FileInfo[] fileInfos = di.GetFiles();
			for (int i = 0; i < fileInfos.Length; i++)
			{
				FileInfo fi = fileInfos[i];
				if (fi.Attributes.HasFlag(FileAttributes.Hidden))
					continue;

				if (false == _sheetNameValidator.IsValid(fi.Name))
					continue;

				if (_isSupportedExtension(fi.Extension))
					results.Add(fi);
			}

			return results;
		}
	}
}