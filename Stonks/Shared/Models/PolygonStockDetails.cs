using System;
using System.Text.Json.Serialization;

namespace Stonks.Shared.Models
{
    public class PolygonStockDetails
    {
        public Uri Logo { get; set; }

        public DateTime Listdate { get; set; }

        public string Cik { get; set; }

        public string Bloomberg { get; set; }
        
        public string Figi { get; set; }

        public string Lei { get; set; }
        
        public long Sic { get; set; }
        
        public string Country { get; set; }

        public string Industry { get; set; }

        public string Sector { get; set; }

        public long Marketcap { get; set; }

        public long Employees { get; set; }

        public string Phone { get; set; }

        public string Ceo { get; set; }

        public Uri Url { get; set; }

        public string Description { get; set; }

        public string Exchange { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        // JsonPropertyName is required here because polygon returns response in snake_case but this specific field
        // returned in camelCase. idk why 0_o
        [JsonPropertyName("exchangeSymbol")]
        public string ExchangeSymbol { get; set; }

        public string HqAddress { get; set; }

        public string HqState { get; set; }
        
        public string HqCountry { get; set; }
        
        public string Type { get; set; }
        
        public string Updated { get; set; }
        
        public string[] Tags { get; set; }

        public string[] Similar { get; set; }
        
        public bool Active { get; set; }

        public static PolygonStockDetails CreateByStock(Stock s)
        {
            return new()
            {
                Name = s.Company.Name,
                Cik = s.Company.Cik,
                Logo = s.Company.Logo,
                Listdate = s.Company.Listdate ?? DateTime.Now,
                Bloomberg = s.Company.Bloomberg,
                Figi = s.Company.Figi,
                Lei = s.Company.Lei,
                Sic = s.Company.Sic ?? 0,
                Country = s.Company.Country,
                Industry = s.Company.Industry,
                Sector = s.Company.Sector,
                Marketcap = s.Company.Marketcap ?? 0,
                Employees = s.Company.Employees ?? 0,
                Phone = s.Company.Phone,
                Ceo = s.Company.Ceo,
                Url = s.Company.Url,
                Description = s.Company.Description,
                HqAddress = s.Company.HqAddress,
                HqState = s.Company.HqState,
                HqCountry = s.Company.HqCountry,
                Type = s.Type,
                Updated = s.Company.Updated.ToString(),
                Exchange = s.PrimaryExchange,
                Tags = s.Company.Tags,
                Similar = s.Company.Similar
            };
        }
    }
}