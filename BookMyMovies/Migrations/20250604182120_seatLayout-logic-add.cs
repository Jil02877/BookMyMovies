using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyMovies.Migrations
{
    /// <inheritdoc />
    public partial class seatLayoutlogicadd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatLayoutJson",
                table: "MoviePostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatLayoutJson",
                table: "MoviePostings");
        }
    }
}
