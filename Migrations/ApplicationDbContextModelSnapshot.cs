﻿// <auto-generated />
using System;
using CreditGuardAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CreditGuardAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("CreditGuardAPI.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AadhaarNumber")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("TEXT");

                    b.Property<int?>("ActiveGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AadhaarNumber")
                        .IsUnique();

                    b.HasIndex("ActiveGroupId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.CustomerGroup", b =>
                {
                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("JoinedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("CustomerId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("CustomerGroups");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Debt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("EMIAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("RemainingAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("TotalEMICount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Debts");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.EmiTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DebtId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("EMIAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("EMINumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("PaidAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("PaidDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("DebtId");

                    b.HasIndex("GroupId");

                    b.ToTable("EmiTransactions");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("SecretAnswer")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("SecretQuestion")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT")
                        .HasAnnotation("RegularExpression", "^[a-zA-Z0-9_]+$");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Customer", b =>
                {
                    b.HasOne("CreditGuardAPI.Models.Group", "ActiveGroup")
                        .WithMany("ActiveCustomers")
                        .HasForeignKey("ActiveGroupId");

                    b.Navigation("ActiveGroup");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.CustomerGroup", b =>
                {
                    b.HasOne("CreditGuardAPI.Models.Customer", "Customer")
                        .WithMany("CustomerGroups")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CreditGuardAPI.Models.Group", "Group")
                        .WithMany("CustomerGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Debt", b =>
                {
                    b.HasOne("CreditGuardAPI.Models.Group", "Group")
                        .WithMany("Debts")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.EmiTransaction", b =>
                {
                    b.HasOne("CreditGuardAPI.Models.Customer", "Customer")
                        .WithMany("EmiTransactions")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CreditGuardAPI.Models.Debt", "Debt")
                        .WithMany("EmiTransactions")
                        .HasForeignKey("DebtId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CreditGuardAPI.Models.Group", "Group")
                        .WithMany("EmiTransactions")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Debt");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Customer", b =>
                {
                    b.Navigation("CustomerGroups");

                    b.Navigation("EmiTransactions");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Debt", b =>
                {
                    b.Navigation("EmiTransactions");
                });

            modelBuilder.Entity("CreditGuardAPI.Models.Group", b =>
                {
                    b.Navigation("ActiveCustomers");

                    b.Navigation("CustomerGroups");

                    b.Navigation("Debts");

                    b.Navigation("EmiTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
