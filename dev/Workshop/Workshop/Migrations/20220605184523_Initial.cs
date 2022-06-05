using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workshop.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationHandler",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHandler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHandler<int>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHandler<int>", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHandler<string>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHandler<string>", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchasePolicy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasePolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewHandler",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewHandler", x => x.Id);
                });

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
                name: "MemberToOrders<int>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    key = table.Column<int>(nullable: false),
                    OrderHandlerintId = table.Column<int>(name: "OrderHandler<int>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberToOrders<int>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberToOrders<int>_OrderHandler<int>_OrderHandler<int>Id",
                        column: x => x.OrderHandlerintId,
                        principalTable: "OrderHandler<int>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberToOrders<string>",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    key = table.Column<string>(nullable: true),
                    OrderHandlerstringId = table.Column<int>(name: "OrderHandler<string>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberToOrders<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberToOrders<string>_OrderHandler<string>_OrderHandler<string>Id",
                        column: x => x.OrderHandlerstringId,
                        principalTable: "OrderHandler<string>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "userController",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reviewHandlerId = table.Column<int>(nullable: true),
                    notificationHandlerId = table.Column<int>(nullable: true),
                    orderHandlerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userController", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userController_NotificationHandler_notificationHandlerId",
                        column: x => x.notificationHandlerId,
                        principalTable: "NotificationHandler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userController_OrderHandler<string>_orderHandlerId",
                        column: x => x.orderHandlerId,
                        principalTable: "OrderHandler<string>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_userController_ReviewHandler_reviewHandlerId",
                        column: x => x.reviewHandlerId,
                        principalTable: "ReviewHandler",
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
                name: "OrderDTO",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clientName = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    storeName = table.Column<string>(nullable: true),
                    MemberToOrdersintId = table.Column<int>(name: "MemberToOrders<int>Id", nullable: true),
                    MemberToOrdersstringId = table.Column<int>(name: "MemberToOrders<string>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDTO", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderDTO_MemberToOrders<int>_MemberToOrders<int>Id",
                        column: x => x.MemberToOrdersintId,
                        principalTable: "MemberToOrders<int>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDTO_MemberToOrders<string>_MemberToOrders<string>Id",
                        column: x => x.MemberToOrdersstringId,
                        principalTable: "MemberToOrders<string>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marketController",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userControllerId = table.Column<int>(nullable: true),
                    orderHandlerId = table.Column<int>(nullable: true),
                    STORE_COUNT = table.Column<int>(nullable: false),
                    PRODUCT_COUNT = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketController", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marketController_OrderHandler<int>_orderHandlerId",
                        column: x => x.orderHandlerId,
                        principalTable: "OrderHandler<int>",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_marketController_userController_userControllerId",
                        column: x => x.userControllerId,
                        principalTable: "userController",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberName = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Birthdate = table.Column<DateTime>(nullable: false),
                    ShoppingCartId = table.Column<int>(nullable: true),
                    UserControllerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberName);
                    table.ForeignKey(
                        name: "FK_Member_ShoppingCart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Member_userController_UserControllerId",
                        column: x => x.UserControllerId,
                        principalTable: "userController",
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
                name: "ProductDTO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: false),
                    OrderDTOid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDTO_OrderDTO_OrderDTOid",
                        column: x => x.OrderDTOid,
                        principalTable: "OrderDTO",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Open = table.Column<bool>(nullable: false),
                    StoreName = table.Column<string>(nullable: true),
                    DiscountPolicyId = table.Column<int>(nullable: true),
                    PurchasePolicyId = table.Column<int>(nullable: true),
                    MarketControllerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Store_DiscountPolicy_DiscountPolicyId",
                        column: x => x.DiscountPolicyId,
                        principalTable: "DiscountPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_marketController_MarketControllerId",
                        column: x => x.MarketControllerId,
                        principalTable: "marketController",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Store_PurchasePolicy_PurchasePolicyId",
                        column: x => x.PurchasePolicyId,
                        principalTable: "PurchasePolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Member_MemberName",
                        column: x => x.MemberName,
                        principalTable: "Member",
                        principalColumn: "MemberName",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Store = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Action_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_RoleId",
                table: "Action",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_marketController_orderHandlerId",
                table: "marketController",
                column: "orderHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_marketController_userControllerId",
                table: "marketController",
                column: "userControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ShoppingCartId",
                table: "Member",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UserControllerId",
                table: "Member",
                column: "UserControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberToOrders<int>_OrderHandler<int>Id",
                table: "MemberToOrders<int>",
                column: "OrderHandler<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_MemberToOrders<string>_OrderHandler<string>Id",
                table: "MemberToOrders<string>",
                column: "OrderHandler<string>Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_MemberToOrders<int>Id",
                table: "OrderDTO",
                column: "MemberToOrders<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_MemberToOrders<string>Id",
                table: "OrderDTO",
                column: "MemberToOrders<string>Id");

            migrationBuilder.CreateIndex(
                name: "IX_Product_StoreId",
                table: "Product",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDTO_OrderDTOid",
                table: "ProductDTO",
                column: "OrderDTOid");

            migrationBuilder.CreateIndex(
                name: "IX_Role_MemberName",
                table: "Role",
                column: "MemberName");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBag_ShoppingCartId",
                table: "ShoppingBag",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingBagProduct_ShoppingBagId",
                table: "ShoppingBagProduct",
                column: "ShoppingBagId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_DiscountPolicyId",
                table: "Store",
                column: "DiscountPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_MarketControllerId",
                table: "Store",
                column: "MarketControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_PurchasePolicyId",
                table: "Store",
                column: "PurchasePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_userController_notificationHandlerId",
                table: "userController",
                column: "notificationHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_userController_orderHandlerId",
                table: "userController",
                column: "orderHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_userController_reviewHandlerId",
                table: "userController",
                column: "reviewHandlerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductDTO");

            migrationBuilder.DropTable(
                name: "ShoppingBagProduct");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "OrderDTO");

            migrationBuilder.DropTable(
                name: "ShoppingBag");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "DiscountPolicy");

            migrationBuilder.DropTable(
                name: "marketController");

            migrationBuilder.DropTable(
                name: "PurchasePolicy");

            migrationBuilder.DropTable(
                name: "MemberToOrders<int>");

            migrationBuilder.DropTable(
                name: "MemberToOrders<string>");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "userController");

            migrationBuilder.DropTable(
                name: "OrderHandler<int>");

            migrationBuilder.DropTable(
                name: "NotificationHandler");

            migrationBuilder.DropTable(
                name: "OrderHandler<string>");

            migrationBuilder.DropTable(
                name: "ReviewHandler");
        }
    }
}
