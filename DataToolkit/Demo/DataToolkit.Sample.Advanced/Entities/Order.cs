namespace DataToolkit.Sample.Advanced.Entities;

public class Order
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }
}