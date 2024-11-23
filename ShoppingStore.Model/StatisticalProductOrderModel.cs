using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class StatisticalProductOrderModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string UserEmail { get; set; }
        public string OrderCode { get; set; }

        //[ForeignKey("OrderCode")] // can not success because FK must be reference to Primary key (here can use because can not get OrderId when still create Order Details)
        //public OrderModel? Order { get; set; } // bên Order có thể có thêm navigation collection ICollection<StatisticalProductOrderModel> nếu muốn dùng Include

        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")] // depend on line 16 (search foreign key attr efcore6 - have 3 ways to use)
        public ProductModel? Product { get; set; } // bên Order có thể có thêm navigation collection ICollection<StatisticalProductOrderModel> nếu muốn dùng Include
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalRevenue { get; set; } // Product Price * Quantity
        public string? CouponCode { get; set; }
    }
}
