using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader obj);
		// Thêm tham số optional cho paymentStatus
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
	}
}
