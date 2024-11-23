using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingStore.Model.Dtos
{
	public class CompareForCreationDto
	{
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

    }
}
