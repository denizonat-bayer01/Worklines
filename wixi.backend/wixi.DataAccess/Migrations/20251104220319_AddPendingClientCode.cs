using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingClientCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wixi_PendingClientCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmployeeSubmissionId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wixi_PendingClientCode", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wixi_PendingClientCode_ClientCode",
                table: "wixi_PendingClientCode",
                column: "ClientCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_wixi_PendingClientCode_Email",
                table: "wixi_PendingClientCode",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_PendingClientCode_ExpirationDate",
                table: "wixi_PendingClientCode",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_wixi_PendingClientCode_IsUsed",
                table: "wixi_PendingClientCode",
                column: "IsUsed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wixi_PendingClientCode");
        }
    }
}
