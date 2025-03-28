using Microsoft.EntityFrameworkCore.Migrations;

namespace CreditGuardAPI.Migrations
{
    public partial class UpdateCustomerModelForStaticFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AadhaarPicture",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "Customers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AadhaarPictureId",
                table: "Customers",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AadhaarPictureId",
                table: "Customers");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "Customers",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "AadhaarPicture",
                table: "Customers",
                type: "BLOB",
                nullable: true);
        }
    }
}