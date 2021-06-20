using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
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
        [Required] public string Ticker { get; set; }

        [Required] public Market Market { get; set; }

        [Required] [ForeignKey("IdCompany")] public Company Company { get; set; }

        public int IdCompany { get; set; }

        [Required] public string Currency { get; set; }

        [Required] public string PrimaryExchange { get; set; }

        [Required] public bool IsActive { get; set; }

        [Required] public DateTime UpdatedAt { get; set; }

        [Required] public string Type { get; set; }

        public static Stock CreateByStockDetails(PolygonStockDetails s)
        {
            var stock = new Stock();
            stock.UpdateByStockDetails(s);
            return stock;
        }

        public void UpdateByStockDetails(PolygonStockDetails s)
        {
            Ticker = s.Symbol;
            PrimaryExchange = s.ExchangeSymbol;
            IsActive = s.Active;
            UpdatedAt = s.Updated == null
                ? DateTime.Now
                : DateTime.ParseExact(s.Updated, "MM/dd/yyyy", new CultureInfo("en-US"));
            Type = s.Type;

            if (Currency == null)
            {
                Currency = "usd";
            }
        }
    }
}