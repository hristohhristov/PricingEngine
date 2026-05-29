using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PricingEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("a1a1a1a1-0000-0000-0000-000000000001"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProductConfigurations",
                columns: new[] { "Id", "ConfigData", "DeletedAt", "ProductCode", "ValidFrom", "ValidTo" },
                values: new object[] { new Guid("a1a1a1a1-0000-0000-0000-000000000001"), "{\"BaseTariff\":0.005,\"FixedFee\":25.00}", null, "HOME_V1", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2100, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
