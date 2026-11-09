using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Resto.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_roles", x => x.Id);
                    table.UniqueConstraint("AK_roles_Id_OrganizationId_RestaurantId", x => new { x.Id, x.OrganizationId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_roles_restaurants_RestaurantId_OrganizationId",
                        columns: x => new { x.RestaurantId, x.OrganizationId },
                        principalTable: "restaurants",
                        principalColumns: new[] { "Id", "OrganizationId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Permission = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_role_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_role_permissions_restaurants_RestaurantId_OrganizationId",
                        columns: x => new { x.RestaurantId, x.OrganizationId },
                        principalTable: "restaurants",
                        principalColumns: new[] { "Id", "OrganizationId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_role_permissions_roles_RoleId_OrganizationId_RestaurantId",
                        columns: x => new { x.RoleId, x.OrganizationId, x.RestaurantId },
                        principalTable: "roles",
                        principalColumns: new[] { "Id", "OrganizationId", "RestaurantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Initials = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PinHash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_restaurants_RestaurantId_OrganizationId",
                        columns: x => new { x.RestaurantId, x.OrganizationId },
                        principalTable: "restaurants",
                        principalColumns: new[] { "Id", "OrganizationId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_roles_RoleId_OrganizationId_RestaurantId",
                        columns: x => new { x.RoleId, x.OrganizationId, x.RestaurantId },
                        principalTable: "roles",
                        principalColumns: new[] { "Id", "OrganizationId", "RestaurantId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RestaurantId_OrganizationId",
                table: "role_permissions",
                columns: new[] { "RestaurantId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleId_OrganizationId_RestaurantId",
                table: "role_permissions",
                columns: new[] { "RoleId", "OrganizationId", "RestaurantId" });

            migrationBuilder.CreateIndex(
                name: "IX_role_permissions_RoleId_Permission",
                table: "role_permissions",
                columns: new[] { "RoleId", "Permission" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_RestaurantId_OrganizationId",
                table: "roles",
                columns: new[] { "RestaurantId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_RestaurantId_OrganizationId",
                table: "users",
                columns: new[] { "RestaurantId", "OrganizationId" });

            migrationBuilder.CreateIndex(
                name: "IX_users_RoleId_OrganizationId_RestaurantId",
                table: "users",
                columns: new[] { "RoleId", "OrganizationId", "RestaurantId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
