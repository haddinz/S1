using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Auth.Id.src.Migrations
{
    /// <inheritdoc />
    public partial class AddForgotPasswordShcema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImmutableEmail",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetExpiresAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Users",
                type: "character varying(225)",
                maxLength: 225,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetUsedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImmutableEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetExpiresAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordResetUsedAt",
                table: "Users");
        }
    }
}
