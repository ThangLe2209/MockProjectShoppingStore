using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingStore.API.Services;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;

namespace ShoppingStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController: Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IResponseCacheService _responseCacheService;

        public OrdersController(IOrderRepository orderRepository, IMapper mapper, IResponseCacheService responseCacheService)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _responseCacheService = responseCacheService ?? throw new ArgumentNullException(nameof(responseCacheService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orderEntities = await _orderRepository.GetOrdersAsync();
            var result = _mapper.Map<IEnumerable<OrderDto>>(orderEntities);
            return Ok(result);

        }

        [HttpGet("{orderId}", Name = "GetOrderById")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Order Not Found");
            }

            return Ok(_mapper.Map<OrderDto>(order));
        }

        //[Authorize]
        [HttpGet("getByUserEmail/{userEmail}")]
        public async Task<IActionResult> GetOrdersByUserEmail(string userEmail)
        {
            var orders = await _orderRepository.GetOrdersByUserEmailAsync(userEmail);
            return Ok(_mapper.Map<IEnumerable<OrderDto>>(orders));
        }

        [HttpGet("orderByOrderCode", Name = "GetOrderByOrderCode")]
        public async Task<IActionResult> GetOrderByOrderCode(string orderCode)
        {
            var order = await _orderRepository.GetOrderByOrderCodeAsync(orderCode);
            if (order == null)
            {
                return NotFound("Order Not Found");
            }

            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderForCreationDto order) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {

            var finalOrder = _mapper.Map<OrderModel>(order);
            _orderRepository.AddOrder(finalOrder);

            await _orderRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdOrderToReturn = _mapper.Map<OrderDto>(finalOrder);

            await _responseCacheService.RemoveCacheResponseAsync("/api/products");
            return CreatedAtRoute("GetOrderById", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
                new
                {
                    orderId = createdOrderToReturn.Id,
                } // value API Get line 24 need - Api get specific pointOfInterest
                , createdOrderToReturn); // final Data (include in response body)
        }

        [HttpPut]
        public async Task<ActionResult> UpdateOrder(dynamic order)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(order.ToString());
            string orderCode = data.orderCode;
            int statusChange = data.status;
            var currentOrder = await _orderRepository.GetOrderByOrderCodeAsync(orderCode);
            if (currentOrder == null)
            {
                return BadRequest("Order not existed!");
            }

            if (statusChange == 0)
            {
                await _orderRepository.AddStatisticalByOrder(currentOrder);
            }
            if(currentOrder.Status == 0 && statusChange != 0) await _orderRepository.RemoveStatisticalByOrder(currentOrder);

            currentOrder.Status = statusChange;
            await _orderRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{orderId}")]
        public async Task<ActionResult> DeleteOrder(Guid orderId)
        {
            OrderModel currentOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            if (currentOrder == null)
            {
                //_logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound("Order not existed");
            }

            await _orderRepository.DeleteOrderDetails(currentOrder.OrderCode);
            _orderRepository.DeleteOrder(currentOrder);
            await _orderRepository.SaveChangesAsync();

            //_mailService.Send("Point of interest deleted.",
            //    $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }

        [HttpGet("orderDetails", Name = "GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails(string orderCode)
        {
            var order = await _orderRepository.GetOrderDetailsByOrderCodeAsync(orderCode);
            if (order.Count() == 0)
            {
                return NotFound("User Not have order details!");
            }

            return Ok(_mapper.Map<IEnumerable<OrderDetailDto>>(order));
        }

        [HttpPost("orderDetails")]
        public async Task<IActionResult> CreateOrderDetails([FromBody] IEnumerable<OrderDetailForCreationDto> orderDetails) // because data create is complex type (PointOfInterestForCreationDto) so by using [ApiController] in line 8 system will automatically know this data from body instead we must specify it like this [FromBody] PointOfInterestForCreationDto pointOfInterest
        {
            var finalOrders = _mapper.Map<IEnumerable<OrderDetails>>(orderDetails);
            _orderRepository.AddOrderDetails(finalOrders);

            await _orderRepository.SaveChangesAsync(); // after this line execute we will have new Id, foregin key data for variable finalPointOfInterest which auto generated from database (can set breakpoint at line 75, 95, 99 to see) and also update to database
            var createdOrderDetailsToReturn = _mapper.Map<IEnumerable<OrderDetailDto>>(finalOrders);
            //return StatusCode(201);
            return CreatedAtRoute("GetOrderDetails", // Name of Api Get from line 55 - to set location header in postman when we successfully created - location header will be api get in line 24 Ex: view cap1 image in folder 04 
              new
              {
                  orderCode = createdOrderDetailsToReturn.First().OrderCode,
              } // value API Get line 24 need - Api get specific pointOfInterest
              , createdOrderDetailsToReturn); // final Data (include in response body)
        }
    }
}
public class TempModel()
{
    public string? Ordercode { get; set; }
    public int Status { get; set; }
}
