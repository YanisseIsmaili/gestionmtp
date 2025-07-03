using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Yprotect.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Utilisateurs",
                type: "TEXT",
                nullable: false,
                defaultValue: "User");

            migrationBuilder.AddColumn<Guid>(
                name: "UtilisateurId",
                table: "Passwords",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Utilisateurs");

            migrationBuilder.DropColumn(
                name: "UtilisateurId",
                table: "Passwords");
        }
    }
}