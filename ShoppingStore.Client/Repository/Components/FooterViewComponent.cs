using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.Client.Repository.Components
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly ContactService _contactService;

		public FooterViewComponent(ContactService contactService)
		{
			_contactService = contactService;
		}

        //[OutputCache(Duration = 2400)]
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var result = await _contactService.GetContactsAsync();
			return View(result.FirstOrDefault() ?? new ContactDto() {Name = "", Map = "", Email = "", Phone = "" });
		}
	}
}
