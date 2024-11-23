using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoppingStore.API.Migrations.ShoppingStoreMigrations
{
    /// <inheritdoc />
    public partial class InitTestDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Slug = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Map = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    LogoImg = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DateStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateExpired = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderCode = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingCost = table.Column<double>(type: "REAL", nullable: false),
                    CouponCode = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shippings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Ward = table.Column<string>(type: "TEXT", nullable: false),
                    District = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shippings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statisticals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Sold = table.Column<int>(type: "INTEGER", nullable: false),
                    Revenue = table.Column<int>(type: "INTEGER", nullable: false),
                    Profit = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statisticals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VnpayInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderDescription = table.Column<string>(type: "TEXT", nullable: false),
                    TransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    OrderId = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: false),
                    PaymentId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VnpayInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Slug = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<double>(type: "decimal(8,2)", nullable: false),
                    CapitalPrice = table.Column<double>(type: "REAL", nullable: false),
                    BrandId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Sold = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compares",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compares_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderCode = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductQuantites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductQuantites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductQuantites_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Star = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StatisticalProductOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    OrderCode = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductPrice = table.Column<double>(type: "REAL", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalRevenue = table.Column<double>(type: "REAL", nullable: false),
                    CouponCode = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatisticalProductOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StatisticalProductOrders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "CreatedDate", "Description", "Name", "Slug", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Samsung is large brand in the world", "Samsung", "samsung", 1, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) },
                    { new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Apple is large brand in the world", "Apple", "apple", 1, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedDate", "Description", "Name", "Slug", "Status", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Pc is large Product in the world", "Pc", "pc", 1, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) },
                    { new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Macbook is large Product in the world", "Macbook", "macbook", 1, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) }
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "CreatedDate", "Description", "Email", "LogoImg", "Map", "Name", "Phone", "UpdatedDate" },
                values: new object[] { new Guid("85db2c6b-477f-4895-9fd9-8cd50d6096ab"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Lorem ipsum dolor sit amet consectetur adipisicing elit. Ipsa expedita ipsam deserunt nam et molestias doloribus necessitatibus eveniet, provident, eum quo quos delectus consequatur voluptatibus ut voluptate adipisci nulla distinctio!", "admin@gmail.com", "https://thumbs.dreamstime.com/b/lets-shopping-logo-design-template-shop-icon-135610500.jpg", "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3918.4854676751984!2d106.76933817480598!3d10.850632389302646!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752763f23816ab%3A0x282f711441b6916f!2sHCMC%20University%20of%20Technology%20and%20Education!5e0!3m2!1sen!2s!4v1729520635763!5m2!1sen!2s\" width=\"300\" height=\"200\" style=\"border:0;\" allowfullscreen=\"\" loading=\"lazy\" referrerpolicy=\"no-referrer-when-downgrade\"></iframe>", "EShopper Shopping Store", "0935768432", new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CapitalPrice", "CategoryId", "CreatedDate", "Description", "Image", "Name", "Price", "Quantity", "Slug", "Sold", "UpdatedDate" },
                values: new object[,]
                {
                    { new Guid("1280759c-7f99-4ce0-baa3-fb78bc2e6e58"), new Guid("4965063c-40fe-40bd-b237-c7c90ff25f1c"), 1233.0, new Guid("0868e0f8-a3e3-4e46-aa2a-fc4351bbf8b5"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Pc1234 is the Best", "1.jpg", "Pc1234", 1233.0, 0, "pc1234", 0, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) },
                    { new Guid("eae3da18-ae13-45e2-80b5-cfa8ffeeea00"), new Guid("9e4391a2-747a-4d22-bafb-2045f76002a0"), 1233.0, new Guid("a99e8fcd-a305-4780-b28b-c83512a3c2b6"), new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312), "Macbook is the Best", "1.jpg", "Macbook", 1233.0, 0, "macbook", 0, new DateTime(2024, 10, 19, 22, 42, 59, 0, DateTimeKind.Local).AddTicks(1312) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Compares_ProductId",
                table: "Compares",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductQuantites_ProductId",
                table: "ProductQuantites",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProductId",
                table: "Ratings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StatisticalProductOrders_ProductId",
                table: "StatisticalProductOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_ProductId",
                table: "Wishlists",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compares");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductQuantites");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Shippings");

            migrationBuilder.DropTable(
                name: "Sliders");

            migrationBuilder.DropTable(
                name: "StatisticalProductOrders");

            migrationBuilder.DropTable(
                name: "Statisticals");

            migrationBuilder.DropTable(
                name: "VnpayInfos");

            migrationBuilder.DropTable(
                name: "Wishlists");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
