using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class ScheduledQueries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduledQueries",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    Query = table.Column<string>(type: "text", nullable: false),
                    Interval = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table => table.PrimaryKey("PK_ScheduledQueries", x => x.Name));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduledQueries");
        }
    }
}
