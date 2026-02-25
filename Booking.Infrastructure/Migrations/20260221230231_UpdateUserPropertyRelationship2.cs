using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPropertyRelationship2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Owners_OwnerId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_UserId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_OwnerId",
                table: "Properties",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_OwnerId",
                table: "Properties");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Owners_OwnerId",
                table: "Properties",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
