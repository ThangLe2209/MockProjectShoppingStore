using System.ComponentModel.DataAnnotations;

namespace Thang.IDP.Entities
{
    public class UserLogin : IConcurrencyAware
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(200)]
        [Required]
        public string Provider { get; set; } // provider like Facebook, AzureAD,...

        [MaxLength(200)]
        [Required]
        public string ProviderIdentityKey { get; set; } // id in that provider

        [Required]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [ConcurrencyCheck]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}
