using System.Transactions;
using WebApplication2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;


namespace WebApplication2.Controllers;
[Route("api/[controller]")]
[ApiController]
public class WarehouseController: ControllerBase
{
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductWarehouse(Product_Warehouse productWarehouse)
    {
        if (!await _dbService.DoesProductExists(productWarehouse.IdProduct))
            return NotFound($"Product with given ID- {productWarehouse.IdProduct} doesn't exists");
        
        if (!await _dbService.DoesWarehouseExists(productWarehouse.IdWarehouse))
            return NotFound($"Product with given ID- {productWarehouse.IdWarehouse} doesn't exists");

        if (!(productWarehouse.Amount > 1))
            return BadRequest("Amount is not bigger than 0");
        
        if (!await _dbService.DoesOrderExistsAndWasCreatedBefore(productWarehouse.IdProduct,productWarehouse.Amount,productWarehouse.CreateAt))
            return NotFound($"Order with given ID and Amount- {productWarehouse.IdProduct},{productWarehouse.Amount} doesn't exists");
        
        if (!await _dbService.CzyNieZamowienieZrealizowane(productWarehouse.IdProduct,productWarehouse.Amount))
            return BadRequest($"Order in ProductWarehouse with given ID and Amount- {productWarehouse.IdProduct},{productWarehouse.Amount} already exists");

        await _dbService.AktulizacjaOrder(productWarehouse.IdProduct, productWarehouse.Amount);
        await _dbService.AddProduct_Warehouse(productWarehouse);
        return Created(Request.Path.Value ?? "api/Warehouse", productWarehouse);

    }
    
}