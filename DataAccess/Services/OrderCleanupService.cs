using DataAccess.Repository.IRepository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataAccess.Services
{
	public class OrderCleanupService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IUnitOfWork _unitOfWork;
		public OrderCleanupService(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
		{
			_serviceProvider = serviceProvider;
			_unitOfWork = unitOfWork;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

					// Lấy các đơn hàng Pending quá 15 phút
					var timeThreshold = DateTime.Now.AddMinutes(-1);
					var expiredOrders = unitOfWork.OrderHeader.GetAll(u =>
						u.OrderStatus == SD.StatusPending &&
						u.OrderDate < timeThreshold).ToList();

					foreach (var order in expiredOrders)
					{
						order.OrderStatus = SD.StatusCancelled;
						order.PaymentStatus = SD.PaymentStatusRejected; // Hoặc một trạng thái "Hết hạn"
					}

					if (expiredOrders.Any()) _unitOfWork.Save();
				}

				// Quét lại mỗi phút 1 lần
				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}
