using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskregister.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateAtAndRenameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChangeStateRationale",
                table: "Tasks",
                newName: "ChangeEndDateRationale");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "ChangeEndDateRationale",
                table: "Tasks",
                newName: "ChangeStateRationale");
        }
    }
}
