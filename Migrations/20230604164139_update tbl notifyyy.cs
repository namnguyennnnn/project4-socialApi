using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn4.Migrations
{
    public partial class updatetblnotifyyy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifies_Posts_PostId",
                table: "Notifies");

            migrationBuilder.DropIndex(
                name: "IX_Notifies_PostId",
                table: "Notifies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notifies_PostId",
                table: "Notifies",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifies_Posts_PostId",
                table: "Notifies",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
