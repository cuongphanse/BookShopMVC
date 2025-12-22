using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addDataCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Hà Nội", "Công nghệ TechWorld", "0243333444", "100000", "Đống Đa", "123 Đường Láng" },
                    { 2, "TP. Hồ Chí Minh", "Thực phẩm sạch GreenFarm", "0285555666", "700000", "Quận 1", "456 Nguyễn Huệ" },
                    { 3, "Đà Nẵng", "Xây dựng An Gia", "0236777888", "550000", "Hải Châu", "78 Lê Lợi" },
                    { 4, "Hải Phòng", "Logistics Toàn Cầu", "0225999000", "180000", "Ngô Quyền", "101 Trần Phú" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
