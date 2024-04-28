using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using WebApplication2.Models;

namespace WebApplication2.Services;

public interface IDbService
{
    Task<Product_Warehouse> AddProduct_Warehouse(Product_Warehouse productWarehouse);
}


public class DbService(IConfiguration configuration) :IDbService
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    
    

    public async Task<Product_Warehouse> AddProduct_Warehouse(Product_Warehouse productWarehouse)
    {
        await using var connection = await GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {



            var command = new SqlCommand(
                @"select IdProduct, Price from Product where IdProduct = @1",
                connection
            );
            command.Parameters.AddWithValue("@1", productWarehouse.IdProduct);
            var reader1 = await command.ExecuteReaderAsync();

            if (!reader1.HasRows)
            {
                return null;
            }

            var price2 = reader1.GetInt32(1);

            var command1 = new SqlCommand(
                @"select IdWarehouse from Warehouse where IdWarehouse = @1",
                connection
            );
            command1.Parameters.AddWithValue("@1", productWarehouse.IdWarehouse);
            var reader2 = await command1.ExecuteReaderAsync();

            if (!reader2.HasRows)
            {
                return null;
            }

            if (productWarehouse.Amount > 0)
            {

            }
            else return null;


            var command2 = new SqlCommand(
                @"select CreatedAt, IdOrder from Order where IdProduct = @1 and Amount =@2",
                connection
            );
            command2.Parameters.AddWithValue("@1", productWarehouse.IdProduct);
            command2.Parameters.AddWithValue("@2", productWarehouse.Amount);
            var reader3 = await command2.ExecuteReaderAsync();

            if (!reader3.HasRows)
            {
                return null;
            }

            if (!await reader3.ReadAsync())
            {
                return null;
            }

            var idOrder = reader3.GetInt32(1);

            var data = reader3.GetDateTime(0);
            if (productWarehouse.CreateAt < data)
            {
                return null;
            }

            var command3 = new SqlCommand(
                @"select * from Product_Warehouse where IdOrder = @1 ",
                connection
            );
            command3.Parameters.AddWithValue("@1", productWarehouse.IdProduct);

            var reader4 = await command3.ExecuteReaderAsync();

            if (!reader4.HasRows)
            {
                return null;
            }

            var command4 = new SqlCommand(
                @"UPDATE Order SET FulfilledAt=@2 where IdOrder=@1",
                connection
            );
            command4.Parameters.AddWithValue("@1", productWarehouse.IdProduct);
            command4.Parameters.AddWithValue("@2", DateTime.Now);

            var command5 = new SqlCommand(
                @"select Price from Product where IdProduct = @1 ",
                connection
            );
            command5.Parameters.AddWithValue("@1", productWarehouse.IdProduct);

            var reader6 = await command5.ExecuteReaderAsync();

            if (!await reader6.ReadAsync())
            {
                return null;
            }

            var price = reader6.GetDouble(0);

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
            ins.Parameters.AddWithValue("@5", price2 * productWarehouse.Amount);
            ins.Parameters.AddWithValue("@6", DateTime.Now);

            var kluczgłowny = (int)(await ins.ExecuteScalarAsync())!;
            productWarehouse.IdProductWarehouse = kluczgłowny;
            return productWarehouse;
        }catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
}