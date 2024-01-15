using System.Data;
using System.Collections.Generic;

namespace EZ.DataTool.Model
{
    public class DataTableCache
    {
        public Dictionary<string, DataTableCollection> Cache;

        public DataTableCache()
        {
            Cache = new Dictionary<string, DataTableCollection>();
        }

        public DataTableCollection Get(string path)
        {
            if (Cache.ContainsKey(path))
                return Cache[path];
            return null;
        }

        public void Put(string path, DataTableCollection tableCollection)
        {
            if (false == Cache.ContainsKey(path))
                Cache.Add(path, tableCollection);
            else
                Cache[path] = tableCollection;
        }
    }
}