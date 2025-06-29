﻿// <auto-generated />
using System;
using FineService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FineService.Migrations
{
    [DbContext(typeof(FinesContext))]
    [Migration("20250610093255_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FineService.Aggregates.Fine.Entities.PaymentReceipt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("AccountCode")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("BankCode")
                        .HasColumnType("numeric(20,0)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("PaymentTransactionId")
                        .HasColumnType("uuid");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("PaymentReceipts");
                });

            modelBuilder.Entity("FineService.Aggregates.Fine.Entities.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Make")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("FineService.Aggregates.Fine.Fine", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("ConfirmationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("IssueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("PaymentReceiptId")
                        .HasColumnType("uuid");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PaymentReceiptId");

                    b.HasIndex("VehicleId");

                    b.ToTable("Fines");
                });

            modelBuilder.Entity("FineService.Aggregates.Fine.Entities.Vehicle", b =>
                {
                    b.OwnsOne("FineService.Aggregates.Fine.Entities.LicensePlate", "LicensePlate", b1 =>
                        {
                            b1.Property<Guid>("VehicleId")
                                .HasColumnType("uuid");

                            b1.Property<string>("BaseNumber")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<long>("Region")
                                .HasColumnType("bigint");

                            b1.HasKey("VehicleId");

                            b1.ToTable("Vehicles");

                            b1.WithOwner()
                                .HasForeignKey("VehicleId");
                        });

                    b.Navigation("LicensePlate");
                });

            modelBuilder.Entity("FineService.Aggregates.Fine.Fine", b =>
                {
                    b.HasOne("FineService.Aggregates.Fine.Entities.PaymentReceipt", "Receipt")
                        .WithMany()
                        .HasForeignKey("PaymentReceiptId");

                    b.HasOne("FineService.Aggregates.Fine.Entities.Vehicle", "Vehicle")
                        .WithMany("Fines")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receipt");

                    b.Navigation("Vehicle");
                });

            modelBuilder.Entity("FineService.Aggregates.Fine.Entities.Vehicle", b =>
                {
                    b.Navigation("Fines");
                });
#pragma warning restore 612, 618
        }
    }
}
