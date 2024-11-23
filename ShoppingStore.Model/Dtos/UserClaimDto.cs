using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model.Dtos
{
    public class UserClaimDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public Guid UserId { get; set; }
    }
}
