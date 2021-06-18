using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Stonks.Shared.Models
{
    public enum Market
    {
        Stocks = 0,
        Crypto = 1,
        Fx = 2
    }

    public class Stock
    {
        [Key]
        public string Id { get; set; }
        
        [Required]
        public string Ticker { get; set; }
        
        [Required]
        public Market Market { get; set; }
        
        [Required]
        [ForeignKey("IdCompany")]
        public Company Company { get; set; }
        
        public int IdCompany { get; set; }
        
        [Required]
        public string Currency { get; set; }
        
        [Required]
        public string PrimaryExchange { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
        
        [Required]
        public DateTime UpdatedAt { get; set; }
        
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
