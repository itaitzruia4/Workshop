using Microsoft.EntityFrameworkCore.Migrations;

namespace Workshop.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropColumn(
                name: "address",
                table: "OrderDTO");

            migrationBuilder.AddColumn<int>(
                name: "addressId",
                table: "OrderDTO",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SupplyAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyAddress", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_addressId",
                table: "OrderDTO",
                column: "addressId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDTO_SupplyAddress_addressId",
                table: "OrderDTO",
                column: "addressId",
                principalTable: "SupplyAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDTO_SupplyAddress_addressId",
                table: "OrderDTO");

            migrationBuilder.DropTable(
                name: "SupplyAddress");

            migrationBuilder.DropIndex(
                name: "IX_OrderDTO_addressId",
                table: "OrderDTO");

            migrationBuilder.DropColumn(
                name: "addressId",
                table: "OrderDTO");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "OrderDTO",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: false),
                    review = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reviewer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                });
        }
    }
}
