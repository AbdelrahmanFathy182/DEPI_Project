using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddValueInDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "devices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "devices");
        }
    }
}
