using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Stonks.Server.Services
{
    public class DbService
    {
        private readonly IConfiguration _config;
        
        public DbService(IConfiguration config)
        {
            _config = config;
        }
        
        private SqlConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        }

        public async Task SyncStockPreviews(IEnumerable<PolygonStockPreview> stockPreviews)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            var sql = @"INSERT INTO Stocks (...) VALUES (@) ON CONFLICT DO UPDATE;";
            await using var transaction = await conn.BeginTransactionAsync();
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Transaction = (SqlTransaction)transaction;
            
            foreach (var stock in stockPreviews)
            {
                cmd.Parameters.Clear();
                // cmd.Parameters.AddWithValue("IdOrder", idOrder);
                // cmd.Parameters.AddWithValue("CreatedAt", request.CreatedAt);
                await cmd.ExecuteNonQueryAsync();
            }

            try
            {
                await transaction.CommitAsync();
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}