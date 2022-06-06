using Microsoft.EntityFrameworkCore.Migrations;

namespace Workshop.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "firstTermId",
                table: "PurchasePolicy",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "secondTermId",
                table: "PurchasePolicy",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "store_termsId",
                table: "DiscountPolicy",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_termsId",
                table: "DiscountPolicy",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Term",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryTerms",
                columns: table => new
                {
                    Category = table.Column<string>(nullable: false),
                    TermsId = table.Column<int>(nullable: true),
                    DiscountPolicyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryTerms", x => x.Category);
                    table.ForeignKey(
                        name: "FK_CategoryTerms_DiscountPolicy_DiscountPolicyId",
                        column: x => x.DiscountPolicyId,
                        principalTable: "DiscountPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryTerms_Term_TermsId",
                        column: x => x.TermsId,
                        principalTable: "Term",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTerms",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermsId = table.Column<int>(nullable: true),
                    DiscountPolicyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTerms", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_ProductTerms_DiscountPolicy_DiscountPolicyId",
                        column: x => x.DiscountPolicyId,
                        principalTable: "DiscountPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTerms_Term_TermsId",
                        column: x => x.TermsId,
                        principalTable: "Term",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePolicy_firstTermId",
                table: "PurchasePolicy",
                column: "firstTermId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePolicy_secondTermId",
                table: "PurchasePolicy",
                column: "secondTermId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPolicy_store_termsId",
                table: "DiscountPolicy",
                column: "store_termsId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPolicy_user_termsId",
                table: "DiscountPolicy",
                column: "user_termsId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTerms_DiscountPolicyId",
                table: "CategoryTerms",
                column: "DiscountPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTerms_TermsId",
                table: "CategoryTerms",
                column: "TermsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTerms_DiscountPolicyId",
                table: "ProductTerms",
                column: "DiscountPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTerms_TermsId",
                table: "ProductTerms",
                column: "TermsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountPolicy_Term_store_termsId",
                table: "DiscountPolicy",
                column: "store_termsId",
                principalTable: "Term",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscountPolicy_Term_user_termsId",
                table: "DiscountPolicy",
                column: "user_termsId",
                principalTable: "Term",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasePolicy_Term_firstTermId",
                table: "PurchasePolicy",
                column: "firstTermId",
                principalTable: "Term",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasePolicy_Term_secondTermId",
                table: "PurchasePolicy",
                column: "secondTermId",
                principalTable: "Term",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscountPolicy_Term_store_termsId",
                table: "DiscountPolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_DiscountPolicy_Term_user_termsId",
                table: "DiscountPolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePolicy_Term_firstTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePolicy_Term_secondTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropTable(
                name: "CategoryTerms");

            migrationBuilder.DropTable(
                name: "ProductTerms");

            migrationBuilder.DropTable(
                name: "Term");

            migrationBuilder.DropIndex(
                name: "IX_PurchasePolicy_firstTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropIndex(
                name: "IX_PurchasePolicy_secondTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropIndex(
                name: "IX_DiscountPolicy_store_termsId",
                table: "DiscountPolicy");

            migrationBuilder.DropIndex(
                name: "IX_DiscountPolicy_user_termsId",
                table: "DiscountPolicy");

            migrationBuilder.DropColumn(
                name: "firstTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropColumn(
                name: "secondTermId",
                table: "PurchasePolicy");

            migrationBuilder.DropColumn(
                name: "store_termsId",
                table: "DiscountPolicy");

            migrationBuilder.DropColumn(
                name: "user_termsId",
                table: "DiscountPolicy");
        }
    }
}
