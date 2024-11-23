using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using ShoppingStore.Client.Controllers;
using ShoppingStore.Client.Models.ViewModels;
using ShoppingStore.Client.Repository;
using ShoppingStore.Client.Tests.Mock;
using ShoppingStore.Model.Dtos;
using ShoppingStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShoppingStore.Client.Tests.Controllers
{
    public class CartControllerTest
    {
        private Mock<ProductService> _mockProductService;
        private Mock<ShippingService> _mockShippingService;
        private Mock<CouponService> _mockCouponService;

        public CartControllerTest()
        {
            _mockProductService = new Mock<ProductService>();
            _mockShippingService = new Mock<ShippingService>();
            _mockCouponService = new Mock<CouponService>();
        }

        [Fact]
        public void Index()
        {
            var mockHttpContext = new Mock<HttpContext>();

            // Cookie mock
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.SetupGet(c => c["CouponTitle"]).Returns("VOUCHER50 | VOUCHER50 giảm 50 $ cho các mặt hàng cho phái đẹp ngày 20/10 ngày phụ nữ Việt Nam");
            cookiesMock.SetupGet(c => c["CouponName"]).Returns("VOUCHER50");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(cookiesMock.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            responseCookiesMock.Setup(c => c.Delete("CouponTitle")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("CouponName")).Verifiable();
            mockHttpContext.Setup(ctx => ctx.Response.Cookies)
                .Returns(responseCookiesMock.Object);

            // Session mock
            var sessionData = new List<CartItemModel>() {
                new CartItemModel(new ProductDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product1",
                    Price = 1233,
                }),
                new CartItemModel(new ProductDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product2",
                    Price = 1234,
                })
            };
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(sessionData);
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            // Controller mock
            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;


            // If need to user AppSetting data.
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //var configuration = builder.Build();

            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CartItemViewModel>(viewResult.Model);

            Assert.True(model.CartItems.Count == 2);
            Assert.Equal("/Cart", (string)viewResult.ViewData["Class"]); // check ViewBag (can't check directly must through ViewData)

            var arrOptions = Assert.IsAssignableFrom<SelectList>(viewResult.ViewData["ArrOptions"]); // array selectList quantity options
            var correctOptions = Assert.IsType<int[]>(arrOptions.Items);
        }

        [Fact]
        public void Checkout()
        {
            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            var result = controller.Checkout();
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Add()
        {
            var validGuid = new Guid("492937ed-56ae-4ad8-9f82-7c75a0362f44");
            _mockProductService.Setup(x => x.GetProductByIdAsync(validGuid))
                    .ReturnsAsync(new ProductDto()
                    {
                        Id = validGuid,
                        Name = "Test Product1",
                        Price = 1233,
                        Quantity = 10,
                    });

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>());
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var requestMock = new Mock<HttpRequest>();
            var headers = new Mock<IHeaderDictionary>();
            headers.Setup(x => x["Referer"]).Returns("https://localhost:7226");
            requestMock.Setup(r => r.Headers).Returns(headers.Object);
            mockHttpContext.Setup(s => s.Request).Returns(requestMock.Object);

            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Add(validGuid);
            var viewResult = Assert.IsType<RedirectResult>(result);
        }

        [Fact]
        public async Task Decrease()
        {
            var validGuid = new Guid("492937ed-56ae-4ad8-9f82-7c75a0362f44");
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = validGuid, ProductName = "Test Product1", Price = 1233, Quantity = 10},
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Decrease(validGuid);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            var res = System.Text.Encoding.Default.GetString(mockSession["Cart"] as byte[]);
            var sessionResult = JsonConvert.DeserializeObject<List<CartItemModel>>(res);
            var cartItemById = sessionResult.FirstOrDefault(s => s.ProductId == validGuid);
            Assert.Equal(9, cartItemById.Quantity);
        }

        [Fact]
        public async Task Increase()
        {
            var validGuid = new Guid("492937ed-56ae-4ad8-9f82-7c75a0362f44");

            _mockProductService.Setup(x => x.GetProductByIdAsync(validGuid))
                .ReturnsAsync(new ProductDto()
                {
                    Id = validGuid,
                    Name = "Test Product1",
                    Price = 1233,
                    Quantity = 20,
                });

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = validGuid, ProductName = "Test Product1", Price = 1233, Quantity = 10},
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Increase(validGuid);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            var res = System.Text.Encoding.Default.GetString(mockSession["Cart"] as byte[]);
            var sessionResult = JsonConvert.DeserializeObject<List<CartItemModel>>(res);
            var cartItemById = sessionResult.FirstOrDefault(s => s.ProductId == validGuid);
            Assert.Equal(11, cartItemById.Quantity);
        }


        [Fact]
        public async Task Remove()
        {
            var validGuid = new Guid("492937ed-56ae-4ad8-9f82-7c75a0362f44");

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = validGuid, ProductName = "Test Product1", Price = 1233, Quantity = 10},
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Remove(validGuid);

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            var res = System.Text.Encoding.Default.GetString(mockSession["Cart"] as byte[]);
            var sessionResult = JsonConvert.DeserializeObject<List<CartItemModel>>(res);
            var cartItemById = sessionResult.FirstOrDefault(s => s.ProductId == validGuid);
            Assert.Null(cartItemById);
        }

        [Fact]
        public async Task Clear()
        {

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product1", Price = 1233, Quantity = 10},
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Clear();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Throws<KeyNotFoundException>(() => mockSession["Cart"]);
        }

        [Fact]
        public async Task ChangeSelectOptions()
        {
            var validGuid = new Guid("492937ed-56ae-4ad8-9f82-7c75a0362f44");

            _mockProductService.Setup(x => x.GetProductByIdAsync(validGuid))
                .ReturnsAsync(new ProductDto()
                {
                    Id = validGuid,
                    Name = "Test Product1",
                    Price = 1233,
                    Quantity = 20,
                });

            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new MockHttpSession();
            mockSession["Cart"] = JsonConvert.SerializeObject(new List<CartItemModel>() {
                new CartItemModel {ProductId = validGuid, ProductName = "Test Product1", Price = 1233, Quantity = 1},
                new CartItemModel {ProductId = Guid.NewGuid(), ProductName = "Test Product2", Price = 1234, Quantity = 8}
            });

            mockHttpContext.Setup(s => s.Session).Returns(mockSession);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);
            controller.Url = new Mock<IUrlHelper>().Object;

            var result = await controller.ChangeSelectOptions(validGuid, 15);

            var viewResult = Assert.IsType<OkObjectResult>(result);

            var res = System.Text.Encoding.Default.GetString(mockSession["Cart"] as byte[]);
            var sessionResult = JsonConvert.DeserializeObject<List<CartItemModel>>(res);
            var cartItemById = sessionResult.FirstOrDefault(s => s.ProductId == validGuid);
            Assert.Equal(15, cartItemById.Quantity);
        }

        [Fact]
        public void RedirectPage()
        {
            var mockHttpContext = new Mock<HttpContext>();
            var claims = new List<Claim>
            {
                new Claim("UserId", Guid.NewGuid().ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = controller.RedirectPage("tinh1", "quan1", "huyen1");
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task GetShipping()
        {
            string city = "city1";
            string district = "district1";
            string ward = "ward1";

            _mockShippingService.Setup(x => x.GetShippingByGeoDataAsync(city,district,ward))
                .ReturnsAsync(new ShippingDto()
                {
                    Id = Guid.NewGuid(),
                    Price = 1233,
                    Ward = ward,
                    District = district,
                    City = city,
                });

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(new Mock<IResponseCookies>().Object);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await controller.GetShipping(city, district, ward);
            var viewResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void DeleteShipping()
        {
            var mockHttpContext = new Mock<HttpContext>();

            // Cookie mock
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.SetupGet(c => c["ShippingPrice"]).Returns("2.0");
            cookiesMock.SetupGet(c => c["ShippingCity"]).Returns("Tỉnh An Giang");
            cookiesMock.SetupGet(c => c["ShippingDistrict"]).Returns("Huyện An Phú");
            cookiesMock.SetupGet(c => c["ShippingWard"]).Returns("Thị trấn An Phú");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(cookiesMock.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            responseCookiesMock.Setup(c => c.Delete("ShippingPrice")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingCity")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingDistrict")).Verifiable();
            responseCookiesMock.Setup(c => c.Delete("ShippingWard")).Verifiable();
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(responseCookiesMock.Object);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = controller.DeleteShipping();

            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task GetCoupon()
        {
            var coupon_value = "VOUCHER50";
            decimal grandTotal = 100M;

            var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var tempCurrentDateExpired = new DateTime(2099, 11, 23, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            _mockCouponService.Setup(x => x.GetCouponValidByNameAsync(coupon_value))
                .ReturnsAsync(new CouponDto()
                {
                    Id = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"),
                    Name = "VOUCHER50",
                    Description = "VOUCHER50 giảm 50 $ cho các mặt hàng cho phái đẹp ngày 20/10 ngày phụ nữ Việt Nam",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 10,
                    Status = 1,
                });

            _mockCouponService.Setup(x => x.GetNewGrandTotalByCoupon(grandTotal, "VOUCHER50"))
                .ReturnsAsync(50M);

            var mockHttpContext = new Mock<HttpContext>();
            var cookiesMock = new Mock<IRequestCookieCollection>();

            //RequestCookie
            cookiesMock.SetupGet(c => c["ShippingPrice"]).Returns("2.0");
            mockHttpContext.Setup(ctx => ctx.Request.Cookies).Returns(cookiesMock.Object);

            // ResponseCookie(notDelete, just append) => need Mock Empty only
            mockHttpContext.Setup(ctx => ctx.Response.Cookies).Returns(new Mock<IResponseCookies>().Object);

            var controller = new CartController(_mockProductService.Object, _mockShippingService.Object, _mockCouponService.Object);
            controller.ControllerContext.HttpContext = mockHttpContext.Object;

            var result = await controller.GetCoupon(coupon_value, grandTotal);

            var viewResult = Assert.IsType<OkObjectResult>(result);
        }
    }
}
