using System.ComponentModel.DataAnnotations;

namespace ShoppingStore.Model.Entities
{
    public class UserRole : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }


        [MaxLength(250)]
        [Required]
        public string Value { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
