namespace InvoiceApi.Models;

public class InvoiceItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class InvoiceRequest
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public List<InvoiceItem> Items { get; set; } = new();
}
