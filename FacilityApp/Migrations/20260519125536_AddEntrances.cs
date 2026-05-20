using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEntrances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntryEntranceId",
                table: "visits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExitEntranceId",
                table: "visits",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntranceId",
                table: "blacklist_entries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentEntranceId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EntranceId",
                table: "access_passes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "entrances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entrances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_entrances_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_visits_EntryEntranceId",
                table: "visits",
                column: "EntryEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_visits_ExitEntranceId",
                table: "visits",
                column: "ExitEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_entrances_TenantId",
                table: "entrances",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_visits_entrances_EntryEntranceId",
                table: "visits",
                column: "EntryEntranceId",
                principalTable: "entrances",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_visits_entrances_ExitEntranceId",
                table: "visits",
                column: "ExitEntranceId",
                principalTable: "entrances",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_visits_entrances_EntryEntranceId",
                table: "visits");

            migrationBuilder.DropForeignKey(
                name: "FK_visits_entrances_ExitEntranceId",
                table: "visits");

            migrationBuilder.DropTable(
                name: "entrances");

            migrationBuilder.DropIndex(
                name: "IX_visits_EntryEntranceId",
                table: "visits");

            migrationBuilder.DropIndex(
                name: "IX_visits_ExitEntranceId",
                table: "visits");

            migrationBuilder.DropColumn(
                name: "EntryEntranceId",
                table: "visits");

            migrationBuilder.DropColumn(
                name: "ExitEntranceId",
                table: "visits");

            migrationBuilder.DropColumn(
                name: "EntranceId",
                table: "blacklist_entries");

            migrationBuilder.DropColumn(
                name: "CurrentEntranceId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EntranceId",
                table: "access_passes");
        }
    }
}
