using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolV01.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    StartRun = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectTypeId = table.Column<int>(type: "int", nullable: true),
                    SubProjectTypeId = table.Column<int>(type: "int", nullable: true),
                    SubSubProjectTypeId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    ByType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClinicId = table.Column<int>(type: "int", nullable: true),
                    HospitalId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Clinics_ClinicId",
                        column: x => x.ClinicId,
                        principalTable: "Clinics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_Hospitals_HospitalId",
                        column: x => x.HospitalId,
                        principalTable: "Hospitals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_ProjectTypes_ProjectTypeId",
                        column: x => x.ProjectTypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_ProjectTypes_SubProjectTypeId",
                        column: x => x.SubProjectTypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_ProjectTypes_SubSubProjectTypeId",
                        column: x => x.SubSubProjectTypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Devices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ClinicId",
                table: "Devices",
                column: "ClinicId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_Deleted",
                table: "Devices",
                column: "Deleted");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_HospitalId",
                table: "Devices",
                column: "HospitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ProjectTypeId",
                table: "Devices",
                column: "ProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SubProjectTypeId",
                table: "Devices",
                column: "SubProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SubSubProjectTypeId",
                table: "Devices",
                column: "SubSubProjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_SupplierId",
                table: "Devices",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
