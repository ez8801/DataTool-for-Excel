
namespace EZ.DataTool
{
    /// <summary>
    /// 컬럼 속성 
    /// * PrimaryKey: 유일하게 구성 
    /// </summary>
    public enum KeyType : byte
    {
        None,
        PrimaryKey,
        QueryKey,
    }   
}