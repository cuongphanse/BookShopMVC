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
	public class ShoppingCartRepository : Repository<ShoppingCart> , IShoppingCartRepository
	{
		private readonly ApplicationDbContext _db;
		public ShoppingCartRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void Update(ShoppingCart obj)
		{
			_db.ShoppingCarts.Update(obj);
		}
	}
}
