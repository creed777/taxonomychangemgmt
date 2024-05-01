using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class addreviewtext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewText",
                table: "ChangeStatus",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewText",
                table: "ChangeStatus");
        }
    }
}
