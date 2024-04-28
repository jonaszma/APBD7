namespace WebApplication2.Models;

public class Order
{
    public int IdOrder { get; set; }
    public int IdProduct { get; set; }
    public int Amount { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime FulfiledAt { get; set; }
}