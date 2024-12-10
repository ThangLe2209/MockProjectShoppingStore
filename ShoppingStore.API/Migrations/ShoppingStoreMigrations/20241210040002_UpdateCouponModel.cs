using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingStore.API.Migrations.ShoppingStoreMigrations
{
    /// <inheritdoc />
    public partial class UpdateCouponModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountDecrease",
                table: "Coupons",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountPercent",
                table: "Coupons",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountDecrease",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Coupons");
        }
    }
}
