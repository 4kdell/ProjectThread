using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectThread.Migrations
{
    /// <inheritdoc />
    public partial class friendAddupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FriendUserId",
                table: "Friends",
                newName: "FriendUserID");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Friends",
                newName: "FriendID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FriendUserID",
                table: "Friends",
                newName: "FriendUserId");

            migrationBuilder.RenameColumn(
                name: "FriendID",
                table: "Friends",
                newName: "FriendId");
        }
    }
}
