namespace DocumentIntelligent;

/// <summary>
/// Result returned to Form Recognizer
/// </summary>
public class OutputRecord
{

    public string RecordId { get; set; }
    public OutputRecordData Data { get; set; }
    public List<OutputRecordMessage> Errors { get; set; }
    public List<OutputRecordMessage> Warnings { get; set; }
}