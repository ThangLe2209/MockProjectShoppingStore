using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.API.Tests.DbContexts
{
    public class ShoppingStoreContextTest
    {
        private readonly ShoppingStoreContext _context;

        public ShoppingStoreContextTest()
        {
            _context = ContextFactory.Create(); // also add seedData in ShoppingStoreContext so don't need test Should_Success_When_Seed_Data() below as anh Tedu do
            _context.Database.EnsureCreated();
        }

        [Fact]
        public void Constructor_CreateInMemoryDb_Success () 
        {
            var context = ContextFactory.Create();
            Assert.NotNull(context.Database.EnsureCreated());
        }

        //[Fact]
        //public async Task Should_Success_When_Seed_Data()
        //{
        //}
    }
}
