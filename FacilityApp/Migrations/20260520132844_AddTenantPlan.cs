using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FacilityApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Plan",
                table: "tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Grandfather all existing tenants as Professional so nothing breaks on upgrade
            migrationBuilder.Sql("UPDATE tenants SET \"Plan\" = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plan",
                table: "tenants");
        }
    }
}
