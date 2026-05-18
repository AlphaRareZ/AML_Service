
using System.Text.Json.Serialization;

namespace Infrastructure.Models.MessageBus;

public class LigandResponseMessage
{
    [JsonPropertyName("success")] public bool Success { get; set; }

    [JsonPropertyName("protein_id")] public int ProteinId { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    
    //[JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
    //[JsonPropertyName("username")] public string Username { get; set; } = string.Empty;

    [JsonPropertyName("files_uploaded")] public int FilesUploaded { get; set; }

    [JsonPropertyName("files_failed")] public int FilesFailed { get; set; }

    // This dictionary automatically captures all the dynamic file names as Keys and their URLs as Values
    [JsonPropertyName("download_links")] public Dictionary<string, string> DownloadLinks { get; set; } = new();
}