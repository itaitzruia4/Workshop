﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Workshop.DataLayer;

namespace Workshop.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20220628161649_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Controllers.MarketController", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("PRODUCT_COUNT")
                        .HasColumnType("int");

                    b.Property<int>("STORE_COUNT")
                        .HasColumnType("int");

                    b.Property<int?>("orderHandlerId")
                        .HasColumnType("int");

                    b.Property<int?>("userControllerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("orderHandlerId");

                    b.HasIndex("userControllerId");

                    b.ToTable("marketController");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Controllers.UserController", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("notificationHandlerId")
                        .HasColumnType("int");

                    b.Property<int?>("orderHandlerId")
                        .HasColumnType("int");

                    b.Property<int?>("reviewHandlerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("notificationHandlerId");

                    b.HasIndex("orderHandlerId");

                    b.HasIndex("reviewHandlerId");

                    b.ToTable("userController");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.CategoryDiscount", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountPolicyId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.HasIndex("DiscountPolicyId");

                    b.ToTable("CategoryDiscount");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.Discount", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("discountJson")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Discount");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("store_discountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("store_discountId");

                    b.ToTable("DiscountPolicy");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.ProductDiscount", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountPolicyId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.HasIndex("DiscountPolicyId");

                    b.ToTable("ProductDiscount");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Product", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Store")
                        .HasColumnType("int");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ProductDTO", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderDTOid")
                        .HasColumnType("int");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderDTOid");

                    b.ToTable("ProductDTO");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.CategoryTerm", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PurchasePolicyId")
                        .HasColumnType("int");

                    b.Property<int?>("TermId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PurchasePolicyId");

                    b.HasIndex("TermId");

                    b.ToTable("CategoryTerm");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.ProductTerm", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("PurchasePolicyId")
                        .HasColumnType("int");

                    b.Property<int?>("TermId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PurchasePolicyId");

                    b.HasIndex("TermId");

                    b.ToTable("ProductTerm");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("store_termsId")
                        .HasColumnType("int");

                    b.Property<int?>("user_termsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("store_termsId");

                    b.HasIndex("user_termsId");

                    b.ToTable("PurchasePolicy");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.Term", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("TermJson")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Term");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingBag", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("ShoppingCartId")
                        .HasColumnType("int");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingCartId");

                    b.ToTable("ShoppingBag");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("ShoppingBagId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingBagId");

                    b.ToTable("ShoppingBagProduct");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingCart", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Store", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("DiscountPolicyId")
                        .HasColumnType("int");

                    b.Property<int?>("MarketControllerId")
                        .HasColumnType("int");

                    b.Property<bool>("Open")
                        .HasColumnType("bit");

                    b.Property<int?>("PurchasePolicyId")
                        .HasColumnType("int");

                    b.Property<string>("StoreName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DiscountPolicyId");

                    b.HasIndex("MarketControllerId");

                    b.HasIndex("PurchasePolicyId");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.SupplyAddress", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Zip")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SupplyAddress");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Action", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ActionType")
                        .HasColumnType("int");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Action");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Member", b =>
                {
                    b.Property<string>("MemberName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ShoppingCartId")
                        .HasColumnType("int");

                    b.Property<int?>("StoreId")
                        .HasColumnType("int");

                    b.Property<int?>("UserControllerId")
                        .HasColumnType("int");

                    b.HasKey("MemberName");

                    b.HasIndex("ShoppingCartId");

                    b.HasIndex("StoreId");

                    b.HasIndex("UserControllerId");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.NameToRole", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("data_key")
                        .HasColumnType("int");

                    b.Property<int?>("fatherId")
                        .HasColumnType("int");

                    b.Property<string>("memberName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("fatherId");

                    b.ToTable("NameToRole");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Role", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("MemberName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("StoreId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberName");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.Event", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sender")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Event");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.EventObservers", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("EventId")
                        .HasColumnType("int");

                    b.Property<int?>("NotificationHandlerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("NotificationHandlerId");

                    b.ToTable("EventObservers");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.EventObserversToMembers", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("EventObserverId")
                        .HasColumnType("int");

                    b.Property<string>("MemberName")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("EventObserverId");

                    b.HasIndex("MemberName");

                    b.ToTable("EventObserversToMembers");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.MemberNotifications", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("MemberName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("NotificationHandlerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberName");

                    b.HasIndex("NotificationHandlerId");

                    b.ToTable("MemberNotifications");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.Notification", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("MemberNotificationsId")
                        .HasColumnType("int");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeOfEvent")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MemberNotificationsId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.NotificationHandler", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("NotificationHandler");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<int>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("OrderHandler<int>Id")
                        .HasColumnType("int");

                    b.Property<int>("key")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderHandler<int>Id");

                    b.ToTable("MemberToOrdersI");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<string>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("OrderHandler<string>Id")
                        .HasColumnType("int");

                    b.Property<string>("key")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OrderHandler<string>Id");

                    b.ToTable("MemberToOrdersS");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.OrderDTO", b =>
                {
                    b.Property<int>("id")
                        .HasColumnType("int");

                    b.Property<int?>("MemberToOrders<int>Id")
                        .HasColumnType("int");

                    b.Property<int?>("MemberToOrders<string>Id")
                        .HasColumnType("int");

                    b.Property<int?>("addressId")
                        .HasColumnType("int");

                    b.Property<string>("clientName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("storeId")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("MemberToOrders<int>Id");

                    b.HasIndex("MemberToOrders<string>Id");

                    b.HasIndex("addressId");

                    b.ToTable("OrderDTO");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.OrderHandler<int>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("OrderHandlerI");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.OrderHandler<string>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("OrderHandlerS");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ProductReviews", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("ReviewHandlerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReviewHandlerId");

                    b.ToTable("ProductReviews");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ProductToReviewDTO", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int?>("ReviewId")
                        .HasColumnType("int");

                    b.Property<int?>("UserReviewsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReviewId");

                    b.HasIndex("UserReviewsId");

                    b.ToTable("ProductToReviewDTO");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.Review", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("productId")
                        .HasColumnType("int");

                    b.Property<string>("review")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("reviewer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Review");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ReviewDTO", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("Review")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reviewer")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReviewDTO");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ReviewHandler", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ReviewHandler");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.UserReviews", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("ReviewHandlerId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ReviewHandlerId");

                    b.ToTable("UserReviews");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.UserToReviewDTO", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("ProductReviewsId")
                        .HasColumnType("int");

                    b.Property<int?>("ReviewId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductReviewsId");

                    b.HasIndex("ReviewId");

                    b.ToTable("UserToReviewDTO");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Controllers.MarketController", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.OrderHandler<int>", "orderHandler")
                        .WithMany()
                        .HasForeignKey("orderHandlerId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Controllers.UserController", "userController")
                        .WithMany()
                        .HasForeignKey("userControllerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Controllers.UserController", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.NotificationHandler", "notificationHandler")
                        .WithMany()
                        .HasForeignKey("notificationHandlerId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.OrderHandler<string>", "orderHandler")
                        .WithMany()
                        .HasForeignKey("orderHandlerId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ReviewHandler", "reviewHandler")
                        .WithMany()
                        .HasForeignKey("reviewHandlerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.CategoryDiscount", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy", null)
                        .WithMany("category_discounts")
                        .HasForeignKey("DiscountPolicyId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.Discount", "store_discount")
                        .WithMany()
                        .HasForeignKey("store_discountId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Discounts.ProductDiscount", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy", null)
                        .WithMany("products_discounts")
                        .HasForeignKey("DiscountPolicyId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Product", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Store", null)
                        .WithMany("Products")
                        .HasForeignKey("StoreId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ProductDTO", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.OrderDTO", null)
                        .WithMany("items")
                        .HasForeignKey("OrderDTOid");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.CategoryTerm", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy", null)
                        .WithMany("category_terms")
                        .HasForeignKey("PurchasePolicyId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.Term", "Term")
                        .WithMany()
                        .HasForeignKey("TermId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.ProductTerm", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy", null)
                        .WithMany("products_terms")
                        .HasForeignKey("PurchasePolicyId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.Term", "Term")
                        .WithMany()
                        .HasForeignKey("TermId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.Term", "store_terms")
                        .WithMany()
                        .HasForeignKey("store_termsId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.Term", "user_terms")
                        .WithMany()
                        .HasForeignKey("user_termsId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingBag", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.ShoppingCart", null)
                        .WithMany("ShoppingBags")
                        .HasForeignKey("ShoppingCartId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingBagProduct", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.ShoppingBag", null)
                        .WithMany("Products")
                        .HasForeignKey("ShoppingBagId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.Store", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Discounts.DiscountPolicy", "DiscountPolicy")
                        .WithMany()
                        .HasForeignKey("DiscountPolicyId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Controllers.MarketController", null)
                        .WithMany("stores")
                        .HasForeignKey("MarketControllerId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Purchases.PurchasePolicy", "PurchasePolicy")
                        .WithMany()
                        .HasForeignKey("PurchasePolicyId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Action", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Role", null)
                        .WithMany("Actions")
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Member", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Market.ShoppingCart", "ShoppingCart")
                        .WithMany()
                        .HasForeignKey("ShoppingCartId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.Store", null)
                        .WithMany("Owners")
                        .HasForeignKey("StoreId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Controllers.UserController", null)
                        .WithMany("members")
                        .HasForeignKey("UserControllerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.NameToRole", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Role", "father")
                        .WithMany("nominees")
                        .HasForeignKey("fatherId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Role", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Member", null)
                        .WithMany("Roles")
                        .HasForeignKey("MemberName");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.EventObservers", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.Event", "Event")
                        .WithMany()
                        .HasForeignKey("EventId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.NotificationHandler", null)
                        .WithMany("observers")
                        .HasForeignKey("NotificationHandlerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.EventObserversToMembers", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.EventObservers", "EventObserver")
                        .WithMany("Observers")
                        .HasForeignKey("EventObserverId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Member", "member")
                        .WithMany("EventObservers")
                        .HasForeignKey("MemberName");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.MemberNotifications", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberName");

                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.NotificationHandler", null)
                        .WithMany("Notifications")
                        .HasForeignKey("NotificationHandlerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Notifications.Notification", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Notifications.MemberNotifications", null)
                        .WithMany("Notifications")
                        .HasForeignKey("MemberNotificationsId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<int>", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.OrderHandler<int>", null)
                        .WithMany("MemberToOrders")
                        .HasForeignKey("OrderHandler<int>Id");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<string>", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.OrderHandler<string>", null)
                        .WithMany("MemberToOrders")
                        .HasForeignKey("OrderHandler<string>Id");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Orders.OrderDTO", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<int>", null)
                        .WithMany("orders")
                        .HasForeignKey("MemberToOrders<int>Id");

                    b.HasOne("Workshop.DataLayer.DataObjects.Orders.MemberToOrders<string>", null)
                        .WithMany("orders")
                        .HasForeignKey("MemberToOrders<string>Id");

                    b.HasOne("Workshop.DataLayer.DataObjects.Market.SupplyAddress", "address")
                        .WithMany()
                        .HasForeignKey("addressId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ProductReviews", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ReviewHandler", null)
                        .WithMany("productReviews")
                        .HasForeignKey("ReviewHandlerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.ProductToReviewDTO", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ReviewDTO", "Review")
                        .WithMany()
                        .HasForeignKey("ReviewId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.UserReviews", null)
                        .WithMany("productToReviewDTOs")
                        .HasForeignKey("UserReviewsId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.UserReviews", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ReviewHandler", null)
                        .WithMany("userReviews")
                        .HasForeignKey("ReviewHandlerId");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Reviews.UserToReviewDTO", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ProductReviews", null)
                        .WithMany("userToReviewDTOs")
                        .HasForeignKey("ProductReviewsId");

                    b.HasOne("Workshop.DataLayer.DataObjects.Reviews.ReviewDTO", "Review")
                        .WithMany()
                        .HasForeignKey("ReviewId");
                });
#pragma warning restore 612, 618
        }
    }
}
