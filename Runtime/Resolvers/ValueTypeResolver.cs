namespace EZ.DataTool
{
    public struct ValueTypeName
    {
        public ValueType ValueType { get; private set; }

        public ValueTypeName(ValueType valueType)
        {
            ValueType = valueType;
        }

        public static implicit operator string(ValueTypeName valueTypeName)
        {
            switch (valueTypeName.ValueType)
            {
                case ValueType.Bool: return "bool";
                case ValueType.Byte: return "byte";
                case ValueType.Short: return "short";
                case ValueType.Int: return "int";
                case ValueType.Float: return "float";
                case ValueType.Int64: return "long";
                case ValueType.String: return "string";
                case ValueType.IntArray: return "int[]";
                case ValueType.FloatArray: return "float[]";
                case ValueType.StringArray: return "string[]";
                case ValueType.BoolArray: return "bool[]";
            }
            return string.Empty;
        }
    }

    public class ValueTypeResolver
    {
        public static ValueType Resolve(string value)
        {
            if (string.IsNullOrEmpty(value))
                return ValueType.None;

            string loweredValue = value.ToLower();
            var valueType = ValueType.None;
            switch (loweredValue)
            {
                case "bool":
                    valueType = ValueType.Bool;
                    break;
                case "byte":
                    valueType = ValueType.Byte;
                    break;
                case "short":
                case "int16":
                    valueType = ValueType.Short;
                    break;
                case "int":
                case "int32":
                    valueType = ValueType.Int;
                    break;
                case "float":
                case "single":
                    valueType = ValueType.Float;
                    break;
                case "long":
                case "int64":
                    valueType = ValueType.Int64;
                    break;
                case "string":
                    valueType = ValueType.String;
                    break;
                case "bool[]":
                    valueType = ValueType.BoolArray;
                    break;
                case "int[]":
                case "int32[]":
                    valueType = ValueType.IntArray;
                    break;
                case "float[]":
                case "single[]":
                    valueType = ValueType.FloatArray;
                    break;
                case "string[]":
                    valueType = ValueType.StringArray;
                    break;
                default:
                    if (loweredValue.Contains("[]"))
                        valueType = ValueType.IntArray;
                    else
                        valueType = ValueType.Int;
                    break;
            }
            return valueType;
        }
    }
}