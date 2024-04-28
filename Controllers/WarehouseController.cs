using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Cw4.Repositories;
using Cw4.DTOs;

namespace Cw4.Controllers
{
    [Route("controller")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWarehouseRepository _warehouseRepository;

        public WarehouseController(IConfiguration configuration, IWarehouseRepository warehouseRepository)
        {
            _configuration = configuration;
            _warehouseRepository = warehouseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetWarehouse(WarehouseProductDTO data)
        {
            return Ok();
        }

        public
    }
}
