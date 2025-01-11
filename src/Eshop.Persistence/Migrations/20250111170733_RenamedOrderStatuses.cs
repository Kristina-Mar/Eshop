using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eshop.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedOrderStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfItems",
                table: "OrderLine",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderLine",
                newName: "NumberOfItems");
        }
    }
}
