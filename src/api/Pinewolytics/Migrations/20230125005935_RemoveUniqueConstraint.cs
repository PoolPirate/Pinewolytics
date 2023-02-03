using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ICNSNames_OSMOAddress",
                table: "ICNSNames");

            migrationBuilder.CreateIndex(
                name: "IX_ICNSNames_OSMOAddress",
                table: "ICNSNames",
                column: "OSMOAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ICNSNames_OSMOAddress",
                table: "ICNSNames");

            migrationBuilder.CreateIndex(
                name: "IX_ICNSNames_OSMOAddress",
                table: "ICNSNames",
                column: "OSMOAddress",
                unique: true);
        }
    }
}
