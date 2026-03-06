using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddContentSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wixi_ContentSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FooterCompanyDescDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterCompanyDescTr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FooterCompanyDescEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterCompanyDescAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    InstagramUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AboutMissionText1De = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutMissionText1Tr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutMissionText1En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMissionText1Ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMissionText2De = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutMissionText2Tr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutMissionText2En = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AboutMissionText2Ar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_ContentSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wixi_ContentSettings");
        }
    }
}
