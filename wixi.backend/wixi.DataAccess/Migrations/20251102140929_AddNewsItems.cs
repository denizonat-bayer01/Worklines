using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wixi_NewsItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TitleDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TitleTr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TitleAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExcerptDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcerptTr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExcerptEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcerptAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentDe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentTr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Featured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Slug = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_NewsItem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_Category",
                table: "wixi_NewsItem",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_DisplayOrder",
                table: "wixi_NewsItem",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_Featured",
                table: "wixi_NewsItem",
                column: "Featured");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_IsActive",
                table: "wixi_NewsItem",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_PublishedAt",
                table: "wixi_NewsItem",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_NewsItem_Slug",
                table: "wixi_NewsItem",
                column: "Slug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wixi_NewsItem");
        }
    }
}
