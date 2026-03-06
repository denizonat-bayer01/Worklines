using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFormSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wixi_ContactFormSubmission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Education = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    WorkExperience = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GermanLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EnglishLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Interest = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Timeline = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrivacyConsent = table.Column<bool>(type: "bit", nullable: false),
                    Newsletter = table.Column<bool>(type: "bit", nullable: false),
                    RequestIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EmailLogId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_ContactFormSubmission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wixi_EmployeeFormSubmission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    Salutation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Experience = table.Column<int>(type: "int", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GermanLevel = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CvFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CvFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CvFileSize = table.Column<long>(type: "bigint", nullable: true),
                    SpecialRequests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EmailLogId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_EmployeeFormSubmission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wixi_EmployerFormSubmission",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CompanyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompanySize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Positions = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialRequests = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EmailLogId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_EmployerFormSubmission", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wixi_ContactFormSubmission_CreatedAt",
                table: "wixi_ContactFormSubmission",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_ContactFormSubmission_Email",
                table: "wixi_ContactFormSubmission",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployeeFormSubmission_CreatedAt",
                table: "wixi_EmployeeFormSubmission",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployeeFormSubmission_Email",
                table: "wixi_EmployeeFormSubmission",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployeeFormSubmission_Profession",
                table: "wixi_EmployeeFormSubmission",
                column: "Profession");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployerFormSubmission_CreatedAt",
                table: "wixi_EmployerFormSubmission",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployerFormSubmission_Email",
                table: "wixi_EmployerFormSubmission",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_EmployerFormSubmission_Industry",
                table: "wixi_EmployerFormSubmission",
                column: "Industry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wixi_ContactFormSubmission");

            migrationBuilder.DropTable(
                name: "wixi_EmployeeFormSubmission");

            migrationBuilder.DropTable(
                name: "wixi_EmployerFormSubmission");
        }
    }
}
