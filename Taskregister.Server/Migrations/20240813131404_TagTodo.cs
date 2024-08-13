using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskregister.Server.Migrations
{
    /// <inheritdoc />
    public partial class TagTodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagTodo_Tags_TagsId",
                table: "TagTodo");

            migrationBuilder.AddForeignKey(
                name: "FK_TagTodo_Tags_TagsId",
                table: "TagTodo",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagTodo_Tags_TagsId",
                table: "TagTodo");

            migrationBuilder.AddForeignKey(
                name: "FK_TagTodo_Tags_TagsId",
                table: "TagTodo",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
