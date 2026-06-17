using DataToolkit.Sample.Basic.Entities;

namespace DataToolkit.Sample.Advanced.Entities;

public class CustomerOrderDto
{
    public Customer Customer { get; set; } = null!;

    public Order Order { get; set; } = null!;
}