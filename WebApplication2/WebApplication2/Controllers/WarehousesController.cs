using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;


namespace WebApplication2.Controllers;

[ApiController]
    [Route("controller-warehouses")]
public class WarehousesController: ControllerBase
{
    private readonly IConfiguration _configuration;
    

    public WarehousesController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
    
}