using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBooking.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueBookingDetailSeat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingDetails_SeatId",
                table: "BookingDetails");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_SeatId",
                table: "BookingDetails",
                column: "SeatId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingDetails_SeatId",
                table: "BookingDetails");

            migrationBuilder.CreateIndex(
                name: "IX_BookingDetails_SeatId",
                table: "BookingDetails",
                column: "SeatId");
        }
    }
}
