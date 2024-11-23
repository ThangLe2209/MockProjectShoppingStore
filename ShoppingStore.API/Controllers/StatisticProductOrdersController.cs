using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.API.Services;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Model;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticProductOrdersController : Controller
    {
        private readonly IStatisticProductOrderRepository _statisticProductOrderRepository;
        private readonly IMapper _mapper;

        public StatisticProductOrdersController(IStatisticProductOrderRepository statisticProductOrderRepository, IMapper mapper)
        {
            _statisticProductOrderRepository = statisticProductOrderRepository ?? throw new ArgumentNullException(nameof(statisticProductOrderRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("getByOrderCode", Name = "GetStatisticalProductOrdersByOrderCode")]
        public async Task<IActionResult> GetStatisticalProductOrdersByOrderCode(string orderCode)
        {
            //var a = 10;
            var statisticProductOrders = await _statisticProductOrderRepository.GetStatisticalProductOrdersByOrderCodeAsync(orderCode);
            if (statisticProductOrders.Count() == 0)
            {
                return NotFound("StatisticProductOrders not found!");
            }
            //var b = 10;

            return Ok(_mapper.Map<IEnumerable<StatisticalProductOrderDto>>(statisticProductOrders));
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateOrderDetails([FromBody] IEnumerable<StatisticalProductOrderForCreationDto> statisticalProductOrderForCreations) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var finalStatisticalProductOrderForCreations = _mapper.Map<IEnumerable<StatisticalProductOrderModel>>(statisticalProductOrderForCreations);
            _statisticProductOrderRepository.AddStatisticalProductOrderForCreations(finalStatisticalProductOrderForCreations);
            //var a = 10;
            await _statisticProductOrderRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            //var b = 10;
            var createdStatisticalProductOrdersToReturn = _mapper.Map<IEnumerable<StatisticalProductOrderDto>>(finalStatisticalProductOrderForCreations);
            //return StatusCode(201);
            //var c = 10;
            return CreatedAtRoute("GetStatisticalProductOrdersByOrderCode", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
              new
              {
                  orderCode = createdStatisticalProductOrdersToReturn.First().OrderCode,
              } // value API Get line 24 need - Api get specific pointOfInterest
              , createdStatisticalProductOrdersToReturn); // final Data (include in response body)
        }
    }
}
