using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DualBuffer.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "calls",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "expiresAt",
                table: "calls",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "calls",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "allocatedChannels",
                table: "calls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "callDuration",
                table: "calls",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "requiredBandwidth",
                table: "calls",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "signalToNoiseRatio",
                table: "calls",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "totalChannels",
                table: "calls",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "allocatedChannels",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "callDuration",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "requiredBandwidth",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "signalToNoiseRatio",
                table: "calls");

            migrationBuilder.DropColumn(
                name: "totalChannels",
                table: "calls");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "calls",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "calls",
                newName: "expiresAt");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "calls",
                newName: "id");
        }
    }
}
