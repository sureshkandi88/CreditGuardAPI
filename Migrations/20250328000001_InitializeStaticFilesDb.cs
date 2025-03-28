using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CreditGuardAPI.Data
{
    public partial class InitializeStaticFilesDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaticFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileType = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "TEXT", nullable: false),
                    RelatedEntityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaticFiles_RelatedEntityType_RelatedEntityId",
                table: "StaticFiles",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaticFiles");
        }
    }
}