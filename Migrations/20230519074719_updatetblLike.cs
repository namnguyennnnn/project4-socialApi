using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn4.Migrations
{
    public partial class updatetblLike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isReact",
                table: "Likes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isReact",
                table: "Likes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
