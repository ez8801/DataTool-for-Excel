using System.Linq;
using System.Collections.Generic;

namespace EZ.DataTool
{
    public class RootViewModel
    {
        private List<TableInfo> _tables;
        private List<TableInfo> _selectableTables;
        private string _searchKeyword;

        public List<string> SheetNames
        {
            get
            {
                var itemSource = _selectableTables
                    .Select(x => x.TableName)
                    .ToList();
                return itemSource;
            }
        }

        public RootViewModel()
        {
            _tables = new List<TableInfo>();
            _selectableTables = new List<TableInfo>();
            _searchKeyword = string.Empty;
        }

        public void SetSearchKeyword(string searchKeyword)
        {
            _searchKeyword = searchKeyword;
            ReloadData();
        }

        private void ReloadData()
        {
            _selectableTables.Clear();
            if (false == string.IsNullOrEmpty(_searchKeyword))
            {
                _selectableTables.AddRange(_tables
                    .Where(x => x.TableName.Contains(_searchKeyword, System.StringComparison.OrdinalIgnoreCase)));
            }
            else
                _selectableTables.AddRange(_tables);
        }

        public TableInfo GetTableInfo(int index)
        {
            if (_selectableTables != null && index >= 0 && index < _selectableTables.Count)
            {
                return _selectableTables[index];
            }
            return null;
        }

        public TableInfo GetTableInfo(string sheetName)
        {
            return _tables.Find(x => x.TableName == sheetName);
        }

        public void AddRange(IEnumerable<TableInfo> tables)
        {
            if (tables != null)
            {
                _tables.AddRange(tables);
                ReloadData();
            }
        }
    }
}