using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Thang.IDP.Migrations
{
    /// <inheritdoc />
    public partial class InitialDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecurityCode = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SecurityCodeExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: false),
                    UserRoleId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Value = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProviderIdentityKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: false)
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
                    { new Guid("1069eee8-509a-46f9-9800-da3d0e12d560"), "de940ab0-f1ba-49eb-bfcb-91639a25ee2a", "FreeUser" },
                    { new Guid("d65cf2bd-3840-41d3-af42-f7ff1a7122ef"), "eb53a067-1fbf-470a-8023-b8b67697bc39", "Admin" },
                    { new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e"), "1c93bbe6-5654-40bf-9854-c9246eb52a94", "PayingUser" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "ConcurrencyStamp", "Email", "Password", "SecurityCode", "SecurityCodeExpirationDate", "Subject", "UserName", "UserRoleId" },
                values: new object[,]
                {
                    { new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), true, "8abe4c23-7894-450a-9eaf-25c4558dc8b3", "david@someprovider.com", "AQAAAAIAAYagAAAAEA2UtwPl2ClVXS2GxZAimtkI1J9LDURK7V5VUi/jjwSl3DA7ILndLMPYR5JxVKJDxQ==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "d860efca-22d9-47fd-8249-791ba61b07c7", "David", new Guid("d65cf2bd-3840-41d3-af42-f7ff1a7122ef") },
                    { new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), true, "7d1c0040-a124-4eb1-84bf-df6f5f118863", "emma@someprovider.com", "AQAAAAIAAYagAAAAEA2UtwPl2ClVXS2GxZAimtkI1J9LDURK7V5VUi/jjwSl3DA7ILndLMPYR5JxVKJDxQ==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "b7539694-97e7-4dfe-84da-b4256e1ff5c7", "Emma", new Guid("d7ab6668-2af4-4ea4-a93b-3d96dc475d8e") }
                });

            migrationBuilder.InsertData(
                table: "UserClaims",
                columns: new[] { "Id", "ConcurrencyStamp", "Type", "UserId", "Value" },
                values: new object[,]
                {
                    { new Guid("5082b6d2-3383-4138-b0bc-5599974b436c"), "8a141208-44e0-4bf4-815b-e7a0fb03887f", "country", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "be" },
                    { new Guid("65e9a6c4-33e1-44bc-872c-9588fcddb8ec"), "94b4af66-c14f-419d-a1d5-fbd77eb9faee", "email", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "david@someprovider.com" },
                    { new Guid("7cde2c6a-1996-4179-bee9-713aae6b5da1"), "a3ab6263-8958-44b3-8bda-5d973f808ecd", "family_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Flagg" },
                    { new Guid("7f43601b-b696-46f4-ad61-027a45b38484"), "1a66b6ef-9eea-4f0b-8091-888748086634", "family_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Flagg" },
                    { new Guid("8095faee-0fdb-47f4-86c6-0ae03cc946f4"), "de85dc9d-e66b-45e5-a1e6-0c24d9782855", "given_name", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "Emma" },
                    { new Guid("9b842d95-8cf8-4374-b8bc-4b99b4e7fd6e"), "af71e890-82b7-4bb2-9d8d-f25af5193899", "role", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "Admin" },
                    { new Guid("a6017373-fe89-429c-bb36-77399a88c3b7"), "25690a42-d814-483e-9852-d2590f1b1c63", "email", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "emma@someprovider.com" },
                    { new Guid("badde062-0d2e-4e67-84b2-33cd997acbca"), "9783c759-2486-4e19-bd02-78a64c42c01f", "country", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "nl" },
                    { new Guid("c158d545-c6df-4747-99d8-71d8ebf36b4d"), "b531abfe-5444-433b-b419-093a611043b0", "role", new Guid("96053525-f4a5-47ee-855e-0ea77fa6c55a"), "PayingUser" },
                    { new Guid("f5b2859b-0893-4952-8188-e2d7f1c31330"), "1606a068-cce0-4fbf-b6dd-87843af68fec", "given_name", new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"), "David" }
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
