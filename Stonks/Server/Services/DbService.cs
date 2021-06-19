using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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

            var sql = @"INSERT INTO ""Stocks""
    (""Ticker"", ""IdCompany"", ""Market"", ""Currency"", ""PrimaryExchange"", ""Type"", ""IsActive"", ""UpdatedAt"") VALUES 
    (@Ticker, @IdCompany, @Market, @Currency, @PrimaryExchange, @Type, @IsActive, @UpdatedAt)
            ON CONFLICT ON CONSTRAINT ""PK_Stocks"" DO UPDATE SET
            ""Ticker"" = @Ticker, ""Market"" = @Market, ""Currency"" = @Currency, ""PrimaryExchange"" = @PrimaryExchange, ""Type"" = @Type, ""IsActive"" = @IsActive, ""UpdatedAt"" = @UpdatedAt;
";
            
            await using var transaction = await conn.BeginTransactionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn) {Transaction = transaction};

            foreach (var stock in stockPreviews)
            {
                var company = await FindCompanyByStockPreview(stock);

                if (company == null)
                {
                    cmd.CommandText = @"INSERT INTO ""Companies"" 
                        (""Name"", ""Cik"", ""Figi"") 
                    VALUES 
                        (@Name, @Cik, @Figi) RETURNING ""IdCompany"";";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("Name", stock.Name);
                    cmd.Parameters.AddWithValue("Cik", stock.Cik != null ? stock.Cik : DBNull.Value);
                    cmd.Parameters.AddWithValue("Figi", stock.CompositeFigi != null ? stock.CompositeFigi : DBNull.Value);
                    var idCompany = await cmd.ExecuteScalarAsync();
                    
                    Console.WriteLine(idCompany);
                    
                    cmd.CommandText = sql;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("IdCompany", idCompany);
                    cmd.Parameters.AddWithValue("Ticker", stock.Ticker);
                    cmd.Parameters.AddWithValue("Market", Enum.Parse(typeof(Market), stock.Market, true));
                    cmd.Parameters.AddWithValue("Currency", stock.CurrencyName);
                    cmd.Parameters.AddWithValue("PrimaryExchange", stock.PrimaryExchange);
                    cmd.Parameters.AddWithValue("Type", stock.Type);
                    cmd.Parameters.AddWithValue("IsActive", stock.Active);
                    cmd.Parameters.AddWithValue("UpdatedAt", DateTime.Parse(stock.LastUpdatedUtc));
                    await cmd.ExecuteNonQueryAsync();
                }
                else
                {
                    if (company.Name != stock.Name)
                    {
                        cmd.CommandText = @"UPDATE ""Companies"" SET ""Name"" = @Name WHERE ""IdCompany"" = @IdCompany";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("Name", stock.Name);
                        cmd.Parameters.AddWithValue("IdCompany", company.IdCompany);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    cmd.CommandText = sql;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("IdCompany", company.IdCompany);
                    cmd.Parameters.AddWithValue("Ticker", stock.Ticker);
                    cmd.Parameters.AddWithValue("Market", Enum.Parse(typeof(Market), stock.Market, true));
                    cmd.Parameters.AddWithValue("Currency", stock.CurrencyName);
                    cmd.Parameters.AddWithValue("PrimaryExchange", stock.PrimaryExchange);
                    cmd.Parameters.AddWithValue("Type", stock.Type);
                    cmd.Parameters.AddWithValue("IsActive", stock.Active);
                    cmd.Parameters.AddWithValue("UpdatedAt", DateTime.Parse(stock.LastUpdatedUtc));
                    await cmd.ExecuteNonQueryAsync();
                }
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

        private Task<Company> FindCompanyByStockPreview(PolygonStockPreview stock)
        {
            return _context.Companies.SingleOrDefaultAsync(c =>
                (c.Cik != null && c.Cik == stock.Cik) 
                || (c.Figi != null && c.Figi == stock.CompositeFigi)
                || c.Name == stock.Name);
        }

        public async Task<IEnumerable<PolygonStockPreview>> FindStocksByQuery(string query)
        {
            /*
             *         public bool Active { get; set; }
        public string CurrencyName { get; set; }
        public string LastUpdatedUtc { get; set; }
        public string Market { get; set; }
        public string Name { get; set; }
        public string Cik { get; set; }
        public string PrimaryExchange { get; set; }
        public string Ticker { get; set; }
        public string Type { get; set; }
             */

            return await _context.Stocks
                .Include(s => s.Company)
                .Select(s => new PolygonStockPreview
                {
                    Active = s.IsActive,
                    CurrencyName = s.Currency,
                    LastUpdatedUtc = s.UpdatedAt.ToString(CultureInfo.InvariantCulture),
                    Market = s.Market.ToString(),
                    Name = s.Company.Name,
                    Cik = s.Company.Cik,
                    PrimaryExchange = s.PrimaryExchange,
                    Ticker = s.Ticker,
                    Type = s.Type
                })
                .ToListAsync();
        }
    }
}