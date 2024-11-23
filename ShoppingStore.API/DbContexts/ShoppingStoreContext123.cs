using Microsoft.EntityFrameworkCore;
using ShoppingStore.Model;
using System;
using System.Reflection.Metadata;

namespace ShoppingStore.API.DbContexts
{
    public class ShoppingStoreContext123 : DbContext
    {
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        public ShoppingStoreContext123(DbContextOptions<ShoppingStoreContext123> options) : base(options) { } // to use ConnectionString register in Container(Program.js)

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=*****;Initial Catalog=tesstSqlite123;User ID=sa;Password=*****;Trust Server Certificate=True");
                //.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                //.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // initial data (seed)
        {
            // Set not delete cascade - reference Migrations\ShoppingStoreMigrations\20240826235204_UpdateProductRatingConstraint.Designer.cs
            modelBuilder
                .Entity<ProductModel>()
                .HasOne(p => p.Brand) // navigation property/collection in product table
                .WithMany(b => b.Products) // navigation property/collection in brand table if not use can left empty
                .OnDelete(DeleteBehavior.Restrict); //default DeleteBehavior.Cascade

            modelBuilder // 1 Category with many Product
                .Entity<ProductModel>()
                .HasOne(p => p.Category) //navigation property: Category in ProductModel table
                .WithMany(c => c.Products)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder // 1 Product with many OrderDetails
                .Entity<OrderDetails>()
                .HasOne(o => o.Product)
                .WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder // prevent delete cascade rating review when delete product (should delete cascade rating when delete product)
            //.Entity<RatingModel>()
            //.HasOne(r => r.Product)
            //.WithMany(p => p.Ratings)
            //.OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //	//optionsBuilder.UseSqlite("connectionstring"); // another way to use ConnectionString register
        //	base.OnConfiguring(optionsBuilder);
        //}
    }
}
