using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ShoppingStore.Client.Areas.Admin.Controllers;
using ShoppingStore.Client.Repository;
using ShoppingStore.Controllers;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.Client.Tests.Areas.Admin.Controllers
{
    public class ProductControllerTest
    {
        private Mock<ProductService> _productService;
        private Mock<CategoryService> _categoryService;
        private Mock<BrandService> _brandService;
        private Mock<ProductQuantityService> _productQuantityService;

        public ProductControllerTest()
        {
            _productService = new Mock<ProductService>();
            _categoryService = new Mock<CategoryService>();
            _brandService = new Mock<BrandService>();
            _productQuantityService = new Mock<ProductQuantityService>();
        }

        [Fact]
        public async Task Create()
        {
            var tempCurrentDate = new DateTime(2024, 10, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);

            // mock category service
            _categoryService.Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(new List<CategoryDto>() {
                    new CategoryDto() { Id = new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"), Name = "Macbook", Slug = "macbook", Description = "Macbook is large Product in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate },
                    new CategoryDto() { Id = new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"), Name = "Pc", Slug = "pc", Description = "Pc is large Product in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate }
                }
            );

            // mock brand service
            _brandService.Setup(x => x.GetBrandsAsync())
                .ReturnsAsync(new List<BrandDto>() {
                    new() { Id = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), Name = "Apple", Slug = "apple", Description = "Apple is large brand in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate },
                    new() { Id = new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"), Name = "Samsung", Slug = "samsung", Description = "Samsung is large brand in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate }
                }
            );

            //mock order detail service
            _productService.Setup(x => x.CreateProductAsync(It.IsAny<object>())) // mock dynamic
                .ReturnsAsync(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });

            // call controller
            ProductForCreationDto newProduct = new()
            {
                Name = "Test Product1",
                Description = "Test Product1 is the Best",
                Image = "1.jpg",
                CategoryId = new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"),
                BrandId = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"),
                Price = 1233,
                CapitalPrice = 1233
            };

            var mockHttpContext = new Mock<HttpContext>();

            var controller = new ProductController(_productService.Object, _categoryService.Object,
                _brandService.Object, _productQuantityService.Object);
            controller.TempData = new TempDataDictionary(mockHttpContext.Object, new Mock<ITempDataProvider>().Object);

            var result = await controller.Create(newProduct);
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
