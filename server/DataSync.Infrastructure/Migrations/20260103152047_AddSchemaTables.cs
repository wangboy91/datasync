using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceId",
                table: "Rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "Rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SchemaTables",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: true),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaTables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SchemaColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemaTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Nullable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaColumns_SchemaTables_SchemaTableId",
                        column: x => x.SchemaTableId,
                        principalTable: "SchemaTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemaColumns_SchemaTableId",
                table: "SchemaColumns",
                column: "SchemaTableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchemaColumns");

            migrationBuilder.DropTable(
                name: "SchemaTables");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Rules");
        }
    }
}
