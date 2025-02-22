using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreditGuardAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeparateFirstAndLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PinCode",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Customers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PinCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Customers",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
