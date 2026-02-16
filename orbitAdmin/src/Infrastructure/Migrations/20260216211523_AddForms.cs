using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolV01.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormNumber = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AgentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceBrand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormCompanyAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormCompanyId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormCompanyAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormCompanyAttachments_FormCompanies_FormCompanyId",
                        column: x => x.FormCompanyId,
                        principalTable: "FormCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormCompanies_Deleted",
                table: "FormCompanies",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_FormCompanies_FormNumber",
                table: "FormCompanies",
                column: "FormNumber",
                unique: true,
                filter: "[FormNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FormCompanyAttachments_Deleted",
                table: "FormCompanyAttachments",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_FormCompanyAttachments_FormCompanyId",
                table: "FormCompanyAttachments",
                column: "FormCompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormCompanyAttachments");

            migrationBuilder.DropTable(
                name: "FormCompanies");
        }
    }
}
