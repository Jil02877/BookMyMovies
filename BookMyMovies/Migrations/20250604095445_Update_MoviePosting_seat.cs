using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyMovies.Migrations
{
    /// <inheritdoc />
    public partial class Update_MoviePosting_seat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeatAvailable",
                table: "MoviePostings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SeatsAvailable",
                table: "MoviePostings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeatAvailable",
                table: "MoviePostings");

            migrationBuilder.DropColumn(
                name: "SeatsAvailable",
                table: "MoviePostings");
        }
    }
}
