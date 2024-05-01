using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class addchangerequeststatusestypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeStatus_StatusType_StatusTypeId",
                table: "ChangeStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_StatusType_StatusTypeId",
                table: "RequestStatus");

            migrationBuilder.DropTable(
                name: "StatusType");

            migrationBuilder.RenameColumn(
                name: "StatusTypeId",
                table: "RequestStatus",
                newName: "RequestStatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatus_StatusTypeId",
                table: "RequestStatus",
                newName: "IX_RequestStatus_RequestStatusTypeId");

            migrationBuilder.RenameColumn(
                name: "StatusTypeId",
                table: "ChangeStatus",
                newName: "ChangeStatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ChangeStatus_StatusTypeId",
                table: "ChangeStatus",
                newName: "IX_ChangeStatus_ChangeStatusTypeId");

            migrationBuilder.CreateTable(
                name: "ChangeStatusType",
                columns: table => new
                {
                    ChangeStatusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeStatusType", x => x.ChangeStatusTypeId);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatusType",
                columns: table => new
                {
                    RequestStatusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatusType", x => x.RequestStatusTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeStatus_ChangeStatusType_ChangeStatusTypeId",
                table: "ChangeStatus",
                column: "ChangeStatusTypeId",
                principalTable: "ChangeStatusType",
                principalColumn: "ChangeStatusTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_RequestStatusType_RequestStatusTypeId",
                table: "RequestStatus",
                column: "RequestStatusTypeId",
                principalTable: "RequestStatusType",
                principalColumn: "RequestStatusTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeStatus_ChangeStatusType_ChangeStatusTypeId",
                table: "ChangeStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_RequestStatusType_RequestStatusTypeId",
                table: "RequestStatus");

            migrationBuilder.DropTable(
                name: "ChangeStatusType");

            migrationBuilder.DropTable(
                name: "RequestStatusType");

            migrationBuilder.RenameColumn(
                name: "RequestStatusTypeId",
                table: "RequestStatus",
                newName: "StatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatus_RequestStatusTypeId",
                table: "RequestStatus",
                newName: "IX_RequestStatus_StatusTypeId");

            migrationBuilder.RenameColumn(
                name: "ChangeStatusTypeId",
                table: "ChangeStatus",
                newName: "StatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ChangeStatus_ChangeStatusTypeId",
                table: "ChangeStatus",
                newName: "IX_ChangeStatus_StatusTypeId");

            migrationBuilder.CreateTable(
                name: "StatusType",
                columns: table => new
                {
                    StatusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusTypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusType", x => x.StatusTypeId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeStatus_StatusType_StatusTypeId",
                table: "ChangeStatus",
                column: "StatusTypeId",
                principalTable: "StatusType",
                principalColumn: "StatusTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_StatusType_StatusTypeId",
                table: "RequestStatus",
                column: "StatusTypeId",
                principalTable: "StatusType",
                principalColumn: "StatusTypeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
