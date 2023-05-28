using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn4.Migrations
{
    public partial class updateTblComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalComment",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalComment",
                table: "Posts");
        }
    }
}
