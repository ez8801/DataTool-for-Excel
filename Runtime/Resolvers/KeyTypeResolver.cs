
namespace EZ.DataTool
{
    public class KeyTypeResolver
    {
        public static KeyType Resolve(string value)
        {
            if (false == string.IsNullOrEmpty(value))
            {
                string loweredValue = value.ToLower();
                if (loweredValue == "primarykey")
                {
                    return KeyType.PrimaryKey;
                }
                else if (loweredValue == "querykey")
                {
                    return KeyType.QueryKey;
                }
            }
            return KeyType.None;
        }
    }
}
