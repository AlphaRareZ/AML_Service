namespace Application.UseCases.Models;

public class AnalysisMessage()
{
    public string AnalysisID { get; set; }
    public string expression_file_url { get; set; }
    public string mapping_file_url { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
}