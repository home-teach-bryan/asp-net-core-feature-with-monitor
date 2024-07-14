using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCoreFeatureWithMonitor.Migrations
{
    /// <inheritdoc />
    public partial class rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderDetails",
                newName: "OrderQuantity");

            migrationBuilder.AddColumn<decimal>(
                name: "OrderPrice",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "訂購價格");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPrice",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OrderQuantity",
                table: "OrderDetails",
                newName: "Quantity");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "價格");
        }
    }
}
