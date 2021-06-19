using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Stonks.Server.Data;
using Stonks.Shared.Models;

namespace Stonks.Server.Services
{
    public class DbService
    {
        private readonly ApplicationDbContext _context;

        public DbService(ApplicationDbContext context)
        {
            _context = context;
        }

        private NpgsqlConnection GetConnection()
        {
            return (NpgsqlConnection) _context.Database.GetDbConnection();
        }

        public async Task SyncStockPreviews(IEnumerable<PolygonStockPreview> stockPreviews)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            /*
        public bool Active { get; set; }
        public string CurrencyName { get; set; }
        public string LastUpdatedUtc { get; set; }
        public string Market { get; set; }
        public string Name { get; set; }
        public string PrimaryExchange { get; set; }
        public string Ticker { get; set; }
        public string Type { get; set; }
             */

            var sql = @"
                    INSERT INTO ""Companies"" 
                        (""Name"", ""Cik"") 
                    VALUES 
                        (@CompanyName, @Cik) 
                    ON CONFLICT (""Cik"") DO UPDATE SET ""Name"" = Excluded.""Name"";

            INSERT INTO ""Stocks""
                (""Ticker"", ""IdCompany"", ""Market"", ""Currency"", ""PrimaryExchange"", ""IsActive"", ""UpdatedAt"") 
                (SELECT @Ticker, ""IdCompany"", @Market, @Currency, @PrimaryExchange, @IsActive, @UpdatedAt FROM ""Companies"" WHERE ""Cik"" = @Cik)
            ON CONFLICT ON CONSTRAINT ""PK_Stocks"" DO UPDATE SET
            ""Ticker"" = @Ticker, ""Market"" = @Market, ""Currency"" = @Currency, ""PrimaryExchange"" = @PrimaryExchange, ""IsActive"" = @IsActive, ""UpdatedAt"" = @UpdatedAt
                ;
";
            await using var transaction = await conn.BeginTransactionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn) {Transaction = transaction};

            foreach (var stock in stockPreviews)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("CompanyName", stock.Name);
                cmd.Parameters.AddWithValue("Cik", stock.Cik);
                cmd.Parameters.AddWithValue("Ticker", stock.Ticker);
                cmd.Parameters.AddWithValue("Market", Enum.Parse(typeof(Market), stock.Market, true));
                cmd.Parameters.AddWithValue("Currency", stock.CurrencyName);
                cmd.Parameters.AddWithValue("PrimaryExchange", stock.PrimaryExchange);
                cmd.Parameters.AddWithValue("IsActive", stock.Active);
                cmd.Parameters.AddWithValue("UpdatedAt", DateTime.Parse(stock.LastUpdatedUtc));
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