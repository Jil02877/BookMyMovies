using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyMovies.Migrations
{
    /// <inheritdoc />
    public partial class Update_MoviePosting_seat_no : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalSeats",
                table: "MoviePostings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalSeats",
                table: "MoviePostings");
        }
    }
}
