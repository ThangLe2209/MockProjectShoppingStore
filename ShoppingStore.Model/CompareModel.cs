using ShoppingStore.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class CompareModel: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel? Product { get; set; }
        public Guid UserId { get; set; } // Property string Subject in User table => not FK key
    }
}
