
namespace EZ.DataTool
{
    public static class Utils
    {
        public static bool IsNumeric(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int:
                case ValueType.Int64:
                case ValueType.Float:
                case ValueType.Byte:
                case ValueType.Short:
                    return true;
            }
            return false;
        }
    }
}