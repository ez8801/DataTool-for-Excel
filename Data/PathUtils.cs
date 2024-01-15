namespace EZ.Data
{
    public class PathUtils
    {
        public static string GetTsvPath(string name) =>
            $"UnimportedAssets/DataAsset/{name}.tsv";

        public static string GetDataAssetPath(string name) =>
            $"Data/DataAsset/{name}.dbf";

        public static string GetDataAssetAddress(string name) =>
            $"DataAsset/{name}.dbf";

        public static string GetDataRecordScriptPath(string @namespace, string tableName) =>
            $"Scripts/Data/Runtime/{@namespace}/{tableName}DataRecord.cs";
    }
}