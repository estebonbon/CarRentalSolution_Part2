using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.CRMigrations
{
    /// <inheritdoc />
    public partial class ReservationUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerReservations",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "INTEGER", nullable: false),
                    ReservationID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReservations", x => new { x.CustomerID, x.ReservationID });
                    table.ForeignKey(
                        name: "FK_CustomerReservations_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerReservations_Reservations_ReservationID",
                        column: x => x.ReservationID,
                        principalTable: "Reservations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReservations_ReservationID",
                table: "CustomerReservations",
                column: "ReservationID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerReservations");
        }
    }
}
