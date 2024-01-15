using System;
using System.IO;
using System.Linq;

namespace EZ.DataTool
{
    public class FileUtils
    {
        public static string[] SupportedExtensions =
        {
            ".xlsx",
            ".xlsm",
            ".xlsb"
        };

        public static bool IsSupportedExtension(string extension)
        {
            for (int i = 0; i < SupportedExtensions.Length; ++i)
            {
                string supportedExtension = SupportedExtensions[i];
                if (string.Compare(extension, supportedExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists(string filePath, params string[] extenions)
        {
            if (!File.Exists(filePath))
            {
                var dir = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var pathWithoutExts = Path.Combine(dir, fileName);

                if (extenions != null)
                {
                    var exts = extenions.Where(x => !string.IsNullOrEmpty(x));
                    foreach (var ext in exts)
                    {
                        string fullPath = string.Empty;
                        if (ext.StartsWith('.'))
                        {
                            fullPath = Path.Combine(pathWithoutExts, ext);
                        }
                        else
                            fullPath = Path.Combine(pathWithoutExts, ".", ext);

                        if (File.Exists(fullPath))
                            return true;
                    }
                }

                return false;
            }
            return true;
        }
    }
}