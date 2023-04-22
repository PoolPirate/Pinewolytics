using System;
using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class ProtoRevTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProtoRevTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TxFrom = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtoRevTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtoRevSwaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Profit = table.Column<BigInteger>(type: "numeric", nullable: false),
                    ProfitUSD = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtoRevSwaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtoRevSwaps_ProtoRevTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "ProtoRevTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProtoRevSwaps_TransactionId",
                table: "ProtoRevSwaps",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProtoRevTransactions_Hash",
                table: "ProtoRevTransactions",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProtoRevTransactions_TxFrom",
                table: "ProtoRevTransactions",
                column: "TxFrom");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProtoRevSwaps");

            migrationBuilder.DropTable(
                name: "ProtoRevTransactions");
        }
    }
}
