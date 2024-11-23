using Microsoft.EntityFrameworkCore;
using ShoppingStore.API.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingStore.API.Tests.DbContexts
{
    public class ContextFactory
    {
        public static ShoppingStoreContext Create()
        {
            DbContextOptions<ShoppingStoreContext> options = new DbContextOptionsBuilder<ShoppingStoreContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options; // binh thg cho nay la .UseSql ne (ben project Api - Program.cs)

            var context = new ShoppingStoreContext(options); // also add seedData in ShoppingStoreContext (there are 2 options: 1 is Tedu,aHieu, 2: add in ShoppingStoreContext also)
            return context;
        }
    }
}
