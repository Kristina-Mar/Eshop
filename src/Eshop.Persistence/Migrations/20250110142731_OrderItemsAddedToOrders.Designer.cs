﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Eshop.Persistence;

#nullable disable

namespace Eshop.Persistence.Migrations
{
    [DbContext(typeof(OrdersContext))]
    [Migration("20250110142731_OrderItemsAddedToOrders")]
    partial class OrderItemsAddedToOrders
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("RestApiEshop.Domain.Models.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("ItemPrice")
                        .HasColumnType("REAL");

                    b.Property<int>("NumberOfItems")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderItemId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("RestApiEshop.Domain.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("OrderId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("RestApiEshop.Domain.Models.OrderItem", b =>
                {
                    b.HasOne("RestApiEshop.Domain.Order", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RestApiEshop.Domain.Order", b =>
                {
                    b.Navigation("OrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}
