using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskregister.Server.Migrations
{
    /// <inheritdoc />
    public partial class TagsRenameColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Tags",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                newName: "IX_Tags_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tags",
                newName: "Value");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                newName: "IX_Tags_Value");
        }
    }
}
