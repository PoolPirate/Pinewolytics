using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class AddICNSNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ICNSNames",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    OSMOAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ICNSNames", x => x.Name));

            migrationBuilder.CreateIndex(
                name: "IX_ICNSNames_OSMOAddress",
                table: "ICNSNames",
                column: "OSMOAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ICNSNames");
        }
    }
}
