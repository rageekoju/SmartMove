using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMove.Model
{
    [Table("transaction")]
    public class Transaction
    {
        [Key] // This annotation is optional, used if you're working with Entity Framework
        public int Id { get; set; }

        [Required(ErrorMessage = "Source is required.")]
        public string Source { get; set; } // For Credit transactions

        public string Category { get; set; } // For Debit transactions

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public double Amount { get; set; } // Common for both Credit and Debit

        public string Tags { get; set; } // Optional metadata tags

        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } // Transaction title or description

        public string Notes { get; set; } // Optional additional information

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; } = DateTime.Now; // Transaction date
    }

}
