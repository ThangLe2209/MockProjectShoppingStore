using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using ShoppingStore.API.Services;
using ShoppingStore.API.Tests.DbContexts;
using ShoppingStore.Model;
using ShoppingStore.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.API.Tests.Services
{
    public class BrandRepositoryTest
    {
        private readonly ShoppingStoreContext _context;
        private readonly IBrandRepository _brandRepository;
        public BrandRepositoryTest()
        {
            _context = ContextFactory.Create();
            _context.Database.EnsureCreated();
            _brandRepository = new BrandRepository(_context);
        }

        [Fact]
        public async Task Get_All_Brands_In_MemoryDb()
        {
            var brands = await _brandRepository.GetBrandsAsync();
            Assert.True(brands != null); // similar: Assert.NotNull(brand);
        }

        [Fact]
        public async Task Get_Exist_Brand_In_MemoryDb()
        {
            var brand = await _brandRepository.GetBrandById(new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"));
            Assert.True(brand != null);
        }

        [Fact]
        public async Task Add_Brand_Success_When_Pass_Valid_Id()
        {
            //var newD = new Guid(); // {00000-00000-0000-0000}
            var newBrandId = Guid.NewGuid(); // random new Guid
            BrandModel newBrand = new()
            {
                Id = newBrandId, // nếu ko add hàng này thì khi SaveChange cũng sẽ có Id(làm real mapping á) nhưng ở đây ko test đc GetBrandById ở dưới do ko có newBrandId
                Name = "New Brand",
                Description = "Description",
                Status = 0
            };
            newBrand.Slug = newBrand.Name.Replace(" ", "-");
            _brandRepository.AddBrand(newBrand);
            await _brandRepository.SaveChangesAsync();

            //var brands = await _brandRepository.GetBrandsAsync();
            //var a = 10;

            var brand = await _brandRepository.GetBrandById(newBrandId);
            Assert.True(brand != null);
        }

        [Fact]
        public async Task Add_Brand_Fail_When_Pass_Invalid_Id()
        {
            BrandModel newBrand = new()
            {
                Id = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), // Id Brand already existed
                Name = "New Brand",
                Description = "Description",
                Status = 0
            };
            newBrand.Slug = newBrand.Name.Replace(" ", "-");

            _brandRepository.AddBrand(newBrand);
            Assert.ThrowsAsync<ArgumentException>(async () => await _brandRepository.SaveChangesAsync());
        }

        // Update ko co vi lam ben phan Controller cua api
        // gia lap Update luon
        [Fact]
        public async Task Update_Brand_Success_When_Pass_Valid_Id()
        {
            var newBrandId = Guid.NewGuid(); // random new Guid
            BrandModel newBrand = new()
            {
                Id = newBrandId, // nếu ko add hàng này thì khi SaveChange cũng sẽ có Id(làm real mapping á) nhưng ở đây ko test đc GetBrandById ở dưới do ko có newBrandId
                Name = "New Brand",
                Description = "Description",
                Status = 0
            };
            newBrand.Slug = newBrand.Name.Replace(" ", "-");
            _brandRepository.AddBrand(newBrand);
            await _brandRepository.SaveChangesAsync();

            var brand = await _brandRepository.GetBrandById(newBrandId);
            brand.Name = "Update New Brand";
            brand.Slug = brand.Name.Replace(" ", "-");
            await _brandRepository.SaveChangesAsync();

            var updatedBrand = await _brandRepository.GetBrandById(newBrandId);
            Assert.True(updatedBrand != null && updatedBrand.Name == "Update New Brand");
        }

        [Fact]
        public async Task Remove_Brand_Should_Have_Change_Record()
        {
            var newBrandId = Guid.NewGuid(); // random new Guid
            BrandModel newBrand = new()
            {
                Id = newBrandId, // nếu ko add hàng này thì khi SaveChange cũng sẽ có Id(làm real mapping á) nhưng ở đây ko test đc GetBrandById ở dưới do ko có newBrandId
                Name = "New Brand",
                Description = "Description",
                Status = 0
            };
            newBrand.Slug = newBrand.Name.Replace(" ", "-");
            _brandRepository.AddBrand(newBrand);
            await _brandRepository.SaveChangesAsync();

            _brandRepository.DeleteBrand(newBrand);
            await _brandRepository.SaveChangesAsync();

            var brand = await _brandRepository.GetBrandById(newBrandId);
            Assert.Null(brand);
        }

        [Fact]
        public async Task Pagination_Get_2_Brand_Per_Page_By_MockDb()
        {
            var data = await _brandRepository.GetBrandsAsync(searchTerm: "", pageNumber: 1, pageSize: 2);
            Assert.True(data.Item1.Count() == 2);
        }

        [Fact]
        public async Task Get_Brand_Success_When_Pass_Valid_Slug()
        {
            var brand = await _brandRepository.GetBrandBySlug("apple");
            Assert.True(brand != null);
        }

        //[Fact] // https://github.com/dotnet/efcore/issues/30246 - InMemoryDb cannot execute .FromQuery so execlude it here
        //public async Task Get_Brands_Success_By_Order_Details()
        //{
        //    //var data = await _brandRepository.GetBrandsByOrderDetailCalculationPaginateAsync(pageNumber: 1, pageSize: 2);
        //    //Assert.True(data.Item1 != null);
        //}
    }
}
