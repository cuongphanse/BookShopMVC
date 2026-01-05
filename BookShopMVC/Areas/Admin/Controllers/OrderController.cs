using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModel;
using Utility;

namespace BookShopMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty] // map du lieu tra ve
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Detail(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(OrderVM);
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
		public IActionResult UpdateOrderDetail()
		{
			OrderHeader orderFromDb = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVM.OrderHeader.Id);
			orderFromDb.Name = OrderVM.OrderHeader.Name;
			orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderFromDb.City = OrderVM.OrderHeader.City;
			orderFromDb.State = OrderVM.OrderHeader.State;
			orderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
			if (!string.IsNullOrEmpty(orderFromDb.Carrier))
			{
				orderFromDb.Carrier = orderFromDb.Carrier;
			}
			if (!string.IsNullOrEmpty(orderFromDb.TrackingNumber))
			{
				orderFromDb.TrackingNumber = orderFromDb.TrackingNumber;
			}
			_unitOfWork.OrderHeader.Update(orderFromDb);
			_unitOfWork.Save();
			TempData["Success"] = "Cập nhật thành công";
			return RedirectToAction(nameof(Detail), new { orderId = orderFromDb.Id });
		}

		#region call api

		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderList = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser").ToList();
			switch (status)
			{
				case "pending":
					orderList = orderList.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
					break;
				case "inproccess":
					orderList = orderList.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;
				case "completed":
					orderList = orderList.Where(u => u.OrderStatus == SD.StatusShipped);
					break;
				case "approved":
					orderList = orderList.Where(u => u.OrderStatus == SD.StatusApproved);
					break;
				default:			
					break;
			}
			return Json(new {data = orderList});
		}
		#endregion
	}
}
