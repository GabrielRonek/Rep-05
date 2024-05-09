using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Cw4.Repositories;
using Cw4.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Cw4.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost]
        public async Task<IActionResult> AddProduct(WarehouseProductDTO data)
        {
            if (!await _warehouseRepository.IsProductExsist(data.IdProduct))
            {
                return NotFound();
            }

            if (!await _warehouseRepository.IsWarehouseExsist(data.IdWarehouse))
            {
                return NotFound();
            }

            var idOrder = await _warehouseRepository.IsOrderExsist(data.IdProduct, data.Amount, data.CreatedAt);

            if (idOrder == -1)
            {
                return NotFound();
            }

            if(await _warehouseRepository.IsOrderFulfilled(idOrder))
            {
                return Conflict("Order is already FulFilled");
            }

            if (await _warehouseRepository.IsOrderInProductWarehouse(idOrder))
            {
                return Conflict("Order is already Complited");
            }

            await _warehouseRepository.UpdateDate(idOrder);

            var id = await _warehouseRepository.NewProductWarehouseRecord(data, idOrder);

            return Created("", id);
        }

        
    }
}
