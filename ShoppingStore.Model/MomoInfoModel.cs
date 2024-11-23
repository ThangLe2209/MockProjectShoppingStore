using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class MomoInfoModel
    {
        [Key]
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string FullName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DatePaid { get; set; }
    }
}
