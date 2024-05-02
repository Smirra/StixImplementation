using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vulnerabilities.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeverityAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MagnussonSeverity",
                table: "Vulnerabilities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Vulnerabilities",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_Created",
                table: "Vulnerabilities",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_MagnussonSeverity",
                table: "Vulnerabilities",
                column: "MagnussonSeverity");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_Modified",
                table: "Vulnerabilities",
                column: "Modified");

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_Status",
                table: "Vulnerabilities",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilities_Created",
                table: "Vulnerabilities");

            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilities_MagnussonSeverity",
                table: "Vulnerabilities");

            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilities_Modified",
                table: "Vulnerabilities");

            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilities_Status",
                table: "Vulnerabilities");

            migrationBuilder.DropColumn(
                name: "MagnussonSeverity",
                table: "Vulnerabilities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vulnerabilities");
        }
    }
}
