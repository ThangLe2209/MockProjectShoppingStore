using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class CouponModel: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Input coupon name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Input coupon description")]
        public required string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }

        //[Required(ErrorMessage = "Input coupon price")]
        //public required decimal Price { get; set; }

        [Required(ErrorMessage = "Input coupon quantity")]
        public required int Quantity { get; set; }
        public int Status { get; set; } // here is still Nullable = false when add-migration => if want this field or any field become Nullable mark it with the ? after the datatype (Ex. string?, int?).
    }
}
