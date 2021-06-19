using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Stonks.Server.Migrations
{
    public partial class AddStockAndCompanyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesActors");

            migrationBuilder.DropTable(
                name: "MoviesGenres");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    IdCompany = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cik = table.Column<string>(type: "text", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: true),
                    Listdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Bloomberg = table.Column<string>(type: "text", nullable: true),
                    Figi = table.Column<string>(type: "text", nullable: true),
                    Lei = table.Column<string>(type: "text", nullable: true),
                    Sic = table.Column<long>(type: "bigint", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Industry = table.Column<string>(type: "text", nullable: true),
                    Sector = table.Column<string>(type: "text", nullable: true),
                    Marketcap = table.Column<long>(type: "bigint", nullable: false),
                    Employees = table.Column<long>(type: "bigint", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Ceo = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    ExchangeSymbol = table.Column<string>(type: "text", nullable: true),
                    HqAddress = table.Column<string>(type: "text", nullable: true),
                    HqState = table.Column<string>(type: "text", nullable: true),
                    HqCountry = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Updated = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string[]>(type: "text[]", nullable: true),
                    Similar = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.IdCompany);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Ticker = table.Column<string>(type: "text", nullable: false),
                    IdCompany = table.Column<int>(type: "integer", nullable: false),
                    IdStock = table.Column<string>(type: "text", nullable: true),
                    Market = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    PrimaryExchange = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => new { x.IdCompany, x.Ticker });
                    table.ForeignKey(
                        name: "FK_Stocks_Companies_IdCompany",
                        column: x => x.IdCompany,
                        principalTable: "Companies",
                        principalColumn: "IdCompany",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InTheaters = table.Column<bool>(type: "boolean", nullable: false),
                    Poster = table.Column<string>(type: "text", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Trailer = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Biography = table.Column<string>(type: "text", nullable: true),
                    Character = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Picture = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoviesGenres",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesGenres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_MoviesGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoviesActors",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    Character = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesActors", x => new { x.MovieId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_MoviesActors_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesActors_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesActors_PersonId",
                table: "MoviesActors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesGenres_GenreId",
                table: "MoviesGenres",
                column: "GenreId");
        }
    }
}
