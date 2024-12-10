﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShoppingStore.API.DbContexts;

#nullable disable

namespace ShoppingStore.API.Migrations.ShoppingStoreMigrations
{
    [DbContext(typeof(ShoppingStoreContext))]
    partial class ShoppingStoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("ShoppingStore.Model.BrandModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Brands");

                    b.HasData(
                        new
                        {
                            Id = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Apple is large brand in the world",
                            Name = "Apple",
                            Slug = "apple",
                            Status = 1,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        },
                        new
                        {
                            Id = new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Samsung is large brand in the world",
                            Name = "Samsung",
                            Slug = "samsung",
                            Status = 1,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        });
                });

            modelBuilder.Entity("ShoppingStore.Model.CategoryModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Macbook is large Product in the world",
                            Name = "Macbook",
                            Slug = "macbook",
                            Status = 1,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        },
                        new
                        {
                            Id = new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Pc is large Product in the world",
                            Name = "Pc",
                            Slug = "pc",
                            Status = 1,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        });
                });

            modelBuilder.Entity("ShoppingStore.Model.CompareModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("Compares");
                });

            modelBuilder.Entity("ShoppingStore.Model.ContactModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LogoImg")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Map")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Contacts");

                    b.HasData(
                        new
                        {
                            Id = new Guid("85db2c6b-477f-4895-9fd9-8cd50d6096ab"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa expedita ipsam deserunt nam et molestias doloribus necessitatibus eveniet, provident, eum quo quos delectus consequatur voluptatibus ut voluptate adipisci nulla distinctio!",
                            Email = "admin@gmail.com",
                            LogoImg = "https://thumbs.dreamstime.com/b/lets-shopping-logo-design-template-shop-icon-135610500.jpg",
                            Map = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3918.4854676751984!2d106.76933817480598!3d10.850632389302646!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752763f23816ab%3A0x282f711441b6916f!2sHCMC%20University%20of%20Technology%20and%20Education!5e0!3m2!1sen!2s!4v1729520635763!5m2!1sen!2s\" width=\"300\" height=\"200\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>",
                            Name = "EShopper Shopping Store",
                            Phone = "0935768432",
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        });
                });

            modelBuilder.Entity("ShoppingStore.Model.CouponModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateExpired")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateStart")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("DiscountDecrease")
                        .HasColumnType("REAL");

                    b.Property<double>("DiscountPercent")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Coupons");
                });

            modelBuilder.Entity("ShoppingStore.Model.OrderDetails", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("ShoppingStore.Model.OrderModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CouponCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("TEXT");

                    b.Property<double>("ShippingCost")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("ShoppingStore.Model.ProductModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BrandId")
                        .HasColumnType("TEXT");

                    b.Property<double>("CapitalPrice")
                        .HasColumnType("REAL");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("decimal(8,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<int>("Sold")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = new Guid("eae3da18-ae13-45e2-80b5-cfa8ffeeea00"),
                            BrandId = new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"),
                            CapitalPrice = 1233.0,
                            CategoryId = new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Macbook is the Best",
                            Image = "1.jpg",
                            Name = "Macbook",
                            Price = 1233.0,
                            Quantity = 0,
                            Slug = "macbook",
                            Sold = 0,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        },
                        new
                        {
                            Id = new Guid("1280759c-7f99-4ce0-baa3-fb78bc2e6e58"),
                            BrandId = new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"),
                            CapitalPrice = 1233.0,
                            CategoryId = new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"),
                            CreatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312),
                            Description = "Pc1234 is the Best",
                            Image = "1.jpg",
                            Name = "Pc1234",
                            Price = 1233.0,
                            Quantity = 0,
                            Slug = "pc1234",
                            Sold = 0,
                            UpdatedDate = new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312)
                        });
                });

            modelBuilder.Entity("ShoppingStore.Model.ProductQuantityModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductQuantites");
                });

            modelBuilder.Entity("ShoppingStore.Model.RatingModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Star")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("ShoppingStore.Model.ShippingModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("District")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ward")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Shippings");
                });

            modelBuilder.Entity("ShoppingStore.Model.SliderModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sliders");
                });

            modelBuilder.Entity("ShoppingStore.Model.StatisticalModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Profit")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Revenue")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Sold")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Statisticals");
                });

            modelBuilder.Entity("ShoppingStore.Model.StatisticalProductOrderModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CouponCode")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<double>("ProductPrice")
                        .HasColumnType("REAL");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<double>("TotalRevenue")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("StatisticalProductOrders");
                });

            modelBuilder.Entity("ShoppingStore.Model.VnpayModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrderId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PaymentId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VnpayInfos");
                });

            modelBuilder.Entity("ShoppingStore.Model.WishlistModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("Wishlists");
                });

            modelBuilder.Entity("ShoppingStore.Model.CompareModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithOne("Compare")
                        .HasForeignKey("ShoppingStore.Model.CompareModel", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.OrderDetails", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.ProductModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.BrandModel", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ShoppingStore.Model.CategoryModel", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ShoppingStore.Model.ProductQuantityModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithMany("ProductQuantities")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.RatingModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithMany("Ratings")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.StatisticalProductOrderModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.WishlistModel", b =>
                {
                    b.HasOne("ShoppingStore.Model.ProductModel", "Product")
                        .WithOne("Wishlist")
                        .HasForeignKey("ShoppingStore.Model.WishlistModel", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShoppingStore.Model.BrandModel", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ShoppingStore.Model.CategoryModel", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("ShoppingStore.Model.ProductModel", b =>
                {
                    b.Navigation("Compare")
                        .IsRequired();

                    b.Navigation("OrderDetails");

                    b.Navigation("ProductQuantities");

                    b.Navigation("Ratings");

                    b.Navigation("Wishlist")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
