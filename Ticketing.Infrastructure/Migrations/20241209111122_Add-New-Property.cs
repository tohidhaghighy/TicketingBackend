using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketing.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSchedule",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSchedule",
                table: "Tickets");
        }
    }
}
