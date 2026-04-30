using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votex.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class liveResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<liveResults>(
                name: "AreLiveResultsOn",
                table: "Votings",
                type: "bit",
                nullable: false,
                defaultValue: false);
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
