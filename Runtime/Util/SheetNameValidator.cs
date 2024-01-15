using System.Text.RegularExpressions;

namespace EZ.DataTool.Util
{
    public class SheetNameValidator
    {
        public bool IsValid(string text)
        {
            return Regex.IsMatch(text, @"^[a-zA-Z0-9.]+$");
        }
    }
}
