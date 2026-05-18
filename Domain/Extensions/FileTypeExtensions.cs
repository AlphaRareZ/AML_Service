using Domain.Enums;

namespace Domain.Extensions;

public static class FileTypeExtensions
{
    public static FileType ToFileType(this string fileName)
    {
        var caseInsensitive = StringComparison.OrdinalIgnoreCase;
        if(fileName.EndsWith("png",caseInsensitive))return FileType.Png;
        if(fileName.EndsWith("csv",caseInsensitive))return FileType.Csv;
        if(fileName.EndsWith("json",caseInsensitive))return FileType.Json;
        if (fileName.EndsWith("pdb", caseInsensitive)) return FileType.Pdb;
        if (fileName.EndsWith("sdf", caseInsensitive)) return FileType.Sdf;
        return default;
    }
}