using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityApp.Migrations
{
    /// <inheritdoc />
    public partial class AddParking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    Make = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Colour = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicles_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IssuedById = table.Column<string>(type: "text", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_tags_AspNetUsers_IssuedById",
                        column: x => x.IssuedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_tags_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vehicle_tags_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parking_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true),
                    VehicleTagId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    VisitId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntryEntranceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExitEntranceId = table.Column<Guid>(type: "uuid", nullable: true),
                    LoggedById = table.Column<string>(type: "text", nullable: false),
                    EnteredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parking_records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_parking_records_AspNetUsers_LoggedById",
                        column: x => x.LoggedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_parking_records_entrances_EntryEntranceId",
                        column: x => x.EntryEntranceId,
                        principalTable: "entrances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_parking_records_entrances_ExitEntranceId",
                        column: x => x.ExitEntranceId,
                        principalTable: "entrances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_parking_records_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parking_records_vehicle_tags_VehicleTagId",
                        column: x => x.VehicleTagId,
                        principalTable: "vehicle_tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_parking_records_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_parking_records_visits_VisitId",
                        column: x => x.VisitId,
                        principalTable: "visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_passes_EntranceId",
                table: "access_passes",
                column: "EntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_EntryEntranceId",
                table: "parking_records",
                column: "EntryEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_ExitEntranceId",
                table: "parking_records",
                column: "ExitEntranceId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_LoggedById",
                table: "parking_records",
                column: "LoggedById");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_TenantId",
                table: "parking_records",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_VehicleId",
                table: "parking_records",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_VehicleTagId",
                table: "parking_records",
                column: "VehicleTagId");

            migrationBuilder.CreateIndex(
                name: "IX_parking_records_VisitId",
                table: "parking_records",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tags_IssuedById",
                table: "vehicle_tags",
                column: "IssuedById");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tags_TenantId_TagNumber",
                table: "vehicle_tags",
                columns: new[] { "TenantId", "TagNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tags_VehicleId",
                table: "vehicle_tags",
                column: "VehicleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_OwnerId",
                table: "vehicles",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_TenantId",
                table: "vehicles",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_access_passes_entrances_EntranceId",
                table: "access_passes",
                column: "EntranceId",
                principalTable: "entrances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_access_passes_entrances_EntranceId",
                table: "access_passes");

            migrationBuilder.DropTable(
                name: "parking_records");

            migrationBuilder.DropTable(
                name: "vehicle_tags");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_access_passes_EntranceId",
                table: "access_passes");
        }
    }
}
