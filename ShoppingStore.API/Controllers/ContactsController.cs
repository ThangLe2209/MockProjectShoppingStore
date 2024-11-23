using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        const int maxContactPageSize = 4;
        public ContactsController(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts()
        {
            var contactEntities = await _contactRepository.GetContactsAsync();
            var result = _mapper.Map<IEnumerable<ContactDto>>(contactEntities);
            return Ok(result);
        }

        [HttpGet("{contactId}", Name = "GetContactById")]
        public async Task<IActionResult> GetContact(Guid contactId)
        {
            var contact = await _contactRepository.GetContactByIdAsync(contactId);
            if (contact == null)
            {
                return NotFound("Contact not existed");
            }

            return Ok(_mapper.Map<ContactDto>(contact));
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromForm] ContactForCreationDto contact) // get FromForm because mapping MultiplePartDatatype
        {
            //var a = 10;
            var checkContactName = await _contactRepository.CheckContactNameAsync(contact.Name);

            if (checkContactName)
            {
                return BadRequest("Contact name already exist in database");
            }

            if (contact.ImageUrl != null)
            {
                var checkContactImageUrl = await _contactRepository.CheckContactImageUrlAsync(contact.ImageUrl);
                if (checkContactImageUrl) return BadRequest("Contact Image Url already exist in database");
                contact.LogoImg = contact.ImageUrl;
            }
            else if (contact.ImageUpload != null)
            {
                var checkContactImageUpload = _contactRepository.CheckContactImageUpload(contact.ImageUpload);
                if (checkContactImageUpload) return BadRequest("Contact Image Upload already exist in database");
                contact.LogoImg = await _contactRepository.AddContactImageAsync(contact.ImageUpload);
            }


            var finalContact = _mapper.Map<ContactModel>(contact);
            _contactRepository.AddContact(finalContact);
            //var b = 10;
            await _contactRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            //var c = 20;
            var createdContactToReturn = _mapper.Map<ContactDto>(finalContact);
            //var a = 10;
            return CreatedAtRoute("GetContactById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    contactId = createdContactToReturn.Id,
                } // value API Get line 89 need - Api get specific product by id
                , createdContactToReturn); // final Data (include in response body)
        }

        [HttpPut("{contactId}")]
        public async Task<IActionResult> UpdateContact(Guid contactId, [FromForm] ContactForEditDto updatedContact)
        {
            try
            {
                ContactModel currentContact = await _contactRepository.GetContactByIdAsync(contactId);
                if (currentContact == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Contact not existed");
                }

                var checkContactName = await _contactRepository.CheckContactNameUpdateAsync(contactId, updatedContact.Name);

                if (checkContactName)
                {
                    return BadRequest("Contact name already exist in database");
                }

                if (updatedContact.ImageUrl != null)
                {
                    var checkContactImageUrl = await _contactRepository.CheckContactImageUrlAsync(updatedContact.ImageUrl);
                    if (checkContactImageUrl) return BadRequest("Contact Image Url already exist in database");
                    if (currentContact.LogoImg != "noimage.jpg") _contactRepository.DeleteOldContactImage(currentContact.LogoImg);
                    currentContact.LogoImg = updatedContact.ImageUrl;
                }
                else if (updatedContact.ImageUpload != null)
                {
                    var checkContactImageUpload = _contactRepository.CheckContactImageUpload(updatedContact.ImageUpload);
                    if (checkContactImageUpload) return BadRequest("Contact Image Upload already exist in database");
                    if (currentContact.LogoImg != "noimage.jpg") _contactRepository.DeleteOldContactImage(currentContact.LogoImg);
                    currentContact.LogoImg = await _contactRepository.AddContactImageAsync(updatedContact.ImageUpload);
                }

                //var a = 10;
                _mapper.Map(updatedContact, currentContact); // source, dest => use mapper like this will override data from source to dest
                //var b = 11;
                await _contactRepository.SaveChangesAsync();
                //var c = 20;
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{contactId}")]
        public async Task<ActionResult> DeleteContact(Guid contactId)
        {
            try
            {
                ContactModel currentContact = await _contactRepository.GetContactByIdAsync(contactId);
                if (currentContact == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Contact not existed");
                }

                var contactImagePath = currentContact.LogoImg;
                _contactRepository.DeleteContact(currentContact);
                await _contactRepository.SaveChangesAsync();
                if (contactImagePath != "noimage.jpg") _contactRepository.DeleteOldContactImage(contactImagePath);

                //_mailService.Send("Point of interest deleted.",
                //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
