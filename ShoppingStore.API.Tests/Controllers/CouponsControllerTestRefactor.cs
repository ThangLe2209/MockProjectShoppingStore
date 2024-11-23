using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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
    public class CouponsControllerTestRefactor
    {
        private readonly ShoppingStoreContext _context;
        private readonly CouponsController _couponsController;

        public CouponsControllerTestRefactor()
        {
            _context = ContextFactory.Create();
            _context.Database.EnsureCreated();

            var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var tempCurrentDateExpired = new DateTime(2024, 12, 23, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var coupons = new List<CouponModel>(){
                new CouponModel()
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
                },
                new CouponModel()
                {
                    Id = new Guid("ae49e41e-a76a-41c6-8a9a-08dcf7f18ab2"),
                    Name = "VOUCHER2/9",
                    Description = "VOUCHER2/9 giảm giá 100 $ cho các mặt hàng lưu niệm ngày Quốc Khánh 2 tháng 9",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 2,
                    Status = 1,
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate,
                },
            };
            _context.Coupons.AddRange(coupons); // ko co mock thi AddRange de mock o day la dc
            _context.SaveChanges();

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CouponProfile());
            });
            var mapper = mockMapper.CreateMapper();

            // Validator
            var couponRepository = new CouponRepository(_context);
            _couponsController = new CouponsController(couponRepository, mapper);
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
                //Assert.False(false, ex.Message);
                throw new Exception("Fail");
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
                //Assert.False(false, ex.Message);
                throw new Exception("Fail");
            }
        }

        [Fact]
        public async Task GetCouponValidByName() //DateExpired must > than DateTime.Now(if not set mockdata again)
        {
            // Arrage - already do in Constructor
            //Act
            var actionResult = await _couponsController.GetCouponValidByName("VOUCHER2/9");

            //Assert
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var resultData = okResult.Value as CouponDto;
            Assert.True(resultData != null); // setup mockData in Constructor
        }

        [Fact]
        public async Task CreateCoupon()
        {
            try
            {
                // Arrange
                var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
                var tempCurrentDateExpired = new DateTime(2024, 11, 20, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
                var newCoupon = new CouponForCreationDto()
                {
                    Name = "Test Voucher",
                    Description = "Test Voucher Description",
                    DateStart = tempCurrentDate,
                    DateExpired = tempCurrentDateExpired,
                    Quantity = 2,
                };
                //Act
                var actionResult = await _couponsController.CreateCoupon(newCoupon);

                //Assert
                var createdResult = actionResult as CreatedAtRouteResult; // IActionResult
                Assert.NotNull(createdResult);
                var resultData = createdResult.Value as CouponDto;
                Assert.NotNull(resultData);

                var couponByIdResult = await _couponsController.GetCoupon(resultData.Id);
                var getCouponByIdResult = couponByIdResult.Result as OkObjectResult; // ActionResult<T>
                Assert.NotNull(getCouponByIdResult);
                Assert.Equivalent(resultData, getCouponByIdResult.Value);
            }
            catch (Exception ex)
            {
                //Assert
                //Assert.False(false, ex.Message);
                throw new Exception("Fail");
            }
        }

        [Fact]
        public async Task UpdateCoupon()
        {
            // Arrange
            var validMockId = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"); // setup mockData in Constructor

            var tempCurrentDate = new DateTime(2024, 11, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var tempCurrentDateExpired = new DateTime(2024, 11, 20, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            var newEditCoupon = new CouponForEditDto()
            {
                Name = "Test Update Voucher",
                Description = "Test Update Voucher Description",
                DateStart = tempCurrentDate,
                DateExpired = tempCurrentDateExpired,
                Quantity = 12,
            };
            //Act
            var actionResult = await _couponsController.UpdateCoupon(validMockId, newEditCoupon);

            //Assert
            var okResult = actionResult as NoContentResult;
            Assert.NotNull(okResult);

            var couponByIdResult = await _couponsController.GetCoupon(validMockId);
            var getCouponByIdResult = couponByIdResult.Result as OkObjectResult; // ActionResult<T>
            Assert.NotNull(getCouponByIdResult);
            var couponById = getCouponByIdResult.Value as CouponDto;
            Assert.NotNull(couponById);
            Assert.True(couponById.Name == "Test Update Voucher"); // setup mockData in Constructor
        }

        [Fact]
        public async Task PartiallyUpdateCouponAsync()
        {
            // Bypass TryValidateModel() Null Exception
            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            _couponsController.ObjectValidator = objectValidator.Object;

            // Arrange
            var validMockId = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"); // setup mockData in Constructor

            var updatedCouponJsonPatch = new JsonPatchDocument<CouponForEditDto>();
            updatedCouponJsonPatch.Replace(c => c.Name, "Test Partial Update Coupon");
            //Act
            var actionResult = await _couponsController.PartiallyUpdateCouponAsync(validMockId, updatedCouponJsonPatch);

            //Assert
            var okResult = actionResult as NoContentResult;
            Assert.NotNull(okResult);
            var couponByIdResult = await _couponsController.GetCoupon(validMockId);
            var getCouponByIdResult = couponByIdResult.Result as OkObjectResult; // ActionResult<T>
            Assert.NotNull(getCouponByIdResult);
            var couponById = getCouponByIdResult.Value as CouponDto;
            Assert.NotNull(couponById);
            Assert.True(couponById.Name == "Test Partial Update Coupon"); // setup mockData in Constructor
        }

        [Fact]
        public async Task DeleteCoupon()
        {
            try
            {
                var validMockId = new Guid("c71afcd5-c5c2-4367-8a9b-08dcf7f18ab2"); // setup mockData in Constructor

                // getCouponData
                var couponByIdResult = await _couponsController.GetCoupon(validMockId);
                var couponByIdSuccessResult = couponByIdResult.Result as OkObjectResult;
                Assert.NotNull(couponByIdSuccessResult);
                var couponById = couponByIdSuccessResult.Value as CouponDto;
                Assert.True(couponById != null); // setup mockData in Constructor

                // deleteCouponData
                var deleteActionResult = await _couponsController.DeleteCoupon(validMockId);
                var deleteResult = deleteActionResult as NoContentResult;
                Assert.NotNull(deleteResult);

                // getCouponData again
                var couponByIdResultAgain = await _couponsController.GetCoupon(validMockId);
                var couponByIdSuccessResultAgain = couponByIdResultAgain.Result as NotFoundObjectResult;
                Assert.NotNull(couponByIdSuccessResultAgain);
            }
            catch (Exception ex)
            {
                //Assert
                //Assert.False(false, ex.Message);
                throw new Exception("Fail");
            }
        }
    }
}
