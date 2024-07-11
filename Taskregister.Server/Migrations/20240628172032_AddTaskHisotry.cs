using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskregister.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskHisotry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "History",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "History",
                table: "Tasks");
        }
    }
}
