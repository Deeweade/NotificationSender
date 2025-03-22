using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotificationSender.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultSenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultRedirectEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultSenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultNotificationChannelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSystems_NotificationChannels_DefaultNotificationChannelId",
                        column: x => x.DefaultNotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsumerSystemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemEvents_ClientSystems_ConsumerSystemId",
                        column: x => x.ConsumerSystemId,
                        principalTable: "ClientSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemEventId = table.Column<int>(type: "int", nullable: false),
                    NotificationChannelId = table.Column<int>(type: "int", nullable: false),
                    RecipientAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RedirectNotifications = table.Column<bool>(type: "bit", nullable: false),
                    RedirectAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationRequests_NotificationChannels_NotificationChannelId",
                        column: x => x.NotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationRequests_SystemEvents_SystemEventId",
                        column: x => x.SystemEventId,
                        principalTable: "SystemEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemEventId = table.Column<int>(type: "int", nullable: false),
                    NotificationChannelId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationTemplates_NotificationChannels_NotificationChannelId",
                        column: x => x.NotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationTemplates_SystemEvents_SystemEventId",
                        column: x => x.SystemEventId,
                        principalTable: "SystemEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SentNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificaitonStatusId = table.Column<int>(type: "int", nullable: false),
                    NotificationRequestId = table.Column<int>(type: "int", nullable: false),
                    NotificationChannelId = table.Column<int>(type: "int", nullable: false),
                    ChannelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsedRecipientAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SentNotifications_NotificationChannels_NotificationChannelId",
                        column: x => x.NotificationChannelId,
                        principalTable: "NotificationChannels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SentNotifications_NotificationRequests_NotificationRequestId",
                        column: x => x.NotificationRequestId,
                        principalTable: "NotificationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SentNotifications_NotificationStatus_NotificationStatusId",
                        column: x => x.NotificationStatusId,
                        principalTable: "NotificationStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "NotificationChannels",
                columns: new[] { "Id", "IsEnabled", "Name" },
                values: new object[,]
                {
                    { 1, true, "Mail" },
                    { 2, false, "SMS" }
                });

            migrationBuilder.InsertData(
                table: "ClientSystems",
                columns: new[] { "Id", "DefaultDisplayName", "DefaultNotificationChannelId", "DefaultRedirectEmail", "DefaultSenderEmail", "DefaultSenderPhone", "SystemName" },
                values: new object[,]
                {
                    { 1, "Система отпусков", 1, null, "email@gmail.com", null, "Absence" },
                    { 2, "Мои цели", 1, null, "email@gmail.com", null, "My goals" }
                });

            migrationBuilder.InsertData(
                table: "SystemEvents",
                columns: new[] { "Id", "ConsumerSystemId", "EventName" },
                values: new object[,]
                {
                    { 1, 1, "Сотрудник направил отпуска на согласование" },
                    { 2, 1, "Руководитель отклонил отпуск" },
                    { 3, 1, "Руководитель согласовал отпуск" },
                    { 4, 1, "Руководитель отклонил все отпуска" },
                    { 5, 1, "Руководитель согласовал все отпуска" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientSystems_DefaultNotificationChannelId",
                table: "ClientSystems",
                column: "DefaultNotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_NotificationChannelId",
                table: "NotificationRequests",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationRequests_SystemEventId",
                table: "NotificationRequests",
                column: "SystemEventId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_NotificationChannelId",
                table: "NotificationTemplates",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_SystemEventId",
                table: "NotificationTemplates",
                column: "SystemEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SentNotifications_NotificationChannelId",
                table: "SentNotifications",
                column: "NotificationChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_SentNotifications_NotificationRequestId",
                table: "SentNotifications",
                column: "NotificationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_SentNotifications_NotificationStatusId",
                table: "SentNotifications",
                column: "NotificationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemEvents_ConsumerSystemId",
                table: "SystemEvents",
                column: "ConsumerSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationTemplates");

            migrationBuilder.DropTable(
                name: "SentNotifications");

            migrationBuilder.DropTable(
                name: "NotificationRequests");

            migrationBuilder.DropTable(
                name: "NotificationStatus");

            migrationBuilder.DropTable(
                name: "SystemEvents");

            migrationBuilder.DropTable(
                name: "ClientSystems");

            migrationBuilder.DropTable(
                name: "NotificationChannels");
        }
    }
}
