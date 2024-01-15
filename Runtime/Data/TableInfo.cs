using System.IO;

namespace EZ.DataTool
{
    public class TableInfo
    {
        /// <summary>
        /// 엑셀 파일 전체경로
        /// e.g) Drive://.../Project/Documents/FileName.xlsx
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// 테이블명 (Sheet Name)
        /// </summary>
        public string TableName { private set; get; }

        /// <summary>
        /// 파일명
        /// e.g) FileName.xlsx
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 네임스페이스 (Workbook Name)
        /// </summary>
        public string Namespace { get; private set; }

        public TableInfo(FileInfo fileInfo, string tableName)
        {
            FullName = fileInfo.FullName;
            TableName = tableName;
            FileName = fileInfo.Name;

            Namespace = Path.GetFileNameWithoutExtension(FullName);
        }

        public TableInfo(string filePath, string tableName)
            : this(new FileInfo(filePath), tableName)
        {

        }
    }
}
