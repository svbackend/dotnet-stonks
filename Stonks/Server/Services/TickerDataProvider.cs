using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Stonks.Server.Data;
using Stonks.Shared.Models;

namespace Stonks.Server.Services
{
    public class PolygonTickersResponse
    {
        public int Count { get; set; }
        public string NextUrl { get; set; }
        public List<PolygonStockPreview> Results { get; set; }
    }

    public class PolygonStockPreview
    {
        public bool Active { get; set; }
        public string CurrencyName { get; set; }
        public string LastUpdatedUtc { get; set; }
        public string Market { get; set; }
        public string Name { get; set; }
        public string Cik { get; set; }
        public string PrimaryExchange { get; set; }
        public string Ticker { get; set; }
        public string Type { get; set; }
        
        public string CompositeFigi { get; set; }
    }

    public class TickerDataProvider
    {
        private readonly PolygonHttpService _polygon;
        private readonly DbService _db;

        public TickerDataProvider(PolygonHttpService polygon, DbService db)
        {
            _polygon = polygon;
            _db = db;
        }

        public async Task<IEnumerable<PolygonStockPreview>> FindStocksByTickerOrCompany(string query)
        {
            var response = await _polygon.FindStocks(query);
            IEnumerable<PolygonStockPreview> stocks;

            if (response.Success)
            {
                stocks = response.Response.Results;
                await _db.SyncStockPreviews(stocks);
            }
            else
            {
                stocks = await _db.FindStocksByQuery(query);
                
            }

            return stocks;
        }

        public async Task<PolygonStockDetails> FindStockByTicker(string ticker)
        {
            var response = await _polygon.FindStockByTicker(ticker);

            if (!response.Success)
            {
                // todo try to find stocks in our db
            }
            // 1. Add api.polygon.io as baseAddress of httpClient
            // 2. Add ?apiKey=xxx to query or Authorization: Bearer XXX to headers
            
            return response.Response;
        }

        // Search by ticker or company name - https://api.polygon.io/v3/reference/tickers?search=Apple&active=true&sort=ticker&order=asc&limit=10

        // {
        //     "ticker":"AAPL",
        //     "name":"Apple Inc.",
        //     "market":"stocks",
        //     "locale":"us",
        //     "primary_exchange":"XNAS",
        //     "type":"CS",
        //     "active":true,
        //     "currency_name":"usd",
        //     "cik":"0000320193",
        //     "composite_figi":"BBG000B9XRY4",
        //     "share_class_figi":"BBG001S5N8V8",
        //     "last_updated_utc":"2021-06-16T00:00:00Z"
        // }

        // load more details about stock and company itself - https://api.polygon.io/v1/meta/symbols/TSLA/company

        // {
        //     "logo": "https://s3.polygon.io/logos/aapl/logo.png",
        //     "listdate": "1990-01-02T00:00:00.000Z",
        //     "cik": 320193,
        //     "bloomberg": "EQ0010169500001000",
        //     "figi": null,
        //     "lei": "HWUPKR0MPOU8FGXBT394",
        //     "sic": 3571,
        //     "country": "usa",
        //     "industry": "Computer Hardware",
        //     "sector": "Technology",
        //     "marketcap": 908316631180,
        //     "employees": 123000,
        //     "phone": "+1 408 996-1010",
        //     "ceo": "Timothy D. Cook",
        //     "url": "http://www.apple.com",
        //     "description": "Apple Inc is designs, manufactures and markets mobile communication and media devices and personal computers, and sells a variety of related software, services, accessories, networking solutions and third-party digital content and applications.",
        //     "exchange": "Nasdaq Global Select",
        //     "name": "Apple Inc.",
        //     "symbol": "AAPL",
        //     "exchangeSymbol": "NGS",
        //     "hq_address": "1 Infinite Loop Cupertino CA, 95014",
        //     "hq_state": "CA",
        //     "hq_country": "USA",
        //     "type": "CS",
        //     "updated": "11/16/2018",
        //     "tags": [
        //     "Technology",
        //     "Consumer Electronics",
        //     "Computer Hardware"
        //         ],
        //     "similar": [
        //     "MSFT",
        //     "NOK",
        //     "IBM",
        //     "HPQ",
        //     "GOOGL",
        //     "BB",
        //     "XLK"
        //         ],
        //     "active": true
        // }

        // OHLC by ticker & range - https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2020-06-01/2020-06-17

        // In order to provide as smooth UX as possible I think I need to do this:
        // When user searches for a company backend need to load list of results from polygon and
        // return it to frontend ASAP but it lacks some info (tags, category) in background (asynchronously) we need to load more details about all returned companies,
        // + cache response from polygon 
        // frontend will display received info even though some details will still be in "loading" state,
        // and instantly send request to backend to receive more details about displayed companies
        // backend will try to load records from polygon (should be cached result by this time)
        // but if no cached result or polygon doesn't respond - we need to search for company details in our db
    }
}