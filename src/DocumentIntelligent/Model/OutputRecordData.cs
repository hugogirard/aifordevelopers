namespace DocumentIntelligent;

/// <summary>
/// Analyze document from Azure Form Recognizer
/// </summary>
public class OutputRecordData
{
    public string purchaseOrderNumber { get; set; }

    public string Merchant { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string DatedAs { get; set; }

    public string VendorName { get; set; }
    public string CompanyName { get; set; }

    public string CompanyAddress { get; set; }
    public string CompanyPhoneNumber { get; set; }

    public string Subtotal { get; set; }
    public string Tax { get; set; }

    public string Total { get; set; }

    public string Signature { get; set; }
}
