using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addColumneSealChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "SealChanges",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SealChanges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SealChanges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SealChanges");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SealChanges");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SealChanges",
                newName: "id");
        }
    }
}
