using ShoppingStore.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
    public class CompareWithProductAndUser
	{
        public CompareModel Compare { get; set; }

        public ProductModel Product { get; set; }

        public User User { get; set; }

    }
}
