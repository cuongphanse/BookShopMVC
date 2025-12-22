
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
		public DbSet<Company> Companies { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }	
		public DbSet<ApplicationUser> ApplicationUsers { get; set; } // ke thua Identity nen khong tao table moi
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
			modelBuilder.Entity<Company>().HasData(
				new Company 
				{ 
					Id = 1, 
					Name = "Công nghệ TechWorld", 
					StreetAddress = "123 Đường Láng", 
					City = "Hà Nội", 
					State = "Đống Đa", 
					PostalCode = "100000", 
					PhoneNumber = "0243333444" 
				},
				new Company 
				{ 
					Id = 2, 
					Name = "Thực phẩm sạch GreenFarm", 
					StreetAddress = "456 Nguyễn Huệ", 
					City = "TP. Hồ Chí Minh", 
					State = "Quận 1", 
					PostalCode = "700000", 
					PhoneNumber = "0285555666" 
				},
				new Company 
				{ 
					Id = 3, 
					Name = "Xây dựng An Gia", 
					StreetAddress = "78 Lê Lợi", 
					City = "Đà Nẵng", 
					State = "Hải Châu", 
					PostalCode = "550000", 
					PhoneNumber = "0236777888" 
				},
				new Company 
				{ 
					Id = 4, 
					Name = "Logistics Toàn Cầu", 
					StreetAddress = "101 Trần Phú", 
					City = "Hải Phòng", 
					State = "Ngô Quyền", 
					PostalCode = "180000", 
					PhoneNumber = "0225999000" 
				}
			);

		}

	}
}
