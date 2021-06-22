using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Stonks.Shared.Models
{
    public class UserStock
    {
        [Required]
        [ForeignKey("IdUser")]
        public ApplicationUser User { get; set; }

        public Guid IdUser { get; set; }
        
        [Required]
        [ForeignKey("IdStock")]
        public ApplicationUser Stock { get; set; } // todo add IdStock

        public int IdStock { get; set; }
    }
}