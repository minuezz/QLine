using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueTicketIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_QueueEntry_TicketNo",
                table: "QueueEntries");

            migrationBuilder.CreateIndex(
                name: "IX_QueueEntry_TicketNo",
                table: "QueueEntries",
                column: "TicketNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QueueEntry_TicketNo",
                table: "QueueEntries");

            migrationBuilder.CreateIndex(
                name: "UX_QueueEntry_TicketNo",
                table: "QueueEntries",
                column: "TicketNo",
                unique: true);
        }
    }
}
