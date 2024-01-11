namespace DocumentIntelligent;

/// <summary>
/// Request payload body from Azure Cognitive Search can be more than one document
/// </summary>
public class WebApiRequestDocumentIntelligent
{
    public List<InputRecordDocumentIntelligent> Values { get; set; }
}
