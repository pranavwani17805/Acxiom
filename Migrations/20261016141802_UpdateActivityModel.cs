using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcxiomCRM.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivityDate",
                table: "Activities",
                newName: "DueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Activities",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Activities",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Activities");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "Activities",
                newName: "ActivityDate");
        }
    }
}
