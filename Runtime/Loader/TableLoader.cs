using EZ.DataTool.Model;
using System.Collections;

namespace EZ.DataTool.Loader
{
    public class TableLoader
    {
        public delegate void OnTableLoaded(DataTableContext ctx);

        public string Path { get; protected set; }
        public string TableName { get; protected set; }

        protected OnTableLoaded _onLoadListener;
        
        public TableLoader(string path, string tableName)
        {
            Path = path;
            TableName = tableName;
        }

        public virtual IEnumerator Load()
        {
            yield return null;
        }

        protected void OnLoaded(DataTableContext loadedTable)
        {
            if (_onLoadListener != null)
                _onLoadListener.Invoke(loadedTable);
        }

        public void SetOnLoadListener(OnTableLoaded l)
        {
            _onLoadListener = l;
        }
    }
}
