using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffAssignments",
                columns: table => new
                {
                    ServicePointId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffAssignments", x => new { x.ServicePointId, x.UserId });
                    table.ForeignKey(
                        name: "FK_StaffAssignments_AppUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffAssignments_ServicePoints_ServicePointId",
                        column: x => x.ServicePointId,
                        principalTable: "ServicePoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffAssignments_UserId",
                table: "StaffAssignments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffAssignments");
        }
    }
}
