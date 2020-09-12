using Microsoft.EntityFrameworkCore.Migrations;

namespace UltimateRedditBot.Database.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildId = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubReddits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    IsNsfw = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubReddits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<decimal>(nullable: false),
                    GuildId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Downs = table.Column<int>(nullable: false),
                    Ups = table.Column<int>(nullable: false),
                    IsOver18 = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    PostLink = table.Column<string>(nullable: true),
                    Thumbnail = table.Column<string>(nullable: true),
                    Selftext = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    SubRedditId = table.Column<int>(nullable: false),
                    PostType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_SubReddits_SubRedditId",
                        column: x => x.SubRedditId,
                        principalTable: "SubReddits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubRedditHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastPostId = table.Column<int>(nullable: false),
                    GuildId = table.Column<int>(nullable: false),
                    SubRedditId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubRedditHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubRedditHistories_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubRedditHistories_Posts_LastPostId",
                        column: x => x.LastPostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubRedditHistories_SubReddits_SubRedditId",
                        column: x => x.SubRedditId,
                        principalTable: "SubReddits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_GuildId",
                table: "Channels",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubRedditId",
                table: "Posts",
                column: "SubRedditId");

            migrationBuilder.CreateIndex(
                name: "IX_SubRedditHistories_GuildId",
                table: "SubRedditHistories",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_SubRedditHistories_LastPostId",
                table: "SubRedditHistories",
                column: "LastPostId");

            migrationBuilder.CreateIndex(
                name: "IX_SubRedditHistories_SubRedditId",
                table: "SubRedditHistories",
                column: "SubRedditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "SubRedditHistories");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "SubReddits");
        }
    }
}
