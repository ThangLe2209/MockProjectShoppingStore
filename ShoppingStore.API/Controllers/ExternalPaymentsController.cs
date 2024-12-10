using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalPaymentsController : Controller
    {
        private readonly IExternalPaymentRepository _externalPaymentRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        const int maxExternalPaymentsPageSize = 4;
        public ExternalPaymentsController(IExternalPaymentRepository externalPaymentRepository, ICouponRepository couponRepository, IMapper mapper)
        {
            _externalPaymentRepository = externalPaymentRepository ?? throw new ArgumentNullException(nameof(externalPaymentRepository));
            _couponRepository = couponRepository ?? throw new ArgumentNullException(nameof(couponRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet("checkValidPaymentPrice")]
        public async Task<IActionResult> CheckValidPaymentPrice(decimal rawTotal, string? couponName, string? shippingPrice, string paymentAmount)
        {
            if (paymentAmount == "0")
            {
                return BadRequest("PaymentAmount not a number");
            }
            if (!String.IsNullOrEmpty(couponName))
            {
                //if (couponName == "VOUCHER50")
                //{
                //    var correctAmount = ((rawTotal / 2) + decimal.Parse(shippingPrice)) * 22000;
                //    return decimal.Parse(paymentAmount) == correctAmount ? Ok() : BadRequest("Wrong amount");
                //}
                var couponEntity = await _couponRepository.GetCouponByName(couponName);
                if (couponEntity == null)
                {
                    return NotFound("Coupon Not Found");
                }
                if (couponEntity.DiscountPercent != 0)
                {
                    var correctAmount = ((rawTotal - rawTotal * couponEntity.DiscountPercent) + decimal.Parse(shippingPrice)) * 22000;
                    return decimal.Parse(paymentAmount) == correctAmount ? Ok() : BadRequest("Wrong amount");
                }
                var correctAmountDecrease = ((rawTotal - couponEntity.DiscountDecrease) + decimal.Parse(shippingPrice)) * 22000;
                return decimal.Parse(paymentAmount) == correctAmountDecrease ? Ok() : BadRequest("Wrong amount");
            }

            var correctAmountNoCoupon = (rawTotal + decimal.Parse(shippingPrice)) * 22000;
            return decimal.Parse(paymentAmount) == correctAmountNoCoupon ? Ok() : BadRequest("Wrong amount");
        }

        [HttpGet("getVnpayByPaymentId/{paymentId}", Name = "GetVnpayByPaymentId")]
        public async Task<IActionResult> GetVnpayByPaymentId(string paymentId)
        {
            var vnpayItem = await _externalPaymentRepository.GetVnpayItemByPaymentId(paymentId);
            if (vnpayItem == null)
            {
                return NotFound("vnpayItem Not Found");
            }

            return Ok(_mapper.Map<VnpayDto>(vnpayItem));
        }

        [HttpGet("{Id}", Name = "GetVnpayItemById")]
        public async Task<IActionResult> GetVnpayItemById(Guid Id)
        {
            var vnpayItem = await _externalPaymentRepository.GetVnpayItemById(Id);
            if (vnpayItem == null)
            {
                return NotFound("vnpayItem Not Found");
            }

            return Ok(_mapper.Map<VnpayDto>(vnpayItem));
        }

        [HttpPost("addVnpay")]
        public async Task<IActionResult> CreateVnPay([FromBody] VnpayForCreationDto vnpay) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var vnpayItem = await _externalPaymentRepository.GetVnpayItemByPaymentId(vnpay.PaymentId);
            if (vnpayItem != null)
            {
                return BadRequest("vnpayItem already existed!");
            }

            var finalVnpay = _mapper.Map<VnpayModel>(vnpay);

            _externalPaymentRepository.AddVnpayItem(finalVnpay);

            await _externalPaymentRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdExternalPaymentToReturn = _mapper.Map<VnpayDto>(finalVnpay);
            return CreatedAtRoute("GetVnpayItemById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    Id = createdExternalPaymentToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdExternalPaymentToReturn); // final Data (include in response body)
        }
    }
}
