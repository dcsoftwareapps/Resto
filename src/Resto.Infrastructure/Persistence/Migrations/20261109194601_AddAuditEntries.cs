using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resto.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: true),
                    OccurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Module = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Detail = table.Column<string>(type: "text", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    BeforeJson = table.Column<string>(type: "jsonb", nullable: true),
                    AfterJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_entries_restaurants_RestaurantId_OrganizationId",
                        columns: x => new { x.RestaurantId, x.OrganizationId },
                        principalTable: "restaurants",
                        principalColumns: new[] { "Id", "OrganizationId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_entries_EntityId",
                table: "audit_entries",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_audit_entries_RestaurantId_OccurredAt",
                table: "audit_entries",
                columns: new[] { "RestaurantId", "OccurredAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_audit_entries_RestaurantId_OrganizationId",
                table: "audit_entries",
                columns: new[] { "RestaurantId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_entries_UserId",
                table: "audit_entries",
                column: "UserId");

            migrationBuilder.Sql("""
                CREATE FUNCTION reject_audit_entries_mutation() RETURNS trigger
                LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION 'Audit entries are append-only.' USING ERRCODE = '55000';
                END;
                $$;

                CREATE TRIGGER audit_entries_append_only
                BEFORE UPDATE OR DELETE OR TRUNCATE ON audit_entries
                FOR EACH STATEMENT EXECUTE FUNCTION reject_audit_entries_mutation();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_entries");
            migrationBuilder.Sql("DROP FUNCTION reject_audit_entries_mutation();");
        }
    }
}
