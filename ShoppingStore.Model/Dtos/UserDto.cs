using ShoppingStore.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Model.Dtos
{
	public class UserDto
	{
		public Guid Id { get; set; }
		public string Subject { get; set; }

		public string? UserName { get; set; }

		public string? Password { get; set; }

		public bool Active { get; set; }

		public string? Email { get; set; }

        public string? SecurityCode { get; set; }

        public Guid UserRoleId { get; set; }
		public RoleDto? UserRole { get; set; }

        public ICollection<UserClaimDto> Claims { get; set; } = new List<UserClaimDto>();

    }
}
