using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectThread.Migrations
{
    /// <inheritdoc />
    public partial class friendAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Friends",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "FriendUserId",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IsDeleted",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "FriendUserId",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Friends");
        }
    }
}
