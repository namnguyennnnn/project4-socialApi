using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn4.Migrations
{
    public partial class updatetblnotify : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifies_Friendships_FriendShipId",
                table: "Notifies");

            migrationBuilder.DropIndex(
                name: "IX_Notifies_FriendShipId",
                table: "Notifies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Notifies_FriendShipId",
                table: "Notifies",
                column: "FriendShipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifies_Friendships_FriendShipId",
                table: "Notifies",
                column: "FriendShipId",
                principalTable: "Friendships",
                principalColumn: "FriendshipId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
