using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Olympus.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMisjeModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bogowie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imie = table.Column<string>(type: "text", nullable: false),
                    Domena = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bogowie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bogowie_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Herosi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imie = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herosi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Herosi_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Smiertelnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Imie = table.Column<string>(type: "text", nullable: false),
                    PoziomPoboznosci = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smiertelnik", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Misje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tytul = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PoziomTrudnosci = table.Column<int>(type: "integer", nullable: false),
                    DataUtworzenia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Termin = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Misje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Misje_Bogowie_BogId",
                        column: x => x.BogId,
                        principalTable: "Bogowie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modlitwa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tresc = table.Column<string>(type: "text", nullable: false),
                    Ofiara = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SmiertelnikId = table.Column<int>(type: "integer", nullable: false),
                    BogId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modlitwa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Modlitwa_Bogowie_BogId",
                        column: x => x.BogId,
                        principalTable: "Bogowie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Modlitwa_Smiertelnik_SmiertelnikId",
                        column: x => x.SmiertelnikId,
                        principalTable: "Smiertelnik",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MisjeHerosow",
                columns: table => new
                {
                    MisjaId = table.Column<int>(type: "integer", nullable: false),
                    HerosId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataPrzyjecia = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataWykonania = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Raport = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MisjeHerosow", x => new { x.MisjaId, x.HerosId });
                    table.ForeignKey(
                        name: "FK_MisjeHerosow_Herosi_HerosId",
                        column: x => x.HerosId,
                        principalTable: "Herosi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MisjeHerosow_Misje_MisjaId",
                        column: x => x.MisjaId,
                        principalTable: "Misje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bogowie_UserId",
                table: "Bogowie",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Herosi_UserId",
                table: "Herosi",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Misje_BogId",
                table: "Misje",
                column: "BogId");

            migrationBuilder.CreateIndex(
                name: "IX_MisjeHerosow_HerosId",
                table: "MisjeHerosow",
                column: "HerosId");

            migrationBuilder.CreateIndex(
                name: "IX_Modlitwa_BogId",
                table: "Modlitwa",
                column: "BogId");

            migrationBuilder.CreateIndex(
                name: "IX_Modlitwa_SmiertelnikId",
                table: "Modlitwa",
                column: "SmiertelnikId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MisjeHerosow");

            migrationBuilder.DropTable(
                name: "Modlitwa");

            migrationBuilder.DropTable(
                name: "Herosi");

            migrationBuilder.DropTable(
                name: "Misje");

            migrationBuilder.DropTable(
                name: "Smiertelnik");

            migrationBuilder.DropTable(
                name: "Bogowie");
        }
    }
}
