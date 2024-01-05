namespace DocumentIntelligent;

/// <summary>
/// Information coming from Azure Cognitive Search
/// </summary>
public class InputRecord 
{
    public string RecordId { get; set; }
    public InputRecordData Data { get; set; }
}
