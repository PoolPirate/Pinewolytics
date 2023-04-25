﻿// <auto-generated />
using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Pinewolytics.Database;

#nullable disable

namespace Pinewolytics.Migrations
{
    [DbContext(typeof(PinewolyticsContext))]
    partial class PinewolyticsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Pinewolytics.Entities.PoolLPerRank", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<long>("Id"));

                    b.Property<BigInteger>("GammBalance")
                        .HasColumnType("numeric");

                    b.Property<int>("PoolId")
                        .HasColumnType("integer");

                    b.Property<long>("Rank")
                        .HasColumnType("bigint");

                    b.Property<Guid>("WalletRankingId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WalletRankingId");

                    b.ToTable("PoolLPerRanks", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Entities.UpdateTimestamp", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Key");

                    b.ToTable("UpdateTimestamps", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Entities.WalletRanking", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("BalanceAmount")
                        .HasColumnType("numeric");

                    b.Property<long>("BalanceRank")
                        .HasColumnType("bigint");

                    b.Property<decimal>("StakedAmount")
                        .HasColumnType("numeric");

                    b.Property<long>("StakedRank")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Address")
                        .IsUnique();

                    b.ToTable("WalletRankings", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ICNSName", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("OSMOAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.HasIndex("OSMOAddress");

                    b.ToTable("ICNSNames", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ProtoRevSwap", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<BigInteger>("Profit")
                        .HasColumnType("numeric");

                    b.Property<decimal>("ProfitUSD")
                        .HasColumnType("numeric");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TransactionId");

                    b.ToTable("ProtoRevSwaps", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ProtoRevTransaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TxFrom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("TxFrom");

                    b.ToTable("ProtoRevTransactions", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ScheduledQuery", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<TimeSpan>("Interval")
                        .HasColumnType("interval");

                    b.Property<string>("Query")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Name");

                    b.ToTable("ScheduledQueries", (string)null);
                });

            modelBuilder.Entity("Pinewolytics.Entities.PoolLPerRank", b =>
                {
                    b.HasOne("Pinewolytics.Entities.WalletRanking", "WalletRanking")
                        .WithMany("LPerRanks")
                        .HasForeignKey("WalletRankingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WalletRanking");
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ProtoRevSwap", b =>
                {
                    b.HasOne("Pinewolytics.Models.Entities.ProtoRevTransaction", "Transaction")
                        .WithMany("Swaps")
                        .HasForeignKey("TransactionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("Pinewolytics.Entities.WalletRanking", b =>
                {
                    b.Navigation("LPerRanks");
                });

            modelBuilder.Entity("Pinewolytics.Models.Entities.ProtoRevTransaction", b =>
                {
                    b.Navigation("Swaps");
                });
#pragma warning restore 612, 618
        }
    }
}
