
namespace EZ.DataTool
{
    public class ColumnFilterResolver
    {
        public static ColumnFilter Resolve(string text)
        {
            if (System.Enum.TryParse<ColumnFilter>(text, true, out var filter))
                return filter;
            return ColumnFilter.None;
        }
    }
}
