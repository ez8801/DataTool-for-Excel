using System.IO;
using UnityEngine;
using EZ.DataTool.Model;
using ExcelDataReader;

namespace EZ.DataTool.Loader
{
    public class WorkSheetLoader : TableLoader
    {
        public static class RowId
        {
            public const int Caption = 0;       // 설명
            public const int ColumnName = 1;    // 컬럼명
            public const int Filter = 2;        // 필터
            public const int Key = 3;           // 키
            public const int DataType = 4;      // 데이터 타입
        }

        public const int RowStart = RowId.Caption;
        public const int RowEnd = RowId.DataType;
        public const int HeaderRowCount = RowEnd + 1;

        private DataTableCache _dataTableCache;

        public WorkSheetLoader(string path, string tableName, DataTableCache cache)
            : base(path, tableName)
        {
            _dataTableCache = cache;
        }

        public object Get(object[] l, int index)
        {
            if (l != null && index >= 0 && index < l.Length)
                return l[index];
            return null;
        }

        public void ReadHeader(ref System.Data.DataTable sheet, ref DbfColumn column, int cellnum)
        {
            if (cellnum < 0)
                throw new System.ArgumentOutOfRangeException("cellnum");

            for (int i = RowStart; i <= RowEnd; i++)
            {
                var row = sheet.Rows[i];
                var cell = Get(row.ItemArray, cellnum);
                string value = cell.ToString();
                switch (i)
                {
                    case RowId.ColumnName:
                        column.SetName(value);
                        break;
                    case RowId.Filter:
                        column.SetFilter(ColumnFilterResolver.Resolve(value));
                        break;
                    case RowId.Key:
                        column.SetKey(KeyTypeResolver.Resolve(value));
                        break;
                    case RowId.DataType:
                        column.SetDataType(value);
                        break;
                }
            }
        }

        private DataTableContext Parse(System.Data.DataTable sheet)
        {
            var dataTableContext = new DataTableContext();
            dataTableContext.SetDataTable(sheet);

            if (sheet.Rows.Count < HeaderRowCount)
            {
                throw ExceptionBuilder.RowNotInTable();
            }

            var firstRow = sheet.Rows[0];
            int columnCount = firstRow.ItemArray.Length;
            int rowCount = sheet.Rows.Count;

            for (int i = 0; i < columnCount; i++)
            {
                var column = new DbfColumn();
                ReadHeader(ref sheet, ref column, i);

                // Comment Column
                if (false == column.IsValid())
                {
                    //Debug.LogWarning($"Note Column({i}): {column.Name}");
                    continue;
                }

                if (false == column.Contains())
                {
                    //Debug.LogWarning($"Not Client Column: {column.ValueType} {column.Name} ({column.Filter})");
                    continue;
                }

                column.SetOrdinal(i + 1);
                dataTableContext.Columns.Add(column);
            }            
            dataTableContext.OnColumnChanged();
            dataTableContext.SetFirstRowNum(HeaderRowCount);

            //var builder = new System.Text.StringBuilder();
            //for (int i = HeaderRowCount; i < rowCount; i++)
            //{
            //    var row = sheet.Rows[i];
            //    var itemCount = row.ItemArray?.Length ?? 0;
            //    builder.AppendLine($"Id: {i}, ItemCount: {itemCount}");
            //}
            //Debug.Log(builder.ToString());

            //var rowMax = 0;
            //for (int i = 0; i < dataTableContext.Columns.Count; i++)
            //{
            //    var column = dataTableContext.Columns[i];
            //    for (int rowId = HeaderRowCount; rowId < rowCount; rowId++)
            //    {
            //        var row = sheet.Rows[rowId];
            //        if (row == null || (row.ItemArray == null || row.ItemArray.Length == 0))
            //            continue;
            //
            //        var cell = row.ItemArray[column.Ordinal - 1];
            //        if (cell == null)
            //        {
            //            Debug.Log($"Row: {rowId}, Col: {i}");
            //        }
            //        else
            //        {
            //            rowMax = Mathf.Max(rowId + 1, rowMax);
            //        }
            //    }
            //}
            return dataTableContext;
        }

        private System.Data.DataTable FindTable(System.Data.DataTableCollection tables)
        {
            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                if (table.TableName.Equals(TableName, System.StringComparison.OrdinalIgnoreCase))
                    return table;
            }
            return null;
        }

        public void LoadSync()
        {
            if (string.IsNullOrEmpty(Path))
            {
                throw new System.ArgumentException(Path);
            }

            string filePath = Path;            
            var cached = _dataTableCache.Get(filePath);
            if (cached != null)
            {
                System.Data.DataTable workSheet = FindTable(cached);
                if (workSheet != null)
                {
                    var tableContext = Parse(workSheet);
                    OnLoaded(tableContext);
                }
                else
                {
                    Debug.LogWarning("Not Found Sheet");
                }
            }
            else
            {
                using (var ms = new FileStream(filePath, FileMode.Open))
                {
                    using (var bs = new BufferedStream(ms, 2048))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(bs))
                        {
                            var dataSet = reader.AsDataSet(null);
                            var tables = dataSet.Tables;

                            _dataTableCache.Put(filePath, tables);

                            System.Data.DataTable workSheet = FindTable(tables);
                            if (workSheet != null)
                            {
                                var tableContext = Parse(workSheet);
                                OnLoaded(tableContext);
                            }
                            else
                            {
                                Debug.LogWarning("Not Foudn Sheet");
                            }
                        }
                    }
                }
            }
        }

        ~WorkSheetLoader()
        {
            _onLoadListener = null;
        }
    }
}
