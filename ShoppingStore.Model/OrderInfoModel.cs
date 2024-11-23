using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class OrderInfoModel
    {
        public string FullName { get; set; } // ng đặt
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public double Amount { get; set; }
    }
}
