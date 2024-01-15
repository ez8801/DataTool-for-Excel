using System.IO;
using System.Data;
using System.Collections.Generic;
using EZ.DataTool.Util;
using System.Threading.Tasks;
using ExcelDataReader;

namespace EZ.DataTool
{
	public class SheetsFinder
	{
		private SheetNameValidator _sheetNameValidator;
		private FileInfo _fileInfo;
		private DataTableCollection _tables;

		public DataTableCollection Tables => _tables;

		public SheetsFinder(SheetNameValidator sheetNameValidator)
        {
			_sheetNameValidator = sheetNameValidator;
			_tables = null;
		}

		public async Task LoadAsync(FileInfo fileInfo)
        {
			_fileInfo = fileInfo;
			var bytes = await File.ReadAllBytesAsync(_fileInfo.FullName);
			using (var bs = new BufferedStream(new MemoryStream(bytes), 2048))
			{
				using (var reader = ExcelReaderFactory.CreateReader(bs))
				{
					var dataSet = reader.AsDataSet(null);
					var tables = dataSet.Tables;
					_tables = tables;
				}
			}
		}

		public List<TableInfo> ToSheets(FileInfo fileInfo)
        {
			var tableInfos = new List<TableInfo>();
			for (int i = 0; i < _tables.Count; i++)
			{
				var sheetName = _tables[i].TableName;
				if (false == _sheetNameValidator.IsValid(sheetName))
					continue;
				
				var tableInfo = new TableInfo(fileInfo, sheetName);
				tableInfos.Add(tableInfo);
			}
			return tableInfos;
        }

		public async Task<List<TableInfo>> FindAllSheetsAsync(FileInfo fileInfo)
		{
			var tableInfos = new List<TableInfo>();
			var bytes = await File.ReadAllBytesAsync(fileInfo.FullName);
			using (var bs = new BufferedStream(new MemoryStream(bytes), 2048))
			{
				using (var reader = ExcelReaderFactory.CreateReader(bs))
				{
					var dataSet = reader.AsDataSet(null);
					var tables = dataSet.Tables;

					for (int i = 0; i < tables.Count; i++)
					{
						var sheetName = tables[i].TableName;
						if (false == _sheetNameValidator.IsValid(sheetName))
							continue;

						var tableInfo = new TableInfo(fileInfo, sheetName);
						tableInfos.Add(tableInfo);
					}
				}
			}
			return tableInfos;
		}
	}
}