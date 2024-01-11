namespace DocumentIntelligent;

public class OutputRecordEmbedding
{
    public string RecordId { get; set; }
    public List<float> Data { get; set; } = new();
    public List<OutputRecordMessage> Errors { get; set; }
    public List<OutputRecordMessage> Warnings { get; set; }
}
