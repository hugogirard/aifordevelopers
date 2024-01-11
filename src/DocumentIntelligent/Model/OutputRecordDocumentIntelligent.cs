namespace DocumentIntelligent;

/// <summary>
/// Result returned to Form Recognizer
/// </summary>
public class OutputRecordDocumentIntelligent
{

    public string RecordId { get; set; }
    public OutputRecordDataDocumentIntelligent Data { get; set; }
    public List<OutputRecordMessage> Errors { get; set; }
    public List<OutputRecordMessage> Warnings { get; set; }
}