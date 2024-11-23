using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Thang.IDP.Migrations
{
    /// <inheritdoc />
    public partial class InitialIDP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SecurityCode = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SecurityCodeExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: false),
                    UserRoleId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ProviderIdentityKey = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecrets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Secret = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecrets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Value" },
                values: new object[,]
                {
                    { new Guid("1069eee8-509a-46f9-9800-da3d0e12d560"), "fa8c18fe-2606-47fe-9f9a-7c0e02282ff2", "FreeUser" },
                    { new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e"), "602f619e-0cb0-400c-9f35-7b16288e7cfd", "PayingUser" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "ConcurrencyStamp", "Email", "Password", "SecurityCode", "SecurityCodeExpirationDate", "Subject", "UserName", "UserRoleId" },
                values: new object[,]
                {
                    { new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), true, "0ec3688c-ed6e-46e1-96fc-f52184f3eb3a", "david@someprovider.com", "AQAAAAIAAYagAAAAEIGgD0IAkW+5XFBUko8X0PnlkVWcWfaJv2mYU2mACHEZ5ilrceWrxHYBreWHKwRfYw==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "d860efca-22d9-47fd-8249-791ba61b07c7", "David", new Guid("1069eee8-509a-46f9-9800-da3d0e12d560") },
                    { new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), true, "b6824444-4c78-417c-8708-3d0f7606c30b", "emma@someprovider.com", "AQAAAAIAAYagAAAAEIGgD0IAkW+5XFBUko8X0PnlkVWcWfaJv2mYU2mACHEZ5ilrceWrxHYBreWHKwRfYw==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "b7539694-97e7-4dfe-84da-b4256e1ff5c7", "Emma", new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e") }
                });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("09ccfd8e-7c2a-4034-88c1-9bb56578c663"), "d379c3ce-33f2-40fe-b419-bd0fcfef9f68", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Flagg" },
                    { new Guid("1fc821b9-7d2b-4a56-82df-fcf68c8c375a"), "574ddca8-f94e-4f1e-8147-7728b5af093b", "email", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "emma@someprovider.com" },
                    { new Guid("235580a3-2d76-45bd-baaa-ab47fc861f79"), "ba8686bb-320a-4e0d-b695-5402f0a7f3f2", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "David" },
                    { new Guid("3c8baee4-3fdc-42a3-80d4-4bb795205cb4"), "bde588af-1de1-4c7a-b04c-8469b77538a5", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" },
                    { new Guid("3fd3d50c-43db-4bbc-970d-1aaafe28dafe"), "68a6bbb6-462d-4abb-95c5-d478d9c29491", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Flagg" },
                    { new Guid("4f8e483f-60b7-493f-8402-13efe09946ae"), "a726318c-45f9-48fe-8fd7-3e234f8c038c", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Emma" },
                    { new Guid("5c3f5a34-98e5-4c32-8d46-6a72f49dd49d"), "fdf6da27-a27e-49e5-853c-036110be2d62", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" },
                    { new Guid("a72fb2fc-e83b-4cb1-8286-dcaa4578d940"), "82530f1c-88f3-4f3d-abe5-f34c2c988f96", "email", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "david@someprovider.com" },
                    { new Guid("df49968e-f97a-485f-b6fa-762fd01d7c36"), "8d975091-6d28-409f-8932-cabf8498f9e5", "role", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" },
                    { new Guid("f04b899e-a473-4e4c-a4b7-7f430ed98ef8"), "6f82dff5-a5bc-4134-ae21-19cc767918eb", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "FreeUser" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Subject",
                table: "Users",
                column: "Subject",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRoleId",
                table: "Users",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecrets_UserId",
                table: "UserSecrets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserSecrets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
