namespace Presentation;

public class CreateLigandsRequestModel
{
    public Dictionary<int, string> ProteinAccessions { get; set; } = new Dictionary<int, string>();
}