using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Dtos
{
	public class CouponForEditDto
    {
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
        public int Status { get; set; } = 1;
    }
}
