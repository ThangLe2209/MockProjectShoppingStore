using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class ProductQuantityModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Input product quantity")]
        public required int Quantity { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }

    }
}
