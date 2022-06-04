using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workshop.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Password = table.Column<string>(nullable: true),
                    MemberName = table.Column<string>(nullable: true),
                    Birthdate = table.Column<DateTime>(nullable: false),
                    ShoppingCartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_ShoppingCart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(nullable: false),
                    ShoppingCartId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingBag_ShoppingCart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Role_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBagProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    ShoppingBagId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingBagProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingBagProduct_ShoppingBag_ShoppingBagId",
                        column: x => x.ShoppingBagId,
                        principalTable: "ShoppingBag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    ActionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_Action_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_RoleId",
                table: "Action",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_ShoppingCartId",
                table: "Members",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_MemberId",
                table: "Role",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBag_ShoppingCartId",
                table: "ShoppingBag",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBagProduct_ShoppingBagId",
                table: "ShoppingBagProduct",
                column: "ShoppingBagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "ShoppingBagProduct");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "ShoppingBag");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "ShoppingCart");
        }
    }
}
