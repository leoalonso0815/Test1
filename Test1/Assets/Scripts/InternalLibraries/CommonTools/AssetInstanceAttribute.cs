using System;

[AttributeUsage(AttributeTargets.Class)]
public class AssetInstanceAttribute : Attribute
{
    public enum ESearchType
    {
        ByDirectoryPath,
        ByFilePath,
    }
    
    public string Path { get; }

    public ESearchType SearchType { get; }

    public AssetInstanceAttribute(string path, ESearchType searchType)
    {
        Path = path;
        SearchType = searchType;
    }
}