using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votex.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Guid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");
        }
    }
}
