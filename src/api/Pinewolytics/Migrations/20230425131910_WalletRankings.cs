using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class WalletRankings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UpdateTimestamps",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateTimestamps", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "WalletRankings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    StakedRank = table.Column<long>(type: "bigint", nullable: false),
                    StakedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BalanceRank = table.Column<long>(type: "bigint", nullable: false),
                    BalanceAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletRankings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoolLPerRanks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    WalletRankingId = table.Column<Guid>(type: "uuid", nullable: false),
                    PoolId = table.Column<int>(type: "integer", nullable: false),
                    GammBalance = table.Column<BigInteger>(type: "numeric", nullable: false),
                    Rank = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolLPerRanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolLPerRanks_WalletRankings_WalletRankingId",
                        column: x => x.WalletRankingId,
                        principalTable: "WalletRankings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoolLPerRanks_WalletRankingId",
                table: "PoolLPerRanks",
                column: "WalletRankingId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletRankings_Address",
                table: "WalletRankings",
                column: "Address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoolLPerRanks");

            migrationBuilder.DropTable(
                name: "UpdateTimestamps");

            migrationBuilder.DropTable(
                name: "WalletRankings");
        }
    }
}
