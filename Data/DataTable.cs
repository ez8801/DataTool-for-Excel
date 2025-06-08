using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EZ.Data
{
    public abstract class DataRecord
    {
        public int ID { get; protected set; }
        public abstract void Deserialize(string[] fields);
    }

    public interface IDataTable
    {
        int RecordCount { get; }
        string GetName();
        DataRecord CreateNewRecord();
        void AddRecord(DataRecord record);
        void OnLoaded();
    }

    public class DataTable<T> : IDataTable where T : DataRecord, new()
    {
        private string _name;
        private List<T> _records = new List<T>();
        private Dictionary<int, T> _recordsById = new Dictionary<int, T>();

        public DataTable(string name)
        {
            _name = name;
        }

        public void CopyRecords(DataTable<T> other)
        {
            _records.AddRange(other._records);
            foreach (KeyValuePair<int, T> keyValuePair in other._recordsById)
                _recordsById.Add(keyValuePair.Key, keyValuePair.Value);
        }

        public DataRecord CreateNewRecord() => new T();

        public void AddRecord(DataRecord record)
        {
            T record1 = (T)record;
            _records.Add(record1);
            _recordsById[record.ID] = record1;
        }

        public List<T> GetRecords() => _records;

        public int RecordCount => _records?.Count ?? 0;

        public List<T> GetRecords(Predicate<T> predicate, int limit = -1)
        {
            return limit >= 0 ? _records.FindAll(predicate).GetRange(0, limit) : _records.FindAll(predicate);
        }

        public static async Task<DataTable<T>> Load(string name)
        {
#if UNITY_EDITOR
            var dbf = new DataTable<T>(name);
            var assetPath = PathUtils.GetTsvPath(name);
            var fullName = Path.Combine(Environment.CurrentDirectory, "Assets", assetPath);
            var text = await File.ReadAllTextAsync(fullName, System.Text.Encoding.UTF8);
            var stringReader = new StringReader(text);
            stringReader.ReadLine();
            while (stringReader.Peek() != -1)
            {
                var line = stringReader.ReadLine();
                var lineElements = line.Split('\t');
                var record = dbf.CreateNewRecord();
                record.Deserialize(lineElements);
                dbf.AddRecord(record);
            }
            dbf.OnLoaded();
            return dbf;
#else
            // Unsupported
            return null;
#endif
        }

        public virtual void OnLoaded()
        {

        }

        public string GetName() => _name;

        public void Clear()
        {
            _records.Clear();
            _recordsById.Clear();
        }

        public T GetRecord(string key)
        {
            return GetRecord(key.GetHashCode());
        }

        public T GetRecord(int id)
        {
            T record;
            _recordsById.TryGetValue(id, out record);
            return record;
        }

        public T GetRecord(Predicate<T> match) => _records.Find(match);

        public T GetRecordAt(int index)
        {
            if (!(index < 0 || index >= RecordCount))
            {
                return _records[index];
            }
            return default;
        }

        public T GetLastRecord()
        {
            var recordCount = RecordCount;
            if (recordCount > 0)
            {
                return GetRecordAt(recordCount - 1);
            }
            return default;
        }

        public bool TryGetRecord(string key, out T record)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (HasRecord(key))
                {
                    record = GetRecord(key);
                    return true;
                }                
            }

            record = null;
            return false;
        }

        public bool TryGetRecord(int key, out T record)
        {
            if (HasRecord(key))
            {
                record = GetRecord(key);
                return true;
            }

            record = null;
            return false;
        }

        public bool HasRecord(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                return _recordsById.ContainsKey(key.GetHashCode());
            }
            return false;
        }

        public bool HasRecord(int id)
        {
            T obj = default(T);
            _recordsById.TryGetValue(id, out obj);
            return obj != null;
        }

        public bool HasRecord(Predicate<T> match) => GetRecord(match) != null;

        public void ReplaceRecordByRecordId(T record)
        {
            int index = _records.FindIndex(r => r.ID == record.ID);
            if (index == -1)
            {
                AddRecord(record);
            }
            else
            {
                _records[index] = record;
                _recordsById[record.ID] = record;
            }
        }

        public void RemoveRecordsWhere(Predicate<T> match)
        {
            List<int> intList = null;
            int index1 = 0;
            for (int count = _records.Count; index1 < count; ++index1)
            {
                if (match(_records[index1]))
                {
                    if (intList == null)
                        intList = new List<int>();
                    intList.Add(index1);
                }
            }
            if (intList == null)
                return;
            List<T> removedRecords = null;
            for (int index2 = intList.Count - 1; index2 >= 0; --index2)
            {
                int index3 = intList[index2];
                T record = _records[index3];
                if (removedRecords != null && record != null)
                    removedRecords.Add(record);
                T obj;
                if (_recordsById.TryGetValue(record.ID, out obj))
                    _recordsById.Remove(obj.ID);
                _records.RemoveAt(index3);
            }
        }

        public override string ToString() => _name;
    }
}