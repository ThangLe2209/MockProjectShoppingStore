using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Newtonsoft.Json;
using ShoppingStore.Client.Controllers;
using ShoppingStore.Client.Repository;
using ShoppingStore.Client.Services.Momo;
using ShoppingStore.Client.Services.Vnpay;
using ShoppingStore.Client.Tests.Mock;
using ShoppingStore.Controllers;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Client.Tests.Controllers
{
    public class CheckoutControllerTest
    {
        private Mock<IMomoService> _momoService;
        private Mock<IVnPayService> _vnPayService;
        private Mock<ExternalPaymentService> _externalPaymentService;
        private Mock<OrderService> _orderService;
        private Mock<OrderDetailService> _orderDetailService;
        private Mock<ProductService> _productService;
        private Mock<CouponService> _couponService;
        private Mock<StatisticProductOrderService> _statisticProductOrderService;

        public CheckoutControllerTest()
        {
            _momoService = new Mock<IMomoService>();
            _vnPayService = new Mock<IVnPayService>();
            _externalPaymentService = new Mock<ExternalPaymentService>();
            _orderService = new Mock<OrderService>();
            _orderDetailService = new Mock<OrderDetailService>();
            _productService = new Mock<ProductService>();
            _couponService = new Mock<CouponService>();
            _statisticProductOrderService = new Mock<StatisticProductOrderService>();
        }

        [Fact]
        public async Task Checkout()
        {
            var mockHttpContext = new Mock<HttpContext>();

            // Request Cookie mock
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.SetupGet(c => c["ShippingPrice"]).Returns("2.0");
            cookiesMock.SetupGet(c => c["ShippingCity"]).Returns("Tỉnh An Giang");
            cookiesMock.SetupGet(c => c["ShippingDistrict"]).Returns("Huyện An Phú");
            cookiesMock.SetupGet(c => c["ShippingWard"]).Returns("Thị trấn An Phú");
            cookiesMock.SetupGet(c => c["CouponName"]).Returns("VOUCHER50"); // test coupon can comment if not want
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(cookiesMock.Object);

            // Response Cookie mock
            var responseCookiesMock = new Mock<IResponseCookies>();
            responseCookiesMock.Setup(c => c.Delete("ShippingPrice")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingCity")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingDistrict")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingWard")).Verifiable();
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

            // session cart mock
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = new Guid("d5bbd837-d919-4911-abc4-3580600e25dd"), ProductName = "Test Product1", Price = 1233, Quantity = 1},
                new CartItemModel {ProductId = new Guid("ae44c3a7-4790-46ac-b790-a952cbdec33e"), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            //mock product service
            _productService.Setup(x => x.GetProductByIdAsync(new Guid("d5bbd837-d919-4911-abc4-3580600e25dd")))
                .ReturnsAsync(new ProductDto()
                {
                    Id = new Guid("d5bbd837-d919-4911-abc4-3580600e25dd"),
                    Name = "Test Product1",
                    Price = 1233,
                    Quantity = 1,
                });
            _productService.Setup(x => x.GetProductByIdAsync(new Guid("ae44c3a7-4790-46ac-b790-a952cbdec33e")))
                .ReturnsAsync(new ProductDto()
                {
                    Id = new Guid("ae44c3a7-4790-46ac-b790-a952cbdec33e"),
                    Name = "Test Product2",
                    Price = 1234,
                    Quantity = 8,
                });

            _productService.Setup(x => x.PartiallyUpdateProductsAsync(It.IsAny<IDictionary<Guid,string>>()))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            //mock order service
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new OrderDto() { }));
            _orderService.Setup(x => x.CreateOrderAsync(It.IsAny<OrderForCreationDto>()))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = content });

            //mock order detail service
            _orderDetailService.Setup(x => x.CreateOrderDetailsAsync(It.IsAny<List<OrderDetailForCreationDto>>()))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            // mock StatisticOrderProduct Create service - test table db only(to join get all data)
            _statisticProductOrderService.Setup(x => x.CreateStatisticalProductOrdersAsync(It.IsAny<List<StatisticalProductOrderForCreationDto>>()))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            // mock coupon service (can comment if not want test coupon service)
            var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var tempCurrentDateExpired = new DateTime(2024, 11, 23, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            HttpContent couponContent = new StringContent(JsonConvert.SerializeObject(new CouponDto()
            {
                Id = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"),
                Name = "VOUCHER50",
                Description = "Giảm 50% cho tổng giá trị đơn hàng mua ngày 20.10",
                DateStart = tempCurrentDate,
                DateExpired = tempCurrentDateExpired,
                Quantity = 1,
                Status = 1,
                CreatedDate = tempCurrentDate,
                UpdatedDate = tempCurrentDate,
            }));
            _couponService.Setup(x => x.GetCouponValidByNameWithErrorMessageAsync("VOUCHER50"))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = couponContent });

            _couponService.Setup(x => x.PartiallyUpdateCouponAsync(It.IsAny<Guid>(), new object { }))
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            // authenticate mock
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            var claims = new List<Claim>
            {
                new Claim("UserId", Guid.NewGuid().ToString()),
                new Claim("email", "test@gmail.com"),
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

            // call controller
            var controller = new CheckoutController(_orderService.Object, _orderDetailService.Object, _productService.Object,
                _couponService.Object,_statisticProductOrderService.Object, _momoService.Object, _vnPayService.Object, _externalPaymentService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Checkout("", "");
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
