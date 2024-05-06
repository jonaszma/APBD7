using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IDbService
{
    Task<int> AddProduct_Warehouse(Product_Warehouse productWarehouse);
    Task<bool> DoesProductExists(int id);
    Task<bool> DoesWarehouseExists(int id);
    Task<bool> DoesOrderExistsAndWasCreatedBefore(int id,int amount,DateTime time);

    
    
    Task<bool> CzyNieZamowienieZrealizowane(int id,int amount);

    Task AktulizacjaOrder(int id, int amount);

    Task<Double> GetPrice(int id);
    

}