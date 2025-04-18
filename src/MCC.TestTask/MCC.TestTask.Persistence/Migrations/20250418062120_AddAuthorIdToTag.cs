using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC.TestTask.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorIdToTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Tags",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AuthorId",
                table: "Tags",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_AuthorId",
                table: "Tags",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_AuthorId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AuthorId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Tags");
        }
    }
}
