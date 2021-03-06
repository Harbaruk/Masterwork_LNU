﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Starter.DAL;
using System;

namespace Starter.DAL.Migrations
{
    [DbContext(typeof(ProjectDbContext))]
    partial class ProjectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Starter.DAL.Entities.BankAccountEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Balance");

                    b.Property<DateTimeOffset?>("ExpiresAt");

                    b.Property<DateTimeOffset>("OpenedAt");

                    b.Property<Guid?>("OwnerId");

                    b.Property<string>("Status");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("BankAccounts");
                });

            modelBuilder.Entity("Starter.DAL.Entities.BlockEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BlockHash")
                        .IsRequired();

                    b.Property<string>("BlockState")
                        .IsRequired();

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("MinerHash");

                    b.Property<long>("Nonce");

                    b.Property<string>("PreviousBlockHash")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("MinerHash");

                    b.ToTable("Blocks");
                });

            modelBuilder.Entity("Starter.DAL.Entities.BlockVerificationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlockId");

                    b.Property<string>("UserPublicKey")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.ToTable("BlockVerifications");
                });

            modelBuilder.Entity("Starter.DAL.Entities.PermissionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("Starter.DAL.Entities.RefreshTokenEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Token")
                        .IsRequired();

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("Starter.DAL.Entities.TransactionEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BlockId");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("Description");

                    b.Property<string>("FromAccountId");

                    b.Property<Guid?>("InitiatorId");

                    b.Property<decimal>("Money");

                    b.Property<string>("State")
                        .IsRequired();

                    b.Property<string>("ToAccountId");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("InitiatorId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Starter.DAL.Entities.TrustfullServerEntity", b =>
                {
                    b.Property<string>("Hash")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("PublicKey")
                        .IsRequired();

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.HasKey("Hash");

                    b.ToTable("TrustfullServers");
                });

            modelBuilder.Entity("Starter.DAL.Entities.TwoFactorAuthEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CreatedAt");

                    b.Property<string>("Secret")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("TwoFactorAuth");
                });

            modelBuilder.Entity("Starter.DAL.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Firstname")
                        .IsRequired();

                    b.Property<bool>("IsVerified");

                    b.Property<string>("Lastname")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Role")
                        .IsRequired();

                    b.Property<string>("Salt")
                        .IsRequired();

                    b.Property<int?>("TwoFactorAuthId");

                    b.HasKey("Id");

                    b.HasIndex("TwoFactorAuthId")
                        .IsUnique()
                        .HasFilter("[TwoFactorAuthId] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Starter.DAL.Entities.UserPermissionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("PermissionId");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPermissions");
                });

            modelBuilder.Entity("Starter.DAL.Entities.BankAccountEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.UserEntity", "Owner")
                        .WithMany("Accounts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.BlockEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.TrustfullServerEntity", "Miner")
                        .WithMany("Blocks")
                        .HasForeignKey("MinerHash")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.BlockVerificationEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.BlockEntity", "Block")
                        .WithMany("Verifications")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.RefreshTokenEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.UserEntity", "User")
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.TransactionEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.BlockEntity", "Block")
                        .WithMany("Transactions")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Starter.DAL.Entities.BankAccountEntity", "FromAccount")
                        .WithMany("SentTransactions")
                        .HasForeignKey("FromAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Starter.DAL.Entities.UserEntity", "Initiator")
                        .WithMany("Transactions")
                        .HasForeignKey("InitiatorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Starter.DAL.Entities.BankAccountEntity", "ToAccount")
                        .WithMany("ReceivedTransactions")
                        .HasForeignKey("ToAccountId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.UserEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.TwoFactorAuthEntity", "TwoFactorAuth")
                        .WithOne("User")
                        .HasForeignKey("Starter.DAL.Entities.UserEntity", "TwoFactorAuthId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Starter.DAL.Entities.UserPermissionEntity", b =>
                {
                    b.HasOne("Starter.DAL.Entities.PermissionEntity", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Starter.DAL.Entities.UserEntity", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
