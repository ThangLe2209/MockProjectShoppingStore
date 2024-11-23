using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlidersController : Controller
    {
        private readonly ISliderRepository _sliderRepository;
        private readonly IMapper _mapper;
        const int maxSliderPageSize = 4;
        public SlidersController(ISliderRepository sliderRepository, IMapper mapper)
        {
            _sliderRepository = sliderRepository ?? throw new ArgumentNullException(nameof(sliderRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SliderDto>>> GetSliders()
        {
            var sliderEntities = await _sliderRepository.GetSlidersAsync();
            var result = _mapper.Map<IEnumerable<SliderDto>>(sliderEntities);
            return Ok(result);
        }

        [HttpGet("{sliderId}", Name = "GetSliderById")]
        public async Task<IActionResult> GetSlider(Guid sliderId)
        {
            var slider = await _sliderRepository.GetSliderByIdAsync(sliderId);
            if (slider == null)
            {
                return NotFound("Slider not existed");
            }

            return Ok(_mapper.Map<SliderDto>(slider));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSlider([FromForm] SliderForCreationDto slider) // get FromForm because mapping MultiplePartDatatype
        {
            //var a = 10;
            var checkSliderName = await _sliderRepository.CheckSliderNameAsync(slider.Name);

            if (checkSliderName)
            {
                return BadRequest("Slider name already exist in database");
            }

            if (slider.ImageUrl != null)
            {
                var checkSliderImageUrl = await _sliderRepository.CheckSliderImageUrlAsync(slider.ImageUrl);
                if (checkSliderImageUrl) return BadRequest("Slider Image Url already exist in database");
                slider.Image = slider.ImageUrl;
            }
            else if (slider.ImageUpload != null)
            {
                var checkSliderImageUpload = _sliderRepository.CheckSliderImageUpload(slider.ImageUpload);
                if (checkSliderImageUpload) return BadRequest("Slider Image Upload already exist in database");
                slider.Image = await _sliderRepository.AddSliderImageAsync(slider.ImageUpload);
            }


            var finalSilder = _mapper.Map<SliderModel>(slider);
            _sliderRepository.AddSlider(finalSilder);
            //var b = 10;
            await _sliderRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            //var c = 20;
            var createdSliderToReturn = _mapper.Map<SliderDto>(finalSilder);
            //var a = 10;
            return CreatedAtRoute("GetSliderById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    sliderId = createdSliderToReturn.Id,
                } // value API Get line 89 need - Api get specific product by id
                , createdSliderToReturn); // final Data (include in response body)
        }

        [HttpPut("{sliderId}")]
        public async Task<IActionResult> UpdateSlider(Guid sliderId, [FromForm] SliderForEditDto updatedSlider)
        {
            try
            {
                SliderModel currentSlider = await _sliderRepository.GetSliderByIdAsync(sliderId);
                if (currentSlider == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Slider not existed");
                }

                var checkSliderName = await _sliderRepository.CheckSliderNameUpdateAsync(sliderId, updatedSlider.Name);

                if (checkSliderName)
                {
                    return BadRequest("Slider name already exist in database");
                }

                if (updatedSlider.ImageUrl != null)
                {
                    var checkSliderImageUrl = await _sliderRepository.CheckSliderImageUrlAsync(updatedSlider.ImageUrl);
                    if (checkSliderImageUrl) return BadRequest("Slider Image Url already exist in database");
                    if (currentSlider.Image != "noimage.jpg") _sliderRepository.DeleteOldSliderImage(currentSlider.Image);
                    currentSlider.Image = updatedSlider.ImageUrl;
                }
                else if (updatedSlider.ImageUpload != null)
                {
                    var checkSliderImageUpload = _sliderRepository.CheckSliderImageUpload(updatedSlider.ImageUpload);
                    if (checkSliderImageUpload) return BadRequest("Slider Image Upload already exist in database");
                    if (currentSlider.Image != "noimage.jpg") _sliderRepository.DeleteOldSliderImage(currentSlider.Image);
                    currentSlider.Image = await _sliderRepository.AddSliderImageAsync(updatedSlider.ImageUpload);
                }

                //var a = 10;
                _mapper.Map(updatedSlider, currentSlider); // source, dest => use mapper like this will override data from source to dest
                //var b = 11;
                await _sliderRepository.SaveChangesAsync();
                //var c = 20;
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{sliderId}")]
        public async Task<ActionResult> DeleteSlider(Guid sliderId)
        {
            try
            {
                SliderModel currentSlider = await _sliderRepository.GetSliderByIdAsync(sliderId);
                var sliderImagePath = currentSlider.Image;
                if (currentSlider == null)
                {
                    //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound("Slider not existed");
                }

                _sliderRepository.DeleteSlider(currentSlider);
                await _sliderRepository.SaveChangesAsync();
                if (sliderImagePath != "noimage.jpg") _sliderRepository.DeleteOldSliderImage(sliderImagePath);

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
