namespace DocumentIntelligent;

/// <summary>
/// Information coming from Azure Cognitive Search
/// </summary>
public class InputRecordDocumentIntelligent 
{
    public string RecordId { get; set; }
    public InputRecordDataDocumentIntelligent Data { get; set; }
}
