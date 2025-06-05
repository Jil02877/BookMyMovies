using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyMovies.Migrations
{
    /// <inheritdoc />
    public partial class Update_MoviePostings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "MoviePostings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "MoviePostings");
        }
    }
}
