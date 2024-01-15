
namespace EZ.DataTool
{
    public class DbfColumn
    {
        public KeyType Key { get; set; }

        public string Name { get; set; }

        public ColumnFilter Filter { get; set; }

        public ValueType ValueType { get; set; }

        public string ValueTypeName { get; set; }

        // Starts at 1
        public int Ordinal { get; private set; } = -1;

        public DbfColumn()
        {
            
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetFilter(ColumnFilter filter)
        {
            Filter = filter;
        }

        public void SetKey(KeyType key)
        {
            Key = key;
        }

        public void SetDataType(string value)
        {
            ValueType = ValueTypeResolver.Resolve(value);
            ValueTypeName = value;
        }

        public void SetOrdinal(int ordinal)
        {
            Ordinal = ordinal;
        }

        public bool CheckUnique()
        {
            return false;
        }

        public bool IsValid()
        {
            return ValueType != ValueType.None;
        }

        public bool Contains()
        {
            return (Filter & ColumnFilter.ClientData) == ColumnFilter.ClientData;
        }
    }
}