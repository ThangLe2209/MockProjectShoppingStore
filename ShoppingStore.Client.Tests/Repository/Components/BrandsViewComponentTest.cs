using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using ShoppingStore.Client.Repository;
using ShoppingStore.Client.Repository.Components;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ShoppingStore.Client.Tests.Repository.Components
{
    public class BrandsViewComponentTest
    {
        [Fact]
        public async Task InvokeAsync()
        {
            //Arrange
            var brandService = new Mock<BrandService>();

            //mock brand service
            var tempCurrentDate = new DateTime(2024, 10, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new List<BrandDto>() { // Serialize Object -> List dc con IEnumerable(Interface) ko dc
                new BrandDto() { Id = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), Name = "Apple", Slug = "apple", Description = "Apple is large brand in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate }
            }));
            var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = content };
            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new PaginationMetadata(2, 1, 1){ }));

            brandService.Setup(x => x.GetBrandsByOrderDetailsPaginate("",1,4))
                .ReturnsAsync(response);

            //Act
            //List<KeyValuePair<string, StringValues>> mockRequestQuery = new ();
            //mockRequestQuery.Add(new KeyValuePair<string, StringValues>("slug", "test"));
            Dictionary<String, StringValues> mockRequestQueryData = new();
            mockRequestQueryData.Add("slug", "test");
            IQueryCollection mockRequestQuery = new QueryCollection(mockRequestQueryData);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.Request.Query).Returns(mockRequestQuery);
            var viewContext = new ViewContext();
            viewContext.HttpContext = mockHttpContext.Object;
            var viewComponentContext = new ViewComponentContext();
            viewComponentContext.ViewContext = viewContext;

            var viewComponent = new BrandsViewComponent(brandService.Object);
            viewComponent.ViewComponentContext = viewComponentContext;
            var result = await viewComponent.InvokeAsync(1,"");

            //Assert
            var okResult = Assert.IsType<ViewViewComponentResult>(result);

            Assert.True(okResult.ViewData["slugQueryString"] == "test"); // check ViewBag
            Assert.True((int)okResult.ViewData["brandMenuId"] == 1);
            Assert.Equivalent(okResult.ViewData["Pager"], new PaginationMetadata(2, 1, 1) { }, strict: true);


            var model = okResult.ViewData.Model as List<BrandDto>; //check model
            Assert.True(model.Count() == 1 && model.Any(b => b.Id == new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0")));
            //...other assertions
        }
    }
}
