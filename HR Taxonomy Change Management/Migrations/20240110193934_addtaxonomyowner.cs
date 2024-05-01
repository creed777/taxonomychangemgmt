using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class addtaxonomyowner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Taxonomy",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxonomyOwner",
                columns: table => new
                {
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxonomyOwner", x => x.OwnerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Taxonomy_OwnerId",
                table: "Taxonomy",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Taxonomy_TaxonomyOwner_OwnerId",
                table: "Taxonomy",
                column: "OwnerId",
                principalTable: "TaxonomyOwner",
                principalColumn: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Taxonomy_TaxonomyOwner_OwnerId",
                table: "Taxonomy");

            migrationBuilder.DropTable(
                name: "TaxonomyOwner");

            migrationBuilder.DropIndex(
                name: "IX_Taxonomy_OwnerId",
                table: "Taxonomy");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Taxonomy");
        }
    }
}
