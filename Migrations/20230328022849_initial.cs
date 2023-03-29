using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ord = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stop = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ord = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stop = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DrawingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ord = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stop = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stage = table.Column<int>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingSteps_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaintingSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ord = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stop = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stage = table.Column<int>(type: "INTEGER", nullable: false),
                    OperationId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaintingSteps_Operations_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Operations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrawingSteps_OperationId",
                table: "DrawingSteps",
                column: "OperationId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ProjectId",
                table: "Operations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PaintingSteps_OperationId",
                table: "PaintingSteps",
                column: "OperationId");

            migrationBuilder.Sql("""
                insert into Projects (Id, Name, Start, Updated, Stop, Ord )
                values ('c01c093a-86bd-44c1-8dd6-49ebf07a5c75', 'A Cool Project', (select date()), (select date()), (select date()), 1 );

                insert into Operations (Id, Name, Start, Updated, Stop, ProjectId, Ord )
                values ('b5d5a757-12f8-48d2-9ee8-18eeb0f025b5', 'Charcoal Operation', (select date()), (select date()), (select date()), 'c01c093a-86bd-44c1-8dd6-49ebf07a5c75', 1 ),
                       ('b5d5a757-12f8-48d2-9ee8-18eeb0f025b2', 'Pencil Operation', (select date()), (select date()), (select date()), 'c01c093a-86bd-44c1-8dd6-49ebf07a5c75', 2 );

                insert into DrawingSteps (Id, Name, Start, Updated, Stop, Stage, OperationId, Ord )
                values ('01ccb7b4-992f-4c44-adbe-44e59b605ee8', 'Charcoal', (select date()), (select date()), (select date()), 0, 'b5d5a757-12f8-48d2-9ee8-18eeb0f025b5', 1 ),
                       ('01ccb7b4-992f-4c44-adbe-44e59b605ee7', 'Dusting', (select date()), (select date()), (select date()), 0, 'b5d5a757-12f8-48d2-9ee8-18eeb0f025b5', 2 ),
                       ('01ccb7b4-992f-4c44-adbe-44e59b605ee6', 'Silhouettes', (select date()), (select date()), (select date()), 0, 'b5d5a757-12f8-48d2-9ee8-18eeb0f025b2', 1 ),
                       ('01ccb7b4-992f-4c44-adbe-44e59b605ee5', 'Pencil', (select date()), (select date()), (select date()), 0, 'b5d5a757-12f8-48d2-9ee8-18eeb0f025b2', 2 );
            """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawingSteps");

            migrationBuilder.DropTable(
                name: "PaintingSteps");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
