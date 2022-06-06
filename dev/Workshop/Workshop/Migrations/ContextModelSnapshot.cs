﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Workshop.DataLayer;

namespace Workshop.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Market.ShoppingBag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("ShoppingCart");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Action", b =>
                {
                    b.Property<int>("ActionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("ActionId");

                    b.HasIndex("RoleId");

                    b.ToTable("Action");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Birthdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MemberName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ShoppingCartId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingCartId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("MemberId")
                        .HasColumnType("int");

                    b.HasKey("RoleId");

                    b.HasIndex("MemberId");

                    b.ToTable("Role");
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
                });

            modelBuilder.Entity("Workshop.DataLayer.DataObjects.Members.Role", b =>
                {
                    b.HasOne("Workshop.DataLayer.DataObjects.Members.Member", null)
                        .WithMany("Roles")
                        .HasForeignKey("MemberId");
                });
#pragma warning restore 612, 618
        }
    }
}
