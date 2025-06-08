using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectThread.Migrations
{
    /// <inheritdoc />
    public partial class receiver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Threads",
                newName: "SenderID");

            migrationBuilder.RenameColumn(
                name: "FriendName",
                table: "Threads",
                newName: "ReceiverName");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Threads",
                newName: "ReceiverID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "Threads",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "ReceiverName",
                table: "Threads",
                newName: "FriendName");

            migrationBuilder.RenameColumn(
                name: "ReceiverID",
                table: "Threads",
                newName: "FriendId");
        }
    }
}
