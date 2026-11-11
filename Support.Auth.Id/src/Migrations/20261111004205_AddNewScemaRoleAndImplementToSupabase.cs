using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Auth.Id.src.Migrations
{
    /// <inheritdoc />
    public partial class AddNewScemaRoleAndImplementToSupabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleName",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleType",
                table: "Roles",
                column: "RoleType",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleType",
                table: "Roles");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);
        }
    }
}
