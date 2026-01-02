using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ScriptiumBackend.Migrations
{
    /// <inheritdoc />
    public partial class cache_meachanism : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "u_cache",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    data = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_u_cache", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "u_cache_record",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cache_id = table.Column<long>(type: "bigint", nullable: false),
                    fetched_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_u_cache_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_u_cache_record_u_cache_cache_id",
                        column: x => x.cache_id,
                        principalTable: "u_cache",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_u_cache_url",
                table: "u_cache",
                column: "url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_u_cache_record_cache_id",
                table: "u_cache_record",
                column: "cache_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "u_cache_record");

            migrationBuilder.DropTable(
                name: "u_cache");
        }
    }
}
