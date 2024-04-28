namespace WebApplication2.Models;

public class Product_Warehouse
{


    public int  IdProductWarehouse { get; set; }
    
    public int IdProduct { get; set; }
    public int IdWarehouse { get; set; }
    public int  Amount { get; set; }
   
    public DateTime CreateAt { get; set; }
}