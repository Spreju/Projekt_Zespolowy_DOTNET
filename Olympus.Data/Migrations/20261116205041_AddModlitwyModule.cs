using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Olympus.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModlitwyModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modlitwa_Bogowie_BogId",
                table: "Modlitwa");

            migrationBuilder.DropForeignKey(
                name: "FK_Modlitwa_Smiertelnik_SmiertelnikId",
                table: "Modlitwa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Smiertelnik",
                table: "Smiertelnik");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modlitwa",
                table: "Modlitwa");

            migrationBuilder.DropIndex(
                name: "IX_Modlitwa_BogId",
                table: "Modlitwa");

            migrationBuilder.RenameTable(
                name: "Smiertelnik",
                newName: "Smiertelnicy");

            migrationBuilder.RenameTable(
                name: "Modlitwa",
                newName: "Modlitwy");

            migrationBuilder.RenameIndex(
                name: "IX_Modlitwa_SmiertelnikId",
                table: "Modlitwy",
                newName: "IX_Modlitwy_SmiertelnikId");

            migrationBuilder.AlterColumn<string>(
                name: "Imie",
                table: "Smiertelnicy",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Smiertelnicy",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tresc",
                table: "Modlitwy",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Modlitwy",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Ofiara",
                table: "Modlitwy",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRozpatrzenia",
                table: "Modlitwy",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataZlozenia",
                table: "Modlitwy",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OdpowiedzBoga",
                table: "Modlitwy",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZalacznikNazwaOryginalna",
                table: "Modlitwy",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZalacznikNazwaPliku",
                table: "Modlitwy",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZalacznikTyp",
                table: "Modlitwy",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Smiertelnicy",
                table: "Smiertelnicy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modlitwy",
                table: "Modlitwy",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Smiertelnicy_UserId",
                table: "Smiertelnicy",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modlitwy_BogId_Status",
                table: "Modlitwy",
                columns: new[] { "BogId", "Status" });

            migrationBuilder.AddForeignKey(
                name: "FK_Modlitwy_Bogowie_BogId",
                table: "Modlitwy",
                column: "BogId",
                principalTable: "Bogowie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Modlitwy_Smiertelnicy_SmiertelnikId",
                table: "Modlitwy",
                column: "SmiertelnikId",
                principalTable: "Smiertelnicy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Smiertelnicy_AspNetUsers_UserId",
                table: "Smiertelnicy",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modlitwy_Bogowie_BogId",
                table: "Modlitwy");

            migrationBuilder.DropForeignKey(
                name: "FK_Modlitwy_Smiertelnicy_SmiertelnikId",
                table: "Modlitwy");

            migrationBuilder.DropForeignKey(
                name: "FK_Smiertelnicy_AspNetUsers_UserId",
                table: "Smiertelnicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Smiertelnicy",
                table: "Smiertelnicy");

            migrationBuilder.DropIndex(
                name: "IX_Smiertelnicy_UserId",
                table: "Smiertelnicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Modlitwy",
                table: "Modlitwy");

            migrationBuilder.DropIndex(
                name: "IX_Modlitwy_BogId_Status",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Smiertelnicy");

            migrationBuilder.DropColumn(
                name: "DataRozpatrzenia",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "DataZlozenia",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "OdpowiedzBoga",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "ZalacznikNazwaOryginalna",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "ZalacznikNazwaPliku",
                table: "Modlitwy");

            migrationBuilder.DropColumn(
                name: "ZalacznikTyp",
                table: "Modlitwy");

            migrationBuilder.RenameTable(
                name: "Smiertelnicy",
                newName: "Smiertelnik");

            migrationBuilder.RenameTable(
                name: "Modlitwy",
                newName: "Modlitwa");

            migrationBuilder.RenameIndex(
                name: "IX_Modlitwy_SmiertelnikId",
                table: "Modlitwa",
                newName: "IX_Modlitwa_SmiertelnikId");

            migrationBuilder.AlterColumn<string>(
                name: "Imie",
                table: "Smiertelnik",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Tresc",
                table: "Modlitwa",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Modlitwa",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Ofiara",
                table: "Modlitwa",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Smiertelnik",
                table: "Smiertelnik",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Modlitwa",
                table: "Modlitwa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Modlitwa_BogId",
                table: "Modlitwa",
                column: "BogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modlitwa_Bogowie_BogId",
                table: "Modlitwa",
                column: "BogId",
                principalTable: "Bogowie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Modlitwa_Smiertelnik_SmiertelnikId",
                table: "Modlitwa",
                column: "SmiertelnikId",
                principalTable: "Smiertelnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
