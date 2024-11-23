using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShoppingStore.API.Controllers;
using ShoppingStore.API.DbContexts;
using ShoppingStore.API.Profiles;
using ShoppingStore.API.Services;
using ShoppingStore.API.Tests.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.API.Tests.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")] //hidden class from xUnit
    internal class CouponsControllerTest // class nay refactor lai nha chi dung inmemorydb thoi ko can dung vo mockData luon
    {
        private ShoppingStoreContext _context;
        private readonly CouponsController _couponsController;

        public CouponsControllerTest()
        {
            Mock<ICouponRepository> mockRepo = new Mock<ICouponRepository>(); // mock here to simulate Get Action

            var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var tempCurrentDateExpired = new DateTime(2024, 11, 20, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var coupons = new List<CouponModel>(){
                new CouponModel()
                {
                    Id = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"),
                    Name = "VOUCHER50",
                    Description = "Giảm 50% cho tổng giá trị đơn hàng mua ngày 20.10",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 1,
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate,
                },
                new CouponModel()
                {
                    Id = new Guid("ae49e41e-a76a-41c6-8a9a-08dcf7f18ab2"),
                    Name = "VOUCHER2/9",
                    Description = "VOUCHER2/9 giảm giá 100 $ cho các mặt hàng lưu niệm ngày Quốc Khánh 2 tháng 9",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 2,
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate,
                },
            };

            mockRepo.Setup(m => m.GetCouponsAsync()).ReturnsAsync(value: coupons);

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CouponProfile());
            });
            var mapper = mockMapper.CreateMapper();

            _couponsController = new CouponsController(mockRepo.Object, mapper);
        }

        public CouponsController MockDB() // use when add, update, remove to simulate real DB
        {
            _context = ContextFactory.Create();
            _context.Database.EnsureCreated();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CouponProfile());
            });
            var mapper = mockMapper.CreateMapper();

            var couponRepository = new CouponRepository(_context);
            return new CouponsController(couponRepository, mapper);
        }

        [Fact]
        public async Task GetCoupons()
        {
            try
            {
                // Arrage - already do in Constructor
                //Act
                var actionResult = await _couponsController.GetCoupons();

                //Assert
                var okResult = actionResult.Result as ObjectResult;
                Assert.NotNull(okResult);
                var resultData = okResult.Value as IEnumerable<CouponDto>;
                Assert.True(resultData != null && resultData.Count() == 2); // setup mockData in Constructor
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }

        [Fact]
        public async Task GetCoupon()
        {
            try
            {
                // Arrage - already do in Constructor
                //Act
                var actionResult = await _couponsController.GetCoupon(new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"));

                //Assert
                var okResult = actionResult.Result as ObjectResult;
                Assert.NotNull(okResult);
                var resultData = okResult.Value as CouponDto;
                Assert.True(resultData != null); // setup mockData in Constructor
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }

        [Fact]
        public async Task GetCouponValidByName()
        {
            try
            {
                // Arrage - already do in Constructor
                //Act
                var actionResult = await _couponsController.GetCouponValidByName("VOUCHER2/9");

                //Assert
                var okResult = actionResult.Result as ObjectResult;
                Assert.NotNull(okResult);
                var resultData = okResult.Value as CouponDto;
                Assert.True(resultData != null); // setup mockData in Constructor
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }

        [Fact]
        public async Task CreateCoupon()
        {
            try
            {
                var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
                var tempCurrentDateExpired = new DateTime(2024, 11, 20, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);

                // Arrage - already do in Constructor
                var newCoupon = new CouponForCreationDto()
                {
                    Name = "Test Voucher",
                    Description = "Test Voucher Description",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 2,
                };
                //Act
                var couponControllerFromMockDb = MockDB(); // use inmemoryDB here for SaveChanges working as real db function.
                //var actionResult = await _couponsController.CreateCoupon(newCoupon); // Id, CreatedDate, UpdatedDate for resultData will be MinValue because here we use mockData not mock inmemory DB so the SaveChanges can not working
                var actionResult = await couponControllerFromMockDb.CreateCoupon(newCoupon);

                //Assert
                var okResult = actionResult as OkObjectResult;
                Assert.NotNull(okResult);

                var resultData = okResult.Value as CouponDto;
                var couponByIdResult = await couponControllerFromMockDb.GetCoupon(resultData.Id); // also check if it store to inmemoryDb also like real DB. (remerber after function finish inmemory will back to original for Coupon here is empty table)

                Assert.True(resultData != null && resultData == couponByIdResult.Value); // setup mockData in Constructor
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }

        [Fact]
        public async Task DeleteCoupon()
        {
            try
            {
                var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
                var tempCurrentDateExpired = new DateTime(2024, 11, 20, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);

                // Arrage - already do in Constructor
                var newCoupon = new CouponForCreationDto()
                {
                    Name = "Test Voucher",
                    Description = "Test Voucher Description",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 2,
                };
                //Act
                var couponControllerFromMockDb = MockDB(); // use inmemoryDB here for SaveChanges working as real db function.
                var actionResult = await couponControllerFromMockDb.CreateCoupon(newCoupon);

                //Assert
                var createResult = actionResult as OkObjectResult;
                Assert.NotNull(createResult);

                var createResultData = createResult.Value as CouponDto;

                var deleteActionResult = await couponControllerFromMockDb.DeleteCoupon(createResultData.Id);
                var deleteResult = actionResult as NoContentResult;
                Assert.NotNull(deleteResult);

            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }
    }
}
