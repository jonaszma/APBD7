using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using WebApplication2.Models;

namespace WebApplication2.Services;


public class DbService(IConfiguration configuration) :IDbService
{
    private readonly IConfiguration _configuration;


    

    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    
    
    
    

    public async Task<int> AddProduct_Warehouse(Product_Warehouse productWarehouse)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        
    

            var command = new SqlCommand(
                @"select IdOrder from Order where IdProduct = @1 and Amount =@2",
                connection
            );
            command.Parameters.AddWithValue("@1", productWarehouse.IdProduct);
            command.Parameters.AddWithValue("@2", productWarehouse.Amount);
            await connection.OpenAsync();
            var reader = await command.ExecuteReaderAsync();
            var idOrder = reader.GetInt32(0);
            
            var command2 = new SqlCommand(
                @"select Price from Product where IdProduct = @1 ",
                connection
            );
            command2.Parameters.AddWithValue("@1", productWarehouse.IdProduct);
            
            await connection.OpenAsync();
            var reader2 = await command.ExecuteReaderAsync();
            var price = reader.GetInt32(0);

            try
            {

                var ins = new SqlCommand(
                    @"INSERT INTO Product_Warhouse values (@1,@2,@3,@4,@5,@6);
                            select cast(scope_indetity() as int)",
                    connection,
                    (SqlTransaction)transaction
                );
                ins.Parameters.AddWithValue("@2", productWarehouse.IdProduct);
                ins.Parameters.AddWithValue("@1", productWarehouse.IdWarehouse);
                ins.Parameters.AddWithValue("@3", idOrder);
                ins.Parameters.AddWithValue("@4", productWarehouse.Amount);
                ins.Parameters.AddWithValue("@5", price * productWarehouse.Amount);
                ins.Parameters.AddWithValue("@6", DateTime.Now);

                await connection.OpenAsync();
                var id = await ins.ExecuteScalarAsync();
                if (id is null) throw new Exception();
                return Convert.ToInt32(id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

    }

    public async Task<bool> DoesProductExists(int id)
    {
        await using var connection = await GetConnection();
        var command = new SqlCommand(
            @"select IdProduct, Price from Product where IdProduct = @1",
            connection
        );
        command.Parameters.AddWithValue("@1", id);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> DoesWarehouseExists(int id)
    {
        await using var connection = await GetConnection();
        var command1 = new SqlCommand(
            @"select IdWarehouse from Warehouse where IdWarehouse = @1",
            connection
        );
        command1.Parameters.AddWithValue("@1", id);
        await connection.OpenAsync();
        var reader = await command1.ExecuteReaderAsync();

        if (!reader.HasRows)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> DoesOrderExistsAndWasCreatedBefore(int id, int amount,DateTime time)
    {
        await using var connection = await GetConnection();
        var command = new SqlCommand(
            @"select IdOrder, CreatedAt from Order where IdProduct = @1 and Amount =@2",
            connection
        );
        command.Parameters.AddWithValue("@1", id);
        command.Parameters.AddWithValue("@2", amount);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        var data = reader.GetDateTime(1);
        if (!reader.HasRows)
        {
            return false;
        }
        if (data>time)
        {
            return false;
        }
        
        return true;
    }

    

    public async Task<bool> CzyNieZamowienieZrealizowane(int id,int amount)
    {
        await using var connection = await GetConnection();
        
        var command = new SqlCommand(
            @"select * from Product_Warehouse  
                        JOIN Order ON  Product_Warehouse.IdOrder = Order.IdOrder
                                                where Order.IdProduct = @1 and Order.Amount=@2 ",
            connection
        );
        command.Parameters.AddWithValue("@1", id);
        command.Parameters.AddWithValue("@2", amount);
        await connection.OpenAsync();
        var reader4 = await command.ExecuteReaderAsync();

        if (!reader4.HasRows)
        {
            return true;
        }

        return false;


    }

    public async Task AktulizacjaOrder(int id, int amount)
    {
        await using var connection = await GetConnection();
        
        var command = new SqlCommand(
            @"UPDATE Order SET FulfilledAt=@2 where IdProduct = @1 and Amount =@3",
            connection
        );
        command.Parameters.AddWithValue("@1", id);
        command.Parameters.AddWithValue("@2", DateTime.Now);
        command.Parameters.AddWithValue("@3", amount);
        await connection.OpenAsync();
        var reader4 = await command.ExecuteReaderAsync();
        
    }

    public async Task<Double> GetPrice(int id)
    {
        await using var connection = await GetConnection();
        
        
        var command = new SqlCommand(
            @"select Price from Product where IdProduct = @1",
            connection
        );
        command.Parameters.AddWithValue("@1", id);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        var price = reader.GetDouble(0);

        return price;
        
    }

    


    
}