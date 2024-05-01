using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class addtaxonomyidtochangedetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                table: "ChangeDetail",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CurrentL1Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentL2Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentL3Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentL4Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentL5Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewL1Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewL2Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewL3Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewL4Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewL5Id",
                table: "ChangeDetail",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentL1Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "CurrentL2Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "CurrentL3Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "CurrentL4Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "CurrentL5Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "NewL1Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "NewL2Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "NewL3Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "NewL4Id",
                table: "ChangeDetail");

            migrationBuilder.DropColumn(
                name: "NewL5Id",
                table: "ChangeDetail");

            migrationBuilder.AlterColumn<int>(
                name: "LegacyId",
                table: "ChangeDetail",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
