using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class linkstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestStatus_Request_RequestId",
                table: "RequestRequestStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRequestStatus",
                table: "RequestRequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_RequestRequestStatus_RequestStatusesRequestStatusId",
                table: "RequestRequestStatus");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "RequestRequestStatus",
                newName: "RequestsRequestId");

            migrationBuilder.AlterColumn<string>(
                name: "Change",
                table: "Request",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatusId",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ChangeText",
                table: "ChangeDetail",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRequestStatus",
                table: "RequestRequestStatus",
                columns: new[] { "RequestStatusesRequestStatusId", "RequestsRequestId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestRequestStatus_RequestsRequestId",
                table: "RequestRequestStatus",
                column: "RequestsRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestStatus_Request_RequestsRequestId",
                table: "RequestRequestStatus",
                column: "RequestsRequestId",
                principalTable: "Request",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestRequestStatus_Request_RequestsRequestId",
                table: "RequestRequestStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestRequestStatus",
                table: "RequestRequestStatus");

            migrationBuilder.DropIndex(
                name: "IX_RequestRequestStatus_RequestsRequestId",
                table: "RequestRequestStatus");

            migrationBuilder.DropColumn(
                name: "RequestStatusId",
                table: "Request");

            migrationBuilder.RenameColumn(
                name: "RequestsRequestId",
                table: "RequestRequestStatus",
                newName: "RequestId");

            migrationBuilder.AlterColumn<string>(
                name: "Change",
                table: "Request",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ChangeText",
                table: "ChangeDetail",
                type: "nvarchar(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestRequestStatus",
                table: "RequestRequestStatus",
                columns: new[] { "RequestId", "RequestStatusesRequestStatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestRequestStatus_RequestStatusesRequestStatusId",
                table: "RequestRequestStatus",
                column: "RequestStatusesRequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestRequestStatus_Request_RequestId",
                table: "RequestRequestStatus",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
