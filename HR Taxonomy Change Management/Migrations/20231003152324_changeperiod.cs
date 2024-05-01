using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class changeperiod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChangePeriodId",
                table: "Request",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChangePeriod",
                columns: table => new
                {
                    ChangePeriodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangePeriod", x => x.ChangePeriodId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_ChangePeriodId",
                table: "Request",
                column: "ChangePeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ChangePeriod_ChangePeriodId",
                table: "Request",
                column: "ChangePeriodId",
                principalTable: "ChangePeriod",
                principalColumn: "ChangePeriodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_ChangePeriod_ChangePeriodId",
                table: "Request");

            migrationBuilder.DropTable(
                name: "ChangePeriod");

            migrationBuilder.DropIndex(
                name: "IX_Request_ChangePeriodId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "ChangePeriodId",
                table: "Request");
        }
    }
}
