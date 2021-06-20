using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Stonks.Shared.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCompany { get; set; }

        [Required] public string Name { get; set; }

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

        public string HqAddress { get; set; }

        public string HqState { get; set; }

        public string HqCountry { get; set; }

        public string Type { get; set; }

        public DateTime? Updated { get; set; }

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
        public static Company CreateByStockDetails(PolygonStockDetails s)
        {
            var company = new Company();
            company.UpdateByStockDetails(s);
            return company;
        }

        public void UpdateByStockDetails(PolygonStockDetails s)
        {
            Name = s.Name;
            Cik = s.Cik;
            Logo = s.Logo;
            Listdate = s.Listdate;
            Bloomberg = s.Bloomberg;
            Figi = s.Figi;
            Lei = s.Lei;
            Sic = s.Sic;
            Country = s.Country;
            Industry = s.Industry;
            Sector = s.Sector;
            Marketcap = s.Marketcap;
            Employees = s.Employees;
            Phone = s.Phone;
            Ceo = s.Ceo;
            Url = s.Url;
            Description = s.Description;
            HqAddress = s.HqAddress;
            HqState = s.HqState;
            HqCountry = s.HqCountry;
            Type = s.Type;
            Updated = s.Updated == null
                ? DateTime.Now
                : DateTime.ParseExact(s.Updated, "MM/dd/yyyy", new CultureInfo("en-US"));
            Tags = s.Tags;
            Similar = s.Similar;
        }
    }
}