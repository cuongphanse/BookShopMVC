using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false),
                    Price500 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price500", "Title" },
                values: new object[,]
                {
                    { 1, "Tô Hoài", "Tác phẩm văn học thiếu nhi kinh điển kể về những cuộc phiêu lưu đầy thú vị và bài học làm người của chú Dế Mèn.", "VN-DM001", 60000.0, 55000.0, 50000.0, 45000.0, "Dế Mèn Phiêu Lưu Ký" },
                    { 2, "Nguyễn Nhật Ánh", "Câu chuyện tình đơn phương day dứt của Ngạn dành cho Hà Lan, gắn liền với ngôi làng Đo Đo và những kỷ niệm tuổi thơ.", "VN-MB002", 110000.0, 100000.0, 95000.0, 90000.0, "Mắt Biếc" },
                    { 3, "Vũ Trọng Phụng", "Tiểu thuyết trào phúng xuất sắc đả kích xã hội tư sản thành thị Việt Nam trước năm 1945 qua nhân vật Xuân Tóc Đỏ.", "VN-SD003", 85000.0, 80000.0, 75000.0, 70000.0, "Số Đỏ" },
                    { 4, "Đoàn Giỏi", "Hành trình lưu lạc của cậu bé An tìm cha trong bối cảnh thiên nhiên hoang sơ và con người hào sảng của vùng Nam Bộ.", "VN-DRPN004", 95000.0, 90000.0, 85000.0, 80000.0, "Đất Rừng Phương Nam" },
                    { 5, "Nguyễn Nhật Ánh", "Cuốn sách đưa người đọc trở về thế giới tuổi thơ hồn nhiên, tinh nghịch với những suy nghĩ vừa non nớt vừa triết lý.", "VN-VTT005", 100000.0, 95000.0, 85000.0, 80000.0, "Cho Tôi Xin Một Vé Đi Tuổi Thơ" },
                    { 6, "Nguyễn Ngọc Tư", "Tuyển tập truyện ngắn khắc họa sâu sắc số phận của những người nông dân miền Tây sông nước với nỗi buồn man mác.", "VN-CDBT006", 75000.0, 70000.0, 65000.0, 60000.0, "Cánh Đồng Bất Tận" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
