using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model
{
    public class StatisticalModel: BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public int Sold { get; set; }

        public int Revenue { get; set; }

        public int Profit { get; set; }
    }
}
