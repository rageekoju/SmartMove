using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMove.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }

}
