using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Migrations;
using Taskregister.Server.Todos.Contstants;

#nullable disable

namespace Taskregister.Server.Migrations
{
    /// <inheritdoc />
    public partial class TaskEntityAddConversionToStateEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
            
            migrationBuilder.Sql($"UPDATE Tasks SET State = '{Todos.Contstants.State.New.ToString()}' WHERE State = '0'");
            migrationBuilder.Sql($"UPDATE Tasks SET State = '{Todos.Contstants.State.Completed.ToString()}' WHERE State = '1'");
            migrationBuilder.Sql($"UPDATE Tasks SET State = '{Todos.Contstants.State.Resumed.ToString()}' WHERE State = '2'");
            //Todos.Contstants.State.New.ToString()
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
            
            migrationBuilder.Sql($"UPDATE Tasks SET  State = '0' WHERE State = '{Todos.Contstants.State.New.ToString()}'");
            migrationBuilder.Sql($"UPDATE Tasks SET  State = '1' WHERE State = '{Todos.Contstants.State.Completed.ToString()}'");
            migrationBuilder.Sql($"UPDATE Tasks SET  State = '2' WHERE State = '{Todos.Contstants.State.Resumed.ToString()}'");
        }
    }
}
