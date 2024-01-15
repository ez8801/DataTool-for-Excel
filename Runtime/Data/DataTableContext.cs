using System.Data;
using System.Collections.Generic;

namespace EZ.DataTool
{
    public class DataTableContext
    {
        public List<DbfColumn> Columns;
        public DataTable DataTable { get; private set; }
        public int FirstRowNum { get; private set; }

        public DataTableContext()
        {
            Columns = new List<DbfColumn>();
        }

        public void SetDataTable(DataTable dataTable)
        {
            DataTable = dataTable;
        }

        public void SetFirstRowNum(int firstRowNum)
        {
            FirstRowNum = firstRowNum;
        }

        public void OnColumnChanged()
        {
            if (Columns.Count == 0)
                throw ExceptionBuilder.ColumnNotInAnyTable();

            int primaryKeyCount = 0;
            var columnNames = new List<string>();

            for (int i = 0; i < Columns.Count; i++)
            {
                var column = Columns[i];
                if (column.Key == KeyType.PrimaryKey)
                    primaryKeyCount++;

                if (columnNames.Contains(column.Name))
                {
                    throw ExceptionBuilder.DuplicateColumnName(column.Name);
                }
                columnNames.Add(column.Name);
            }

            if (primaryKeyCount > 1)
                throw ExceptionBuilder.KeyTooManyColumns();
        }
    }
}