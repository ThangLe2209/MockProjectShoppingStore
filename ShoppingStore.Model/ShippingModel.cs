using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ShoppingStore.Model.Repository.Validation;

namespace ShoppingStore.Model
{
    public class ShippingModel : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }

    }
}
