using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWorthTracker.Database.Migrations
{
    /// <inheritdoc />
    public partial class DefinitionsOneTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assetsdefinitions");

            migrationBuilder.DropTable(
                name: "debtsdefinitions");

            migrationBuilder.CreateTable(
                name: "definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_definitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_definitions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_definitions_user_id",
                table: "definitions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "definitions");

            migrationBuilder.CreateTable(
                name: "assetsdefinitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assetsdefinitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_assetsdefinitions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "debtsdefinitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<int>(type: "INTEGER", nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_debtsdefinitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_debtsdefinitions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assetsdefinitions_user_id",
                table: "assetsdefinitions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_debtsdefinitions_user_id",
                table: "debtsdefinitions",
                column: "user_id");
        }
    }
}
