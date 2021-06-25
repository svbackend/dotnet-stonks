using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stonks.Shared.Models
{
    public class StockChartOhlcItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        
        [Required]
        [ForeignKey("IdStock")]
        public Stock Stock { get; set; }

        public int IdStock { get; set; }
        
        [Required]
        public long V { get; set; }

        [Required]
        public double Vw { get; set; }

        [Required]
        public double O { get; set; }

        [Required]
        public double C { get; set; }

        [Required]
        public double H { get; set; }

        [Required]
        public double L { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Timestamp { get; set; }

        [Required]
        public long N { get; set; }
    }
}