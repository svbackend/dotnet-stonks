using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stonks.Shared.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCompany { get; set; }

        [Required]
        public string Name { get; set; }

        public string Cik { get; set; }
        
        public Uri Logo { get; set; }

        public DateTime? Listdate { get; set; }

        public string Bloomberg { get; set; }
        
        public string Figi { get; set; }

        public string Lei { get; set; }
        
        public long? Sic { get; set; }
        
        public string Country { get; set; }

        public string Industry { get; set; }

        public string Sector { get; set; }

        public long? Marketcap { get; set; }

        public long? Employees { get; set; }

        public string Phone { get; set; }

        public string Ceo { get; set; }

        public Uri Url { get; set; }

        public string Description { get; set; }

        public string Symbol { get; set; }

        public string ExchangeSymbol { get; set; }

        public string HqAddress { get; set; }

        public string HqState { get; set; }
        
        public string HqCountry { get; set; }
        
        public string Type { get; set; }
        
        public string Updated { get; set; }

        public string[] Tags { get; set; } = Array.Empty<string>();

        public string[] Similar { get; set; } = Array.Empty<string>();

        // "ticker":"APLE",
        // "name":"Apple Hospitality REIT, Inc.",
        // "market":"stocks",
        // "locale":"us",
        // "primary_exchange":"XNYS",
        // "type":"CS",
        // "active":true,
        // "currency_name":"usd",
        // "cik":"0001418121",
        // "composite_figi":"BBG006473QX9",
        // "share_class_figi":"BBG006473QY8",
        // "last_updated_utc":"2021-06-16T00:00:00Z"
    }
}
