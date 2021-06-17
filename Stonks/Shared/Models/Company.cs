using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stonks.Shared.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        

        
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
