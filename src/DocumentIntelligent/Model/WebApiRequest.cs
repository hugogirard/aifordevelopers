namespace DocumentIntelligent;

/// <summary>
/// Request payload body from Azure Cognitive Search can be more than one document
/// </summary>
public class WebApiRequest
{
    public List<InputRecord> Values { get; set; }
}