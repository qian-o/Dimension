using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DimensionService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallRoom",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseOwnerID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HouseOwnerDevice = table.Column<int>(type: "int", nullable: false),
                    RoomID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Roommate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallRoom", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChatColumn",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChatID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatColumn", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChatLink",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserID2 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ChatID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLink", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SenderID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    MessageContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVisible = table.Column<int>(type: "int", nullable: false),
                    IsWithdraw = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FriendInfo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Friends = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewFriends = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendInfo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LoginInfo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Effective = table.Column<bool>(type: "bit", nullable: false),
                    UseDevice = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginInfo", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadPortrait = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Profession = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Personalized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    OnLine = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CallRoom_HouseOwnerID_HouseOwnerDevice",
                table: "CallRoom",
                columns: new[] { "HouseOwnerID", "HouseOwnerDevice" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatColumn_UserID",
                table: "ChatColumn",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatLink_UserID1_UserID2",
                table: "ChatLink",
                columns: new[] { "UserID1", "UserID2" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatID",
                table: "ChatMessages",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_LoginInfo_UserID",
                table: "LoginInfo",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallRoom");

            migrationBuilder.DropTable(
                name: "ChatColumn");

            migrationBuilder.DropTable(
                name: "ChatLink");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "FriendInfo");

            migrationBuilder.DropTable(
                name: "LoginInfo");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
