using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wixi.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddContactInfoToContentSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressGermany",
                table: "wixi_ContentSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressTurkeyIstanbul",
                table: "wixi_ContentSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressTurkeyMersin",
                table: "wixi_ContentSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "wixi_ContentSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                table: "wixi_ContentSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressGermany",
                table: "wixi_ContentSettings");

            migrationBuilder.DropColumn(
                name: "AddressTurkeyIstanbul",
                table: "wixi_ContentSettings");

            migrationBuilder.DropColumn(
                name: "AddressTurkeyMersin",
                table: "wixi_ContentSettings");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "wixi_ContentSettings");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                table: "wixi_ContentSettings");
        }
    }
}
