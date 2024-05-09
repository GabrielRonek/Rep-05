using Cw4.DTOs;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System.Windows.Input;

namespace Cw4.Repositories

{
    public interface IWarehouseRepository
    {
        public Task<bool> IsProductExsist(int id);
        public Task<bool> IsWarehouseExsist(int id);
        public Task<int> IsOrderExsist(int id, int Amount, DateTime createdAt);
        public Task<bool> IsOrderFulfilled(int id);
        public Task<bool> IsOrderInProductWarehouse(int idOrder);
        public Task UpdateDate(int idOrder);
        public Task<string> NewProductWarehouseRecord(WarehouseProductDTO warehouseProductDTO, int idOrder);
    }
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IConfiguration _configuration;

        public WarehouseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> IsProductExsist(int id)
        {

            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select IdProduct from Product where IdProduct=@1", conn);
                command.Parameters.AddWithValue("@1", id);
                await conn.OpenAsync();
                if (await command.ExecuteScalarAsync() is not null)
                {
                    return true;
                }
                return false;
            }


        }

        public async Task<int> IsOrderExsist(int id, int Amount, DateTime createdAt)
        {

            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select IdOrder from \"Order\" where IdProduct=@1 and amount=@2 and createdAt<@3", conn);
                command.Parameters.AddWithValue("@1", id);
                command.Parameters.AddWithValue("@2", Amount);
                command.Parameters.AddWithValue("@3", createdAt);
                await conn.OpenAsync();
                var res = await command.ExecuteScalarAsync();

                if (res is not null)
                {
                    return (int)res;
                }
                return -1;
            }


        }

        public async Task<bool> IsOrderFulfilled(int id)
        {

            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select IdOrder from \"Order\" where IdOrder=@1 and FulfilledAt IS NULL", conn);
                command.Parameters.AddWithValue("@1", id);
                await conn.OpenAsync();
                var res = await command.ExecuteScalarAsync();


                if (res is not null)
                {
                    return false;
                }

                return true;
            }
        }
        public async Task<bool> IsWarehouseExsist(int id)
        {

            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select IdWarehouse from Warehouse where IdWarehouse=@1", conn);
                command.Parameters.AddWithValue("@1", id);
                await conn.OpenAsync();
                if (await command.ExecuteScalarAsync() is not null)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> IsOrderInProductWarehouse(int idOrder)
        {

            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select IdProductWarehouse from Product_Warehouse where IdOrder=@1", conn);
                command.Parameters.AddWithValue("@1", idOrder);
                await conn.OpenAsync();
                if (await command.ExecuteScalarAsync() is not null)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task UpdateDate(int idOrder)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var actualDate = DateTime.Now;
                var command = new SqlCommand($"Update \"Order\" Set FulfilledAt=@2 where IdOrder=@1", conn);
                command.Parameters.AddWithValue("@1", idOrder);
                command.Parameters.AddWithValue("@2", actualDate);
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<string> NewProductWarehouseRecord(WarehouseProductDTO warehouseProductDTO, int idOrder)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var actualDate = DateTime.Now;
                var command = new SqlCommand($"Insert Into Product_Warehouse (IdWarehouse, IdProduct, idOrder, Amount, Price, CreatedAt) values (@1,@2,@3,@4,@5,@6); SELECT SCOPE_IDENTITY()", conn);
                decimal price = await GetProductPrice(warehouseProductDTO.IdProduct, warehouseProductDTO.Amount);
                command.Parameters.AddWithValue("@1", warehouseProductDTO.IdWarehouse);
                command.Parameters.AddWithValue("@2", warehouseProductDTO.IdProduct);
                command.Parameters.AddWithValue("@3", idOrder);
                command.Parameters.AddWithValue("@4", warehouseProductDTO.Amount);
                command.Parameters.AddWithValue("@5", price);
                command.Parameters.AddWithValue("@6", actualDate);
                await conn.OpenAsync();
                var res = await command.ExecuteScalarAsync();
                return Convert.ToString(res);
            }
        }

        public async Task<decimal> GetProductPrice(int idProduct, int amount)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("Default")))
            {
                var command = new SqlCommand($"Select Price from Product where IdProduct=@1", conn);
                command.Parameters.AddWithValue("@1", idProduct);
                await conn.OpenAsync();
                var res = await command.ExecuteScalarAsync();

                if (res is not null)
                {
                    return (decimal)res*amount;
                }

                return -1;
            }
        }


    }
}
