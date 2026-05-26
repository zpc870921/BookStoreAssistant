using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bookstoreagent.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentSessionState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentSessionState",
                table: "Conversations",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Conversations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentSessionState",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Conversations");
        }
    }
}
