using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoxOffice.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Performances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PerformanceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Performances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Performances_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Performances_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PerformanceId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Performances_PerformanceId",
                        column: x => x.PerformanceId,
                        principalTable: "Performances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    OperationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSold = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketBookings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketBookings_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "GenreName" },
                values: new object[,]
                {
                    { 1, "Drama" },
                    { 2, "Comedy" },
                    { 3, "Musical" },
                    { 4, "Tragedy" }
                });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "BirthDate", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, new DateTime(1985, 4, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "William", "Shakespeare" },
                    { 2, new DateTime(1990, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anton", "Chekhov" },
                    { 3, new DateTime(1982, 10, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Oscar", "Wilde" },
                    { 4, new DateTime(1978, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Arthur", "Miller" },
                    { 5, new DateTime(1995, 5, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Emma", "Johnson" },
                    { 6, new DateTime(1988, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "James", "Smith" },
                    { 7, new DateTime(2000, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Olivia", "Brown" },
                    { 8, new DateTime(1992, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Daniel", "Taylor" }
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "PersonId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "PersonId" },
                values: new object[,]
                {
                    { 1, 5 },
                    { 2, 6 },
                    { 3, 7 },
                    { 4, 8 }
                });

            migrationBuilder.InsertData(
                table: "Performances",
                columns: new[] { "Id", "AuthorId", "EventDate", "GenreId", "PerformanceName" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 11, 5, 19, 0, 0, 0, DateTimeKind.Unspecified), 4, "Hamlet" },
                    { 2, 2, new DateTime(2026, 11, 12, 18, 30, 0, 0, DateTimeKind.Unspecified), 1, "The Cherry Orchard" },
                    { 3, 3, new DateTime(2026, 11, 20, 19, 0, 0, 0, DateTimeKind.Unspecified), 2, "The Importance of Being Earnest" },
                    { 4, 4, new DateTime(2026, 12, 1, 19, 30, 0, 0, DateTimeKind.Unspecified), 1, "Death of a Salesman" },
                    { 5, 1, new DateTime(2026, 12, 10, 18, 0, 0, 0, DateTimeKind.Unspecified), 2, "A Midsummer Night's Dream" },
                    { 6, 4, new DateTime(2026, 12, 18, 19, 0, 0, 0, DateTimeKind.Unspecified), 4, "The Crucible" }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "Id", "PerformanceId", "Price", "SeatName" },
                values: new object[,]
                {
                    { 1, 1, 250m, "A1" },
                    { 2, 1, 350m, "B4" },
                    { 3, 2, 400m, "A2" },
                    { 4, 2, 500m, "B5" },
                    { 5, 3, 300m, "A3" },
                    { 6, 3, 400m, "B6" },
                    { 7, 4, 450m, "A4" },
                    { 8, 4, 550m, "B7" },
                    { 9, 5, 350m, "A5" },
                    { 10, 5, 450m, "B8" },
                    { 11, 6, 500m, "A6" },
                    { 12, 6, 600m, "B9" }
                });

            migrationBuilder.InsertData(
                table: "TicketBookings",
                columns: new[] { "Id", "CustomerId", "IsSold", "OperationDate", "TicketId" },
                values: new object[,]
                {
                    { 1, 1, true, new DateTime(2026, 10, 20, 12, 15, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, true, new DateTime(2026, 10, 21, 9, 40, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 3, 3, false, new DateTime(2026, 10, 22, 16, 5, 0, 0, DateTimeKind.Unspecified), 4 },
                    { 4, 4, true, new DateTime(2026, 10, 23, 11, 20, 0, 0, DateTimeKind.Unspecified), 6 },
                    { 5, 1, false, new DateTime(2026, 10, 25, 18, 45, 0, 0, DateTimeKind.Unspecified), 8 },
                    { 6, 2, true, new DateTime(2026, 10, 27, 14, 10, 0, 0, DateTimeKind.Unspecified), 11 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_PersonId",
                table: "Authors",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PersonId",
                table: "Customers",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Performances_AuthorId",
                table: "Performances",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Performances_GenreId",
                table: "Performances",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBookings_CustomerId",
                table: "TicketBookings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketBookings_TicketId",
                table: "TicketBookings",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PerformanceId",
                table: "Tickets",
                column: "PerformanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketBookings");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Performances");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
