
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAcess.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }

		public DbSet<ApplicationUser> applicationUsers { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Hành động", DisplayOrder = 1 },
				new Category { Id = 2, Name = "Alime", DisplayOrder = 2 },
				new Category { Id = 3, Name = "Khoa học", DisplayOrder = 3 }
			);
			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = 1,
					Title = "Dế Mèn Phiêu Lưu Ký",
					Author = "Tô Hoài",
					Description = "Tác phẩm văn học thiếu nhi kinh điển kể về những cuộc phiêu lưu đầy thú vị và bài học làm người của chú Dế Mèn.",
					ISBN = "VN-DM001",
					ListPrice = 60000,
					Price = 55000,
					Price100 = 50000,
					Price500 = 45000,
					CategoryId = 1,
					ImageUrl = ""
				},
				new Product
				{
					Id = 2,
					Title = "Mắt Biếc",
					Author = "Nguyễn Nhật Ánh",
					Description = "Câu chuyện tình đơn phương day dứt của Ngạn dành cho Hà Lan, gắn liền với ngôi làng Đo Đo và những kỷ niệm tuổi thơ.",
					ISBN = "VN-MB002",
					ListPrice = 110000,
					Price = 100000,
					Price100 = 95000,
					Price500 = 90000,
					CategoryId = 2,
					ImageUrl = ""
				},
				new Product
				{
					Id = 3,
					Title = "Số Đỏ",
					Author = "Vũ Trọng Phụng",
					Description = "Tiểu thuyết trào phúng xuất sắc đả kích xã hội tư sản thành thị Việt Nam trước năm 1945 qua nhân vật Xuân Tóc Đỏ.",
					ISBN = "VN-SD003",
					ListPrice = 85000,
					Price = 80000,
					Price100 = 75000,
					Price500 = 70000,
					CategoryId = 3,
					ImageUrl = ""
				},
				new Product
				{
					Id = 4,
					Title = "Đất Rừng Phương Nam",
					Author = "Đoàn Giỏi",
					Description = "Hành trình lưu lạc của cậu bé An tìm cha trong bối cảnh thiên nhiên hoang sơ và con người hào sảng của vùng Nam Bộ.",
					ISBN = "VN-DRPN004",
					ListPrice = 95000,
					Price = 90000,
					Price100 = 85000,
					Price500 = 80000,
					CategoryId = 1,
					ImageUrl = ""
				},
				new Product
				{
					Id = 5,
					Title = "Cho Tôi Xin Một Vé Đi Tuổi Thơ",
					Author = "Nguyễn Nhật Ánh",
					Description = "Cuốn sách đưa người đọc trở về thế giới tuổi thơ hồn nhiên, tinh nghịch với những suy nghĩ vừa non nớt vừa triết lý.",
					ISBN = "VN-VTT005",
					ListPrice = 100000,
					Price = 95000,
					Price100 = 85000,
					Price500 = 80000,
					CategoryId = 1,
					ImageUrl = ""
				},
				new Product
				{
					Id = 6,
					Title = "Cánh Đồng Bất Tận",
					Author = "Nguyễn Ngọc Tư",
					Description = "Tuyển tập truyện ngắn khắc họa sâu sắc số phận của những người nông dân miền Tây sông nước với nỗi buồn man mác.",
					ISBN = "VN-CDBT006",
					ListPrice = 75000,
					Price = 70000,
					Price100 = 65000,
					Price500 = 60000,
					CategoryId = 2,
					ImageUrl = ""
				}
				);
		}

	}
}
