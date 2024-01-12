namespace DocumentIntelligent;

/// <summary>
/// Analyze document from Azure Form Recognizer
/// </summary>
public class OutputRecordDataDocumentIntelligent
{
    public string id { get; set; } = Guid.NewGuid().ToString();

    public string purchaseOrderNumber { get; set; }

    public string Merchant { get; set; }

    public string Website { get; set; }
    public string Email { get; set; }
    public string DatedAs { get; set; }

    public string ShippedToVendorName { get; set; }
    public string ShippedToCompanyName { get; set; }

    public string ShippedToCompanyAddress { get; set; }
    public string ShippedToCompanyPhoneNumber { get; set; }

    public string ShippedFromCompanyName { get; set; }

    public string ShippedFromCompanyAddress { get; set; }
    public string ShippedFromCompanyPhoneNumber { get; set; }

    public string ShippedFromName { get; set; }

    public List<ItemPurchased> ItemsPurchased { get; set; } = new();

    public double Subtotal { get; set; }
    public double Tax { get; set; }

    public double Total { get; set; }

    public string Signature { get; set; }
}


public class  ItemPurchased
{
    public string Detail { get; set; }

    public double Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Total { get; set; }
}