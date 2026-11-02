using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Data.CRMigrations
{
    /// <inheritdoc />
    public partial class Add_Auditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Reservations",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Reservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Reservations",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Reservations",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Customers",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Customers",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Customers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Customers");
        }
    }
}
