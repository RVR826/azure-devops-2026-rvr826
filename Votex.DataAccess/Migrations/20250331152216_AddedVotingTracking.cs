using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Votex.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedVotingTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlreadyVoted",
                columns: table => new
                {
                    AlreadyVotedForId = table.Column<int>(type: "int", nullable: false),
                    AlreadyVotedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlreadyVoted", x => new { x.AlreadyVotedForId, x.AlreadyVotedId });
                    table.ForeignKey(
                        name: "FK_AlreadyVoted_AspNetUsers_AlreadyVotedId",
                        column: x => x.AlreadyVotedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_AlreadyVoted_Votings_AlreadyVotedForId",
                        column: x => x.AlreadyVotedForId,
                        principalTable: "Votings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlreadyVoted_AlreadyVotedId",
                table: "AlreadyVoted",
                column: "AlreadyVotedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlreadyVoted");
        }
    }
}
