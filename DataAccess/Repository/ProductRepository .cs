using DataAccess.Repository.IRepository;
using DataAcess.Data;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext _db;
		public ProductRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(Product obj)
		{
			var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
			if(objFromDb != null)
			{
				objFromDb.Title = obj.Title;
				objFromDb.Description = obj.Description;
				objFromDb.ISBN = obj.ISBN;
				objFromDb.Author = obj.Author;
				objFromDb.ListPrice = obj.ListPrice;
				objFromDb.Price = obj.Price;
				objFromDb.Price100 = obj.Price100;
				objFromDb.Price500 = obj.Price500;
				objFromDb.CategoryId = obj.CategoryId;
				if(!string.IsNullOrEmpty(obj.ImageUrl))
				{
					objFromDb.ImageUrl = obj.ImageUrl;
				}
			}
		}	
	}
}
