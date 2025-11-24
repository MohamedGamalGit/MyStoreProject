using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class createtablesforpermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<int>(
            //    name: "Timestamp",
            //    table: "ActivityLogs",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime2",
            //    oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ActionEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageAction_ActionEntity_ActionEntityId",
                        column: x => x.ActionEntityId,
                        principalTable: "ActionEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PageAction_Page_PageId",
                        column: x => x.PageId,
                        principalTable: "Page",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePageAction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PageActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePageAction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePageAction_PageAction_PageActionId",
                        column: x => x.PageActionId,
                        principalTable: "PageAction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePageAction_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionEntity_Key",
                table: "ActionEntity",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Page_Key",
                table: "Page",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageAction_ActionEntityId",
                table: "PageAction",
                column: "ActionEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PageAction_PageId",
                table: "PageAction",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAction_PageActionId",
                table: "RolePageAction",
                column: "PageActionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePageAction_RoleId_PageActionId",
                table: "RolePageAction",
                columns: new[] { "RoleId", "PageActionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePageAction");

            migrationBuilder.DropTable(
                name: "PageAction");

            migrationBuilder.DropTable(
                name: "ActionEntity");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "ActivityLogs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
