using System.Text;

namespace EZ.DataTool.TextTemplate
{
    public class Args
    {
        public DataTableContext Context;
    }

    public class TextTemplate
    {
        public string DefaultTemplate =
@"[MemoryPack.MemoryPackable]
public partial class @nameDataRecord : EZ.Data.DataRecord 
{
@fields

    public override void Deserialize(string[] fields)
    {
@deserialize
    }
}
";
        private readonly string _indent = "    ";

        private int indentLevel = 0;
        
        private void Indent(StringBuilder builder)
        {
            for (int i = 0; i < indentLevel; i++)
                builder.Append(_indent);
        }

        private void RemoveLastNewLine(StringBuilder builder)
        {
            if (builder != null && builder.Length > 0)
            {
                var newLine = System.Environment.NewLine;
                builder.Remove(builder.Length - newLine.Length, newLine.Length);
            }
        }

        private string GetFieldsBlock(StringBuilder builder, Args args)
        {
            builder.Clear();

            indentLevel = 1;
            for (int i = 0; i < args.Context.Columns.Count; i++)
            {
                var column = args.Context.Columns[i];
                string typeName = new ValueTypeName(column.ValueType);
                var columnName = column.Name;
                if (column.Key == KeyType.PrimaryKey)
                {
                    if (column.ValueType == ValueType.String)
                    {
                        Indent(builder);
                        builder.Append($"public {typeName} {columnName} ");
                        builder.AppendLine("{ get; private set; }");
                    }
                    continue;
                }
                
                Indent(builder);
                builder.Append($"public {typeName} {columnName} ");
                builder.AppendLine("{ get; private set; }");
            }

            RemoveLastNewLine(builder);
            return builder.ToString();
        }

        private string GetDeserializeBlock(StringBuilder builder, Args args)
        {
            builder.Clear();
            indentLevel = 2;
            for (int i = 0; i < args.Context.Columns.Count; i++)
            {
                var column = args.Context.Columns[i];
                Indent(builder);

                if (column.ValueType == ValueType.String)
                {
                    builder.AppendLine($"{column.Name} = fields[{i}];");

                    if (column.Key == KeyType.PrimaryKey)
                    {
                        Indent(builder);
                        builder.AppendLine($"ID = {column.Name}.GetHashCode();");
                    }
                }
                else
                {
                    var convertMethodName = GetConvertMethodName(column.ValueType);
                    if (column.Key == KeyType.PrimaryKey)
                        builder.AppendLine($"ID = SafeConvert.{convertMethodName}(fields[{i}]);");
                    else
                        builder.AppendLine($"{column.Name} = SafeConvert.{convertMethodName}(fields[{i}]);");
                }
            }

            RemoveLastNewLine(builder);
            return builder.ToString();
        }

        public string GetConvertMethodName(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return "ToBoolean";
                case ValueType.Byte: return "ToByte";
                case ValueType.Short: return "ToInt16";
                case ValueType.Int: return "ToInt32";
                case ValueType.Int64: return "ToInt64";
                case ValueType.Float: return "ToSingle";
                case ValueType.BoolArray: return "ToBoolArray";
                case ValueType.IntArray: return "ToInt32Array";
                case ValueType.FloatArray: return "ToSingleArray";
                case ValueType.StringArray: return "ToStringArray";
            }
            return string.Empty;
        }

        public string SetParameter(string text,  string paramName, string paramValue)
        {
            return text.Replace(paramName, paramValue);
        }

        public string TransformText(Args args)
        {
            var builder = new StringBuilder();
            var transformText = DefaultTemplate;
            transformText = SetParameter(transformText, "@name", args.Context.DataTable.TableName);
            transformText = SetParameter(transformText, "@fields", GetFieldsBlock(builder, args));
            transformText = SetParameter(transformText, "@deserialize", GetDeserializeBlock(builder, args));
            return transformText;
        }
    }
}