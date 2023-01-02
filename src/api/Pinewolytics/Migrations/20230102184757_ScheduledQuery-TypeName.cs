using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pinewolytics.Migrations
{
    /// <inheritdoc />
    public partial class ScheduledQueryTypeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "ScheduledQueries",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "ScheduledQueries");
        }
    }
}
