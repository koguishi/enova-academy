using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace enova_academy.Migrations
{
    /// <inheritdoc />
    public partial class courses_slug_unique_key : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Courses_Slug",
                table: "Courses",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_Slug",
                table: "Courses");
        }
    }
}
