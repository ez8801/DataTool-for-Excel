namespace EZ.DataTool
{
    /// <summary>
    /// 컬럼 종류 
    /// * Data : 서버, 클라이언트 모두 사용
    /// * ServerData : 서버에서만 사용
    /// * ClientOnly : 클라이언트에서만 사용(바이너리로 뽑는다.)
    /// </summary>
    [System.Flags]
    public enum ColumnFilter : byte
    {
        None = 0,
        ServerData = 1 << 0,
        ClientData = 1 << 1,
        Data = ServerData | ClientData,
    }
}