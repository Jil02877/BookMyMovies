using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMyMovies.Migrations
{
    /// <inheritdoc />
    public partial class price_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Price",
                table: "MoviePostings",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "MoviePostings");
        }
    }
}
