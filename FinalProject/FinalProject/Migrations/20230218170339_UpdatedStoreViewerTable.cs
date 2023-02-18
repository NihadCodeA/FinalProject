using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class UpdatedStoreViewerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoreViewer_Stores_storeId",
                table: "StoreViewer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoreViewer",
                table: "StoreViewer");

            migrationBuilder.RenameTable(
                name: "StoreViewer",
                newName: "StoresViewer");

            migrationBuilder.RenameColumn(
                name: "storeId",
                table: "StoresViewer",
                newName: "StoreId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "StoresViewer",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_StoreViewer_storeId",
                table: "StoresViewer",
                newName: "IX_StoresViewer_StoreId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoresViewer",
                table: "StoresViewer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoresViewer_Stores_StoreId",
                table: "StoresViewer",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StoresViewer_Stores_StoreId",
                table: "StoresViewer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StoresViewer",
                table: "StoresViewer");

            migrationBuilder.RenameTable(
                name: "StoresViewer",
                newName: "StoreViewer");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "StoreViewer",
                newName: "storeId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "StoreViewer",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_StoresViewer_StoreId",
                table: "StoreViewer",
                newName: "IX_StoreViewer_storeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StoreViewer",
                table: "StoreViewer",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreViewer_Stores_storeId",
                table: "StoreViewer",
                column: "storeId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
