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

            // i dont like this part, especially because we have ON CONFLICT ON CONSTRAINT AK_Stocks_IdCompany_Ticker
            // which works nice and fast but.. It's hardcoded constraint name, if something will be changed in db
            // like rename of constraint or fields - this will no longer work

            var sql = @"INSERT INTO ""Stocks""
    (""Ticker"", ""IdCompany"", ""Market"", ""Currency"", ""PrimaryExchange"", ""Type"", ""IsActive"", ""UpdatedAt"") VALUES 
    (@Ticker, @IdCompany, @Market, @Currency, @PrimaryExchange, @Type, @IsActive, @UpdatedAt)
            ON CONFLICT ON CONSTRAINT ""AK_Stocks_IdCompany_Ticker"" DO UPDATE SET
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
                    cmd.Parameters.AddWithValue("Figi",
                        stock.CompositeFigi != null ? stock.CompositeFigi : DBNull.Value);
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

        private async Task<Company> FindCompanyByStockPreview(PolygonStockPreview stock)
        {
            return await FindCompany(stock.Cik, stock.CompositeFigi, stock.Name);
        }

        private async Task<Company> FindCompanyByStockDetails(PolygonStockDetails stock)
        {
            return await FindCompany(stock.Cik, stock.Figi, stock.Name);
        }

        private async Task<Company> FindCompany(string cik, string figi, string name)
        {
            return await _context.Companies.SingleOrDefaultAsync(c =>
                (c.Cik != null && c.Cik == cik)
                || (c.Figi != null && c.Figi == figi)
                || c.Name == name);
        }

        public async Task<IEnumerable<PolygonStockPreview>> FindStocksByQuery(string query)
        {
            var q = query.ToUpper();

            return await _context.Stocks
                .Include(s => s.Company)
                .Where(s => s.Ticker.Contains(q) || s.Company.Name.ToUpper().Contains(q))
                .OrderBy(s => s.Ticker)
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

        public async Task SyncStockDetails(PolygonStockDetails s)
        {
            var company = await FindCompanyByStockDetails(s);

            if (company != null)
            {
                company.UpdateByStockDetails(s);
                _context.Entry(company).State = EntityState.Modified;

                var stock = await _context.Stocks
                    .SingleOrDefaultAsync(model => model.Ticker == s.Symbol && model.IdCompany == company.IdCompany);
                if (stock != null)
                {
                    stock.UpdateByStockDetails(s);
                    _context.Entry(stock).State = EntityState.Modified;
                }
                else
                {
                    stock = Stock.CreateByStockDetails(s);
                    stock.Company = company;
                    await _context.Stocks.AddAsync(stock);
                }
            }
            else
            {
                company = Company.CreateByStockDetails(s);
                var stock = Stock.CreateByStockDetails(s);
                stock.Company = company;
                await _context.Companies.AddAsync(company);
                await _context.Stocks.AddAsync(stock);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PolygonStockDetails> FindStockByTicker(string ticker)
        {
            return await _context.Stocks
                .Include(s => s.Company)
                .Where(model => model.Ticker == ticker.ToUpper())
                .Select(s => PolygonStockDetails.CreateByStock(s))
                .FirstOrDefaultAsync();
        }

        public async Task SyncChart(PolygonChartResponse chart)
        {
            await using var conn = GetConnection();
            await conn.OpenAsync();

            var sql = @"INSERT INTO ""ChartOhlcItems""
                        (""IdStock"", ""V"", ""Vw"", ""O"", ""C"", ""H"", ""L"", ""Timestamp"", ""N"") 
                    SELECT ""IdStock"", @V, @Vw, @O, @C, @H, @L, @Timestamp, @N FROM ""Stocks""
                    WHERE ""Ticker"" = @Ticker ORDER BY ""IdStock"" LIMIT 1
                    ON CONFLICT DO NOTHING";

            await using var transaction = await conn.BeginTransactionAsync();
            await using var cmd = new NpgsqlCommand(sql, conn) {Transaction = transaction};

            foreach (var chartItem in chart.Results)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("Ticker", chart.Ticker.ToUpper());
                cmd.Parameters.AddWithValue("V", chartItem.V);
                cmd.Parameters.AddWithValue("Vw", chartItem.Vw);
                cmd.Parameters.AddWithValue("O", chartItem.O);
                cmd.Parameters.AddWithValue("C", chartItem.C);
                cmd.Parameters.AddWithValue("H", chartItem.H);
                cmd.Parameters.AddWithValue("L", chartItem.L);
                cmd.Parameters.AddWithValue("N", chartItem.N);
                cmd.Parameters.AddWithValue("Timestamp", DateTimeOffset.FromUnixTimeMilliseconds(chartItem.T));
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

        public async Task<ChartItem[]> GetChartItemsByTicker(string ticker, DateTime from, DateTime to)
        {
            return await _context.ChartOhlcItems
                .Include(i => i.Stock)
                .Where(i => i.Stock.Ticker == ticker && i.Timestamp >= from && i.Timestamp <= to)
                .Select(i => ChartItem.CreateByChartOhlcItem(i))
                .ToArrayAsync();
        }
    }
}