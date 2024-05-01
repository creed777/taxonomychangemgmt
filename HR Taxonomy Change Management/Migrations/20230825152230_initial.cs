using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR_Taxonomy_Change_Management.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestType",
                columns: table => new
                {
                    RequestTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestType", x => x.RequestTypeId);
                });

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

            migrationBuilder.CreateTable(
                name: "Taxonomy",
                columns: table => new
                {
                    TaxonomyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxonomy", x => x.TaxonomyId);
                    table.ForeignKey(
                        name: "FK_Taxonomy_Taxonomy_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Taxonomy",
                        principalColumn: "TaxonomyId");
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Justification = table.Column<string>(type: "text", nullable: false),
                    Change = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    SubmitUser = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    SubmitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyUser = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    LegacyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Request_RequestType_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestType",
                        principalColumn: "RequestTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    ChangeDetailId = table.Column<int>(type: "int", nullable: true),
                    SubmitUser = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false)
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
                name: "ChangeDetail",
                columns: table => new
                {
                    ChangeDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    ChangeText = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    CurrentL1 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    CurrentL2 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    CurrentL3 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    CurrentL4 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    CurrentL5 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    NewL1 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    NewL2 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    NewL3 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    NewL4 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    NewL5 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    SubmitUser = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    SubmitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifyUser = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LegacyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeDetail", x => x.ChangeDetailId);
                    table.ForeignKey(
                        name: "FK_ChangeDetail_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatus",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    StatusesStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatus", x => new { x.RequestId, x.StatusesStatusId });
                    table.ForeignKey(
                        name: "FK_RequestStatus_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestStatus_Status_StatusesStatusId",
                        column: x => x.StatusesStatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId",
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
                name: "IX_ChangeDetail_RequestId",
                table: "ChangeDetail",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeDetailStatus_StatusesStatusId",
                table: "ChangeDetailStatus",
                column: "StatusesStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestTypeId",
                table: "Request",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestStatus_StatusesStatusId",
                table: "RequestStatus",
                column: "StatusesStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_StatusTypeId",
                table: "Status",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxonomy_ParentId",
                table: "Taxonomy",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChangeDetailStatus");

            migrationBuilder.DropTable(
                name: "RequestStatus");

            migrationBuilder.DropTable(
                name: "Taxonomy");

            migrationBuilder.DropTable(
                name: "ChangeDetail");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropTable(
                name: "StatusType");

            migrationBuilder.DropTable(
                name: "RequestType");
        }
    }
}
