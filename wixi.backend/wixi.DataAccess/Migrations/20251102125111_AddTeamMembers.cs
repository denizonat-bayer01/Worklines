using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wixi_TeamMember",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Experience = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PositionDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PositionTr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PositionEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LocationDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocationTr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LocationEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EducationDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EducationTr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EducationEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BioDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BioTr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BioEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhilosophyDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhilosophyTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhilosophyEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationsDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationsTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecializationsEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguagesDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguagesTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguagesEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementsDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementsTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AchievementsEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_TeamMember", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wixi_TeamMember_DisplayOrder",
                table: "wixi_TeamMember",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_TeamMember_IsActive",
                table: "wixi_TeamMember",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_TeamMember_Slug",
                table: "wixi_TeamMember",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wixi_TeamMember");
        }
    }
}
