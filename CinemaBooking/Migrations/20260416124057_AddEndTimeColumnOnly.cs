using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddEndTimeColumnOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Showtimes",
                nullable: false,
                defaultValue: DateTime.Now);
            migrationBuilder.AddColumn<DateTime>(
                            name: "EndTime",
                            table: "Showtimes",
                            nullable: false,
                            defaultValue: DateTime.Now);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("StartTime", "Showtimes");
            migrationBuilder.DropColumn("EndTime", "Showtimes");
        }
    }
}
