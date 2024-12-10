using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using static Azure.Core.HttpHeader;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponsController : Controller
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        const int maxCounponsPageSize = 4;
        public CouponsController(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("getNewGrandTotalByCoupon/{oldGrandTotal}/{couponName}")]
        public async Task<ActionResult<decimal>> getNewGrandTotalByCoupon(decimal oldGrandTotal, string couponName)
        {
            var couponEntity = await _couponRepository.GetCouponByName(couponName);
            if (couponEntity == null)
            {
                return NotFound("Coupon Not Found");
            }
            if (couponEntity.DiscountPercent != 0)
            {
                return Ok(oldGrandTotal - oldGrandTotal * couponEntity.DiscountPercent);
            }
            return Ok(oldGrandTotal - couponEntity.DiscountDecrease);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetCoupons()
        {
            var couponEntities = await _couponRepository.GetCouponsAsync();
            var result = _mapper.Map<IEnumerable<CouponDto>>(couponEntities);
            return Ok(result);
        }

        [HttpGet("{couponId}", Name = "GetCouponById")]
        public async Task<ActionResult<CouponDto>> GetCoupon(Guid couponId)
        {
            var coupon = await _couponRepository.GetCouponById(couponId);
            if (coupon == null)
            {
                return NotFound("Coupon Not Found");
            }

            return Ok(_mapper.Map<CouponDto>(coupon));
        }

        [HttpGet("GetCouponExistedByName")]
        public async Task<ActionResult<CouponDto>> GetCouponExistedByName(string couponName)
        {
            try
            {
                var coupon = await _couponRepository.GetCouponByName(couponName);
                if (coupon == null)
                {
                    return NotFound("Coupon Not Found");
                }

                return Ok(_mapper.Map<CouponDto>(coupon));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCouponValidByName")]
        public async Task<ActionResult<CouponDto>> GetCouponValidByName(string couponName)
        {
            try
            {
                var coupon = await _couponRepository.GetCouponValidByName(couponName);
                if (coupon == null)
                {
                    return NotFound("Coupon Not Found");
                }

                return Ok(_mapper.Map<CouponDto>(coupon));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CouponForCreationDto coupon) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var checkCoupon = await _couponRepository.GetCouponByName(coupon.Name);

            if (checkCoupon != null)
            {
                return BadRequest("Coupon already exist in database");
            }

            var finalCoupon = _mapper.Map<CouponModel>(coupon);

            _couponRepository.AddCoupon(finalCoupon);

            await _couponRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdCouponToReturn = _mapper.Map<CouponDto>(finalCoupon);
            return CreatedAtRoute("GetCouponById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    couponId = createdCouponToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdCouponToReturn); // final Data (include in response body)
        }

        [HttpPatch("{couponId}")]
        public async Task<ActionResult> PartiallyUpdateCouponAsync(Guid couponId,
            [FromBody] JsonPatchDocument<CouponForEditDto> patchDocument) //// Need to download nuget package: Microsoft.AspNetCore.JsonPatch
        {
            // https://stackoverflow.com/questions/70184265/http-client-patch-method-with-asp-net-core-2-1-bad-request
            // https://github.com/KevinDockx/JsonPatch
            var couponEntity = await _couponRepository.GetCouponById(couponId);
            if (couponEntity == null)
            {
                return NotFound("Coupon Not Exist!");
            }
            var couponToPatch = _mapper.Map<CouponForEditDto>(couponEntity);

            patchDocument.ApplyTo(couponToPatch, ModelState);
            // ApiController not automatic check this time
            if (!ModelState.IsValid) // check for input formatter - here is JsonPatchDocument => as long as document valid it will true => miss case validation required name if we choose opt remove in documentjsonpatch
            {
                return BadRequest(JsonConvert.SerializeObject(ModelState));
            }

            // also check pointOfInterestToPatch (dto) after check json patch document (is it still valid after patchDocument has been applied) => now not miss case remove required name, and some other validation case
            if (!TryValidateModel(couponToPatch))
            {
                return BadRequest(JsonConvert.SerializeObject(ModelState));
            }

            _mapper.Map(couponToPatch, couponEntity); //ovveride left - source(Models.PointOfInterestForUpdateDto) to right- dest(Entities.PointOfInterest)
            await _couponRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{couponId}")]
        public async Task<ActionResult> DeleteCoupon(Guid couponId)
        {
            CouponModel currentCoupon = await _couponRepository.GetCouponById(couponId);
            if (currentCoupon == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Coupon not existed");
            }

            _couponRepository.DeleteCoupon(currentCoupon);
            await _couponRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }

        [HttpPut("{couponId}")]
        public async Task<ActionResult> UpdateCoupon(Guid couponId, [FromBody] CouponForEditDto updatedCoupon)
        {
            CouponModel? currentCoupon = await _couponRepository.GetCouponById(couponId);
            if (currentCoupon == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Coupon not existed");
            }

            CouponModel? couponWithName = await _couponRepository.GetCouponByName(updatedCoupon.Name);
            if(couponWithName != null && couponWithName.Id != couponId)
            {
                return BadRequest("Coupon Name already existed");
            }

            _mapper.Map(updatedCoupon, currentCoupon); // source, dest => use mapper like this will override data from source to dest
            await _couponRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
