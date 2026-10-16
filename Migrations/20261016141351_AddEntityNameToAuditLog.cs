using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcxiomCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityNameToAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableName",
                table: "AuditLogs",
                newName: "EntityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "AuditLogs",
                newName: "TableName");
        }
    }
}
