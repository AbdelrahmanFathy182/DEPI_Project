using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeDashboard.Migrations
{
    /// <inheritdoc />
    public partial class LoginMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Accounts",
                newName: "HashPassword");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HashPassword",
                table: "Accounts",
                newName: "Password");
        }
    }
}
