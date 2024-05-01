using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class addchangerequeststatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_Request_RequestId",
                table: "RequestStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_Status_StatusesStatusId",
                table: "RequestStatus");

            migrationBuilder.DropTable(
                name: "ChangeDetailStatus");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestStatus",
                table: "RequestStatus");

            migrationBuilder.RenameColumn(
                name: "StatusesStatusId",
                table: "RequestStatus",
                newName: "StatusTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatus_StatusesStatusId",
                table: "RequestStatus",
                newName: "IX_RequestStatus_StatusTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "RequestStatus",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatusId",
                table: "RequestStatus",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusDate",
                table: "RequestStatus",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SubmitUser",
                table: "RequestStatus",
                type: "nvarchar(150)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestStatus",
                table: "RequestStatus",
                column: "RequestStatusId");

            migrationBuilder.CreateTable(
                name: "ChangeStatus",
                columns: table => new
                {
                    ChangeStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChangeDetailId = table.Column<int>(type: "int", nullable: true),
                    SubmitUser = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeStatus", x => x.ChangeStatusId);
                    table.ForeignKey(
                        name: "FK_ChangeStatus_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestRequestStatus",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RequestStatusesRequestStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestRequestStatus", x => new { x.RequestId, x.RequestStatusesRequestStatusId });
                    table.ForeignKey(
                        name: "FK_RequestRequestStatus_RequestStatus_RequestStatusesRequestStatusId",
                        column: x => x.RequestStatusesRequestStatusId,
                        principalTable: "RequestStatus",
                        principalColumn: "RequestStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestRequestStatus_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeDetailChangeStatus",
                columns: table => new
                {
                    ChangeDetailId = table.Column<int>(type: "int", nullable: false),
                    ChangeStatusesChangeStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeDetailChangeStatus", x => new { x.ChangeDetailId, x.ChangeStatusesChangeStatusId });
                    table.ForeignKey(
                        name: "FK_ChangeDetailChangeStatus_ChangeDetail_ChangeDetailId",
                        column: x => x.ChangeDetailId,
                        principalTable: "ChangeDetail",
                        principalColumn: "ChangeDetailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeDetailChangeStatus_ChangeStatus_ChangeStatusesChangeStatusId",
                        column: x => x.ChangeStatusesChangeStatusId,
                        principalTable: "ChangeStatus",
                        principalColumn: "ChangeStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeDetailChangeStatus_ChangeStatusesChangeStatusId",
                table: "ChangeDetailChangeStatus",
                column: "ChangeStatusesChangeStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeStatus_StatusTypeId",
                table: "ChangeStatus",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestRequestStatus_RequestStatusesRequestStatusId",
                table: "RequestRequestStatus",
                column: "RequestStatusesRequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_StatusType_StatusTypeId",
                table: "RequestStatus",
                column: "StatusTypeId",
                principalTable: "StatusType",
                principalColumn: "StatusTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestStatus_StatusType_StatusTypeId",
                table: "RequestStatus");

            migrationBuilder.DropTable(
                name: "ChangeDetailChangeStatus");

            migrationBuilder.DropTable(
                name: "RequestRequestStatus");

            migrationBuilder.DropTable(
                name: "ChangeStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestStatus",
                table: "RequestStatus");

            migrationBuilder.DropColumn(
                name: "RequestStatusId",
                table: "RequestStatus");

            migrationBuilder.DropColumn(
                name: "StatusDate",
                table: "RequestStatus");

            migrationBuilder.DropColumn(
                name: "SubmitUser",
                table: "RequestStatus");

            migrationBuilder.RenameColumn(
                name: "StatusTypeId",
                table: "RequestStatus",
                newName: "StatusesStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestStatus_StatusTypeId",
                table: "RequestStatus",
                newName: "IX_RequestStatus_StatusesStatusId");

            migrationBuilder.AlterColumn<int>(
                name: "RequestId",
                table: "RequestStatus",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestStatus",
                table: "RequestStatus",
                columns: new[] { "RequestId", "StatusesStatusId" });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false),
                    ChangeDetailId = table.Column<int>(type: "int", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmitUser = table.Column<string>(type: "nvarchar(150)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusId);
                    table.ForeignKey(
                        name: "FK_Status_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChangeDetailStatus",
                columns: table => new
                {
                    ChangeDetailId = table.Column<int>(type: "int", nullable: false),
                    StatusesStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeDetailStatus", x => new { x.ChangeDetailId, x.StatusesStatusId });
                    table.ForeignKey(
                        name: "FK_ChangeDetailStatus_ChangeDetail_ChangeDetailId",
                        column: x => x.ChangeDetailId,
                        principalTable: "ChangeDetail",
                        principalColumn: "ChangeDetailId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChangeDetailStatus_Status_StatusesStatusId",
                        column: x => x.StatusesStatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChangeDetailStatus_StatusesStatusId",
                table: "ChangeDetailStatus",
                column: "StatusesStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_StatusTypeId",
                table: "Status",
                column: "StatusTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_Request_RequestId",
                table: "RequestStatus",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestStatus_Status_StatusesStatusId",
                table: "RequestStatus",
                column: "StatusesStatusId",
                principalTable: "Status",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
