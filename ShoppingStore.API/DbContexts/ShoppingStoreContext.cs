using Microsoft.EntityFrameworkCore;
using ShoppingStore.Model;
using System;
using System.Numerics;
using System.Reflection.Metadata;

namespace ShoppingStore.API.DbContexts
{
    public class ShoppingStoreContext : DbContext
    {
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<RatingModel> Ratings { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<SliderModel> Sliders { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<WishlistModel> Wishlists { get; set; }
        public DbSet<CompareModel> Compares { get; set; }
        public DbSet<ProductQuantityModel> ProductQuantites { get; set; }
        public DbSet<ShippingModel> Shippings { get; set; }
        public DbSet<CouponModel> Coupons { get; set; }
        public DbSet<StatisticalModel> Statisticals { get; set; }
        public DbSet<StatisticalProductOrderModel> StatisticalProductOrders { get; set; }
        //public DbSet<MomoInfoModel> MomoInfos { get; set; }
        public DbSet<VnpayModel> VnpayInfos { get; set; }


        public ShoppingStoreContext(DbContextOptions<ShoppingStoreContext> options) : base(options) { } // to use ConnectionString register in Container(Program.js)

        protected override void OnModelCreating(ModelBuilder modelBuilder) // initial data (seed)
        {
            var tempCurrentDate = new DateTime(2024, 10, 19, 22, 42, 59, DateTimeKind.Local).AddTicks(1312);

            // Don't use Guid.NewGuid() use new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"); instead
            CategoryModel macbook = new() { Id = new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"), Name = "Macbook", Slug = "macbook", Description = "Macbook is large Product in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate };
            CategoryModel pc = new() { Id = new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"), Name = "Pc", Slug = "pc", Description = "Pc is large Product in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate };

            BrandModel apple = new() { Id = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), Name = "Apple", Slug = "apple", Description = "Apple is large brand in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate };
            BrandModel samsung = new() { Id = new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"), Name = "Samsung", Slug = "samsung", Description = "Samsung is large brand in the world", Status = 1, CreatedDate = tempCurrentDate, UpdatedDate = tempCurrentDate };

            modelBuilder.Entity<ProductModel>()
                .HasData(
                new ProductModel()
                {
                    Id = new Guid("eae3da18-ae13-45e2-80b5-cfa8ffeeea00"),
                    Name = "Macbook",
                    Slug = "macbook",
                    Description = "Macbook is the Best",
                    Image = "1.jpg",
                    CategoryId = macbook.Id,
                    BrandId = apple.Id,
                    Price = 1233,
                    CapitalPrice = 1233,
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate
                },
                new ProductModel()
                {
                    Id = new Guid("1280759c-7f99-4ce0-baa3-fb78bc2e6e58"),
                    Name = "Pc1234",
                    Slug = "pc1234",
                    Description = "Pc1234 is the Best",
                    Image = "1.jpg",
                    CategoryId = pc.Id,
                    BrandId = samsung.Id,
                    Price = 1233,
                    CapitalPrice = 1233,
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate
                });

            modelBuilder.Entity<CategoryModel>().HasData(macbook, pc);
            modelBuilder.Entity<BrandModel>().HasData(apple, samsung);

            modelBuilder.Entity<ContactModel>().HasData(
                new ContactModel()
                {
                    Id = new Guid("85db2c6b-477f-4895-9fd9-8cd50d6096ab"),
                    Name = "EShopper Shopping Store",
                    Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa expedita ipsam deserunt nam et molestias doloribus necessitatibus eveniet, provident, eum quo quos delectus consequatur voluptatibus ut voluptate adipisci nulla distinctio!",
                    Map = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3918.4854676751984!2d106.76933817480598!3d10.850632389302646!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752763f23816ab%3A0x282f711441b6916f!2sHCMC%20University%20of%20Technology%20and%20Education!5e0!3m2!1sen!2s!4v1729520635763!5m2!1sen!2s\" width=\"300\" height=\"200\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>",
                    Email = "admin@gmail.com",
                    Phone = "0935768432",
                    LogoImg = "https://thumbs.dreamstime.com/b/lets-shopping-logo-design-template-shop-icon-135610500.jpg",
                    CreatedDate = tempCurrentDate,
                    UpdatedDate = tempCurrentDate
                });

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

            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entitytype in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entitytype.ClrType.GetProperties().Where(p => p.PropertyType == typeof(decimal));
                    foreach (var property in properties)
                    {
                        //Converting decimal to double
                        modelBuilder.Entity(entitytype.Name).Property(property.Name).HasConversion<double>();
                    }
                }
            }
        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //	//optionsBuilder.UseSqlite("connectionstring"); // another way to use ConnectionString register
        //	base.OnConfiguring(optionsBuilder);
        //}

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((BaseEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    var createdDate = ((BaseEntity)entityEntry.Entity).CreatedDate;
                    if (createdDate == DateTime.MinValue) // e.CreatedDate == default(DateTime) 
                    {
                        ((BaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
