using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalClinic.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalTreatmentSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobalTreatment",
                table: "Treatments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothRecordId",
                table: "TreatmentRecords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "TreatmentRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OdontogramId",
                table: "TreatmentRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentRecords_OdontogramId",
                table: "TreatmentRecords",
                column: "OdontogramId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentRecords_Odontograms_OdontogramId",
                table: "TreatmentRecords",
                column: "OdontogramId",
                principalTable: "Odontograms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentRecords_Odontograms_OdontogramId",
                table: "TreatmentRecords");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentRecords_OdontogramId",
                table: "TreatmentRecords");

            migrationBuilder.DropColumn(
                name: "IsGlobalTreatment",
                table: "Treatments");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "TreatmentRecords");

            migrationBuilder.DropColumn(
                name: "OdontogramId",
                table: "TreatmentRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "ToothRecordId",
                table: "TreatmentRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
