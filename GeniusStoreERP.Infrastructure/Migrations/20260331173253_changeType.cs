using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeniusStoreERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "InvoiceStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "نشط");

            migrationBuilder.UpdateData(
                table: "InvoiceStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "ملغاة");

            migrationBuilder.InsertData(
                table: "PartnerTransactionTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 10, "الغاء فاتورة" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PartnerTransactionTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "InvoiceStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "نقدي");

            migrationBuilder.UpdateData(
                table: "InvoiceStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "آجل");
        }
    }
}
