using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class CreatedSliderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageEn = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MessageAz = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    MessageColor = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    HeaderTextEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HeaderTextAz = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DetailEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DetailAz = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RedirectUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RedirectTextEn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RedirectTextAz = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sliders");
        }
    }
}
