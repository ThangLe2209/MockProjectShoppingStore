using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class BrandsControllerTest
    {
        private readonly ShoppingStoreContext _context;
        private readonly BrandsController _brandsController;
        public BrandsControllerTest()
        {
            _context = ContextFactory.Create();
            _context.Database.EnsureCreated();
            var brandRepository = new BrandRepository(_context);
            var responseCache = new FakeResponseCacheService(); // only fake data without implement just to constructor BrandsController below(or can create another constructor without cache param in real BrandsController)  -> can't test controller action which use cache in BrandControllers

            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BrandProfile());
                cfg.AddProfile(new ProductProfile());
            });
            var mapper = mockMapper.CreateMapper();

            _brandsController = new BrandsController(brandRepository, mapper, responseCache);
        }

        [Fact]
        public async Task GetBrands()
        {
            // Arrage - already do in Constructor
            //Act
            var actionResult = await _brandsController.GetBrands(); // this function not use Cache -> can do (will find way to test cache also later - cach de nhat la lay thong tin ben api qua roi tao 1 class responseCache y chang ben api la dc nhung lam vay thì dang bi 1 cai la db ben day la inMemory => sai cache => ko nen test controller co cache, chi test nhung cache function trong ResponseCacheService thoi)

            //Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.NotNull(okResult);
            var resultData = okResult.Value as IEnumerable<BrandDto>;
            Assert.True(resultData != null && resultData.Count() == 2); // setup mockData in ShoppingStoreContext
        }

        [Fact]
        public async Task CreateBrand()
        {
            // Arrange
            var newBrand = new BrandForCreationDto()
            {
                Name = "New Brand",
                Slug = "New-Brand",
                Description = "New Brand is large brand in the world",
                Status = 1,
            };

            //Act
            var actionResult = await _brandsController.CreateBrand(newBrand);

            //Assert
            var createdResult = actionResult as CreatedAtRouteResult; // IActionResult
            Assert.NotNull(createdResult);
            var resultData = createdResult.Value as BrandDto;
            Assert.NotNull(resultData);

            var brandByIdResult = await _brandsController.GetBrand(resultData.Id);
            var getBrandByIdResult = brandByIdResult as OkObjectResult; // IActionResult
            Assert.NotNull(getBrandByIdResult);
            Assert.Equivalent(resultData, getBrandByIdResult.Value);
        }

        [Fact]
        public async Task UpdateBrand()
        {
            // Arrange
            var validMockId = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"); // setup mockData in ShoppingStoreContext

            var newBrand = new BrandForEditDto()
            {
                Name = "New Brand",
                Slug = "New-Brand",
                Description = "New Brand is large brand in the world",
                Status = 1,
            };
            //Act
            var actionResult = await _brandsController.UpdateBrand(validMockId, newBrand);

            //Assert
            var okResult = actionResult as NoContentResult;
            Assert.NotNull(okResult);

            var brandByIdResult = await _brandsController.GetBrand(validMockId);
            var getBrandByIdResult = brandByIdResult as OkObjectResult; // IActionResult
            Assert.NotNull(getBrandByIdResult);
            var brandById = getBrandByIdResult.Value as BrandDto;
            Assert.NotNull(brandById);
            Assert.True(brandById.Name == "New Brand");
        }

        [Fact]
        public async Task DeleteBrand()
        {
            var validMockId = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"); // setup mockData in ShoppingStoreContext

            // deleteCouponData
            var deleteActionResult = await _brandsController.DeleteBrand(validMockId);
            var deleteResult = deleteActionResult as NoContentResult;
            Assert.NotNull(deleteResult);

            // getCouponData again
            var brandByIdResultAgain = await _brandsController.GetBrand(validMockId);
            var brandByIdSuccessResultAgain = brandByIdResultAgain as NotFoundObjectResult;
            Assert.NotNull(brandByIdSuccessResultAgain);
        }
    }
}
