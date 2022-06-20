using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Workshop.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    discountJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationHandler",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHandler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHandlerI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHandlerI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderHandlerS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderHandlerS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    review = table.Column<string>(nullable: true),
                    reviewer = table.Column<string>(nullable: true),
                    productId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDTO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Review = table.Column<string>(nullable: true),
                    Reviewer = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewDTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewHandler",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
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
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupplyAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "DiscountPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    store_discountId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountPolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountPolicy_Discount_store_discountId",
                        column: x => x.store_discountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventObservers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: true),
                    NotificationHandlerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventObservers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventObservers_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventObservers_NotificationHandler_NotificationHandlerId",
                        column: x => x.NotificationHandlerId,
                        principalTable: "NotificationHandler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberToOrdersI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    key = table.Column<int>(nullable: false),
                    OrderHandlerintId = table.Column<int>(name: "OrderHandler<int>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberToOrdersI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberToOrdersI_OrderHandlerI_OrderHandler<int>Id",
                        column: x => x.OrderHandlerintId,
                        principalTable: "OrderHandlerI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberToOrdersS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    key = table.Column<string>(nullable: true),
                    OrderHandlerstringId = table.Column<int>(name: "OrderHandler<string>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberToOrdersS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberToOrdersS_OrderHandlerS_OrderHandler<string>Id",
                        column: x => x.OrderHandlerstringId,
                        principalTable: "OrderHandlerS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ReviewHandlerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductReviews_ReviewHandler_ReviewHandlerId",
                        column: x => x.ReviewHandlerId,
                        principalTable: "ReviewHandler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "userController",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                        name: "FK_userController_OrderHandlerS_orderHandlerId",
                        column: x => x.orderHandlerId,
                        principalTable: "OrderHandlerS",
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
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ReviewHandlerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReviews_ReviewHandler_ReviewHandlerId",
                        column: x => x.ReviewHandlerId,
                        principalTable: "ReviewHandler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                name: "CategoryDiscount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    DiscountId = table.Column<int>(nullable: true),
                    DiscountPolicyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryDiscount_Discount_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoryDiscount_DiscountPolicy_DiscountPolicyId",
                        column: x => x.DiscountPolicyId,
                        principalTable: "DiscountPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDiscount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    DiscountId = table.Column<int>(nullable: true),
                    DiscountPolicyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDiscount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDiscount_Discount_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDiscount_DiscountPolicy_DiscountPolicyId",
                        column: x => x.DiscountPolicyId,
                        principalTable: "DiscountPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDTO",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false),
                    clientName = table.Column<string>(nullable: true),
                    addressId = table.Column<int>(nullable: true),
                    storeName = table.Column<string>(nullable: true),
                    MemberToOrdersintId = table.Column<int>(name: "MemberToOrders<int>Id", nullable: true),
                    MemberToOrdersstringId = table.Column<int>(name: "MemberToOrders<string>Id", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDTO", x => x.id);
                    table.ForeignKey(
                        name: "FK_OrderDTO_MemberToOrdersI_MemberToOrders<int>Id",
                        column: x => x.MemberToOrdersintId,
                        principalTable: "MemberToOrdersI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDTO_MemberToOrdersS_MemberToOrders<string>Id",
                        column: x => x.MemberToOrdersstringId,
                        principalTable: "MemberToOrdersS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDTO_SupplyAddress_addressId",
                        column: x => x.addressId,
                        principalTable: "SupplyAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserToReviewDTO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    ReviewId = table.Column<int>(nullable: true),
                    ProductReviewsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToReviewDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserToReviewDTO_ProductReviews_ProductReviewsId",
                        column: x => x.ProductReviewsId,
                        principalTable: "ProductReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserToReviewDTO_ReviewDTO_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "ReviewDTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marketController",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    userControllerId = table.Column<int>(nullable: true),
                    orderHandlerId = table.Column<int>(nullable: true),
                    STORE_COUNT = table.Column<int>(nullable: false),
                    PRODUCT_COUNT = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marketController", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marketController_OrderHandlerI_orderHandlerId",
                        column: x => x.orderHandlerId,
                        principalTable: "OrderHandlerI",
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
                    EventObserversId = table.Column<int>(nullable: true),
                    UserControllerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberName);
                    table.ForeignKey(
                        name: "FK_Member_EventObservers_EventObserversId",
                        column: x => x.EventObserversId,
                        principalTable: "EventObservers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "ProductToReviewDTO",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ReviewId = table.Column<int>(nullable: true),
                    UserReviewsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToReviewDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToReviewDTO_ReviewDTO_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "ReviewDTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductToReviewDTO_UserReviews_UserReviewsId",
                        column: x => x.UserReviewsId,
                        principalTable: "UserReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingBagProduct",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                    Id = table.Column<int>(nullable: false),
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
                name: "MemberNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    MemberName = table.Column<string>(nullable: true),
                    NotificationHandlerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberNotifications_Member_MemberName",
                        column: x => x.MemberName,
                        principalTable: "Member",
                        principalColumn: "MemberName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberNotifications_NotificationHandler_NotificationHandlerId",
                        column: x => x.NotificationHandlerId,
                        principalTable: "NotificationHandler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    RoleType = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: false),
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
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    TimeOfEvent = table.Column<DateTime>(nullable: false),
                    MemberNotificationsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_MemberNotifications_MemberNotificationsId",
                        column: x => x.MemberNotificationsId,
                        principalTable: "MemberNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Action",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "NameToRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    roleId = table.Column<int>(nullable: true),
                    memberName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameToRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NameToRole_Role_roleId",
                        column: x => x.roleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
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
                name: "Term",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    TermJson = table.Column<string>(nullable: true),
                    PurchasePolicyId = table.Column<int>(nullable: true),
                    PurchasePolicyId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Term", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchasePolicy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    user_termsId = table.Column<int>(nullable: true),
                    store_termsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasePolicy_Term_store_termsId",
                        column: x => x.store_termsId,
                        principalTable: "Term",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchasePolicy_Term_user_termsId",
                        column: x => x.user_termsId,
                        principalTable: "Term",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Action_RoleId",
                table: "Action",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDiscount_DiscountId",
                table: "CategoryDiscount",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDiscount_DiscountPolicyId",
                table: "CategoryDiscount",
                column: "DiscountPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPolicy_store_discountId",
                table: "DiscountPolicy",
                column: "store_discountId");

            migrationBuilder.CreateIndex(
                name: "IX_EventObservers_EventId",
                table: "EventObservers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventObservers_NotificationHandlerId",
                table: "EventObservers",
                column: "NotificationHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_marketController_orderHandlerId",
                table: "marketController",
                column: "orderHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_marketController_userControllerId",
                table: "marketController",
                column: "userControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_EventObserversId",
                table: "Member",
                column: "EventObserversId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ShoppingCartId",
                table: "Member",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UserControllerId",
                table: "Member",
                column: "UserControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberNotifications_MemberName",
                table: "MemberNotifications",
                column: "MemberName");

            migrationBuilder.CreateIndex(
                name: "IX_MemberNotifications_NotificationHandlerId",
                table: "MemberNotifications",
                column: "NotificationHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberToOrdersI_OrderHandler<int>Id",
                table: "MemberToOrdersI",
                column: "OrderHandler<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_MemberToOrdersS_OrderHandler<string>Id",
                table: "MemberToOrdersS",
                column: "OrderHandler<string>Id");

            migrationBuilder.CreateIndex(
                name: "IX_NameToRole_roleId",
                table: "NameToRole",
                column: "roleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_MemberNotificationsId",
                table: "Notification",
                column: "MemberNotificationsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_MemberToOrders<int>Id",
                table: "OrderDTO",
                column: "MemberToOrders<int>Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_MemberToOrders<string>Id",
                table: "OrderDTO",
                column: "MemberToOrders<string>Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDTO_addressId",
                table: "OrderDTO",
                column: "addressId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_StoreId",
                table: "Product",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscount_DiscountId",
                table: "ProductDiscount",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDiscount_DiscountPolicyId",
                table: "ProductDiscount",
                column: "DiscountPolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDTO_OrderDTOid",
                table: "ProductDTO",
                column: "OrderDTOid");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ReviewHandlerId",
                table: "ProductReviews",
                column: "ReviewHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToReviewDTO_ReviewId",
                table: "ProductToReviewDTO",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToReviewDTO_UserReviewsId",
                table: "ProductToReviewDTO",
                column: "UserReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePolicy_store_termsId",
                table: "PurchasePolicy",
                column: "store_termsId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchasePolicy_user_termsId",
                table: "PurchasePolicy",
                column: "user_termsId");

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
                name: "IX_Term_PurchasePolicyId",
                table: "Term",
                column: "PurchasePolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Term_PurchasePolicyId1",
                table: "Term",
                column: "PurchasePolicyId1");

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

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_ReviewHandlerId",
                table: "UserReviews",
                column: "ReviewHandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToReviewDTO_ProductReviewsId",
                table: "UserToReviewDTO",
                column: "ProductReviewsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserToReviewDTO_ReviewId",
                table: "UserToReviewDTO",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_PurchasePolicy_PurchasePolicyId",
                table: "Store",
                column: "PurchasePolicyId",
                principalTable: "PurchasePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Term_PurchasePolicy_PurchasePolicyId",
                table: "Term",
                column: "PurchasePolicyId",
                principalTable: "PurchasePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Term_PurchasePolicy_PurchasePolicyId1",
                table: "Term",
                column: "PurchasePolicyId1",
                principalTable: "PurchasePolicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePolicy_Term_store_termsId",
                table: "PurchasePolicy");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasePolicy_Term_user_termsId",
                table: "PurchasePolicy");

            migrationBuilder.DropTable(
                name: "Action");

            migrationBuilder.DropTable(
                name: "CategoryDiscount");

            migrationBuilder.DropTable(
                name: "NameToRole");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductDiscount");

            migrationBuilder.DropTable(
                name: "ProductDTO");

            migrationBuilder.DropTable(
                name: "ProductToReviewDTO");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "ShoppingBagProduct");

            migrationBuilder.DropTable(
                name: "UserToReviewDTO");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "MemberNotifications");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "OrderDTO");

            migrationBuilder.DropTable(
                name: "UserReviews");

            migrationBuilder.DropTable(
                name: "ShoppingBag");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "ReviewDTO");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "DiscountPolicy");

            migrationBuilder.DropTable(
                name: "marketController");

            migrationBuilder.DropTable(
                name: "MemberToOrdersI");

            migrationBuilder.DropTable(
                name: "MemberToOrdersS");

            migrationBuilder.DropTable(
                name: "SupplyAddress");

            migrationBuilder.DropTable(
                name: "EventObservers");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "userController");

            migrationBuilder.DropTable(
                name: "OrderHandlerI");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "NotificationHandler");

            migrationBuilder.DropTable(
                name: "OrderHandlerS");

            migrationBuilder.DropTable(
                name: "ReviewHandler");

            migrationBuilder.DropTable(
                name: "Term");

            migrationBuilder.DropTable(
                name: "PurchasePolicy");
        }
    }
}
