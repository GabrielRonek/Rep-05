using Cw4.DTOs;
using System.Data.SqlClient;

namespace Cw4.Repositories

{
    public interface IWarehouseRepository
    {
        
    }
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IConfiguration _configuration;

        public WarehouseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    public interface WarehouseCheck
        {

        }
    }
}
