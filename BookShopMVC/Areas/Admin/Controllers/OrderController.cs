using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModel;
using Models.VnPay;
using System.Security.Claims;
using Utility;
using Utility.VnPay;

namespace BookShopMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty] // map du lieu tra ve
		public OrderVM OrderVM { get; set; }
		private readonly IVnPayService _vnPayService;
		public OrderController(IUnitOfWork unitOfWork, IVnPayService vpnPayService)
		{
			_unitOfWork = unitOfWork;
			_vnPayService = vpnPayService;
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
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcesscing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["Success"] = "Đã chuyển trạng thái đơn hàng sang Đang xử lý";
			return RedirectToAction(nameof(Detail), new { orderId = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult ShipProcesscing()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
			}
			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["Success"] = "Đã chuyển trạng thái đơn hàng sang đang giao hàng";
			return RedirectToAction(nameof(Detail), new { orderId = OrderVM.OrderHeader.Id });
		}
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public async Task<IActionResult> CancelOrder()
		{
			var id = OrderVM.OrderHeader.Id;
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);

			if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				// Thực hiện Refund qua VNPay
				var refundRequest = new VnPayRefundRequest
				{
					OrderId = orderHeader.Id.ToString(),
					Amount = (long)orderHeader.OrderTotal,
					TransactionDate = orderHeader.PaymentDate.ToString("yyyyMMddHHmmss"),
					CreateBy = User.Identity.Name
				};

				var result = await _vnPayService.Refund(refundRequest);

				// Phân tích kết quả JSON từ VNPay trả về
				// Nếu vnp_ResponseCode == "00" mới là thành công
				if (result.Contains("\"vnp_ResponseCode\":\"00\""))
				{
					_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusCancelled, SD.StatusRefunded);
				}
				else
				{
					TempData["Error"] = "Hoàn tiền VNPay thất bại.";
					return RedirectToAction("Details", new { id = id });
				}
			}
			else
			{
				// Hủy đơn hàng bình thường nếu chưa thanh toán
				_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusCancelled, SD.StatusCancelled);
			}

			_unitOfWork.Save();
			TempData["Success"] = "Đơn hàng đã được hủy và hoàn tiền thành công.";
			return RedirectToAction(nameof(Detail), new { id = id });
		}
		[HttpPost]
		public IActionResult PayNow()
		{
			try
			{
				var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);

				if (orderHeader == null)
				{
					TempData["error"] = "Không tìm thấy thông tin đơn hàng.";
					return RedirectToAction("Details", new { id = OrderVM.OrderHeader.Id });
				}

				var paymentRequest = new VnPayRequestModel
				{
					Amount = orderHeader.OrderTotal, // Quan trọng: Lấy giá từ DB
					OrderId = orderHeader.Id,
					CreatedDate = DateTime.Now
				};
				string returnUrl = Url.Action("PaymentCallback", "Order", new { id = orderHeader.Id }, Request.Scheme);
				string paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, paymentRequest, returnUrl);

				if (string.IsNullOrEmpty(paymentUrl))
				{
					throw new Exception("Không thể tạo URL thanh toán VNPay.");
				}

				return Redirect(paymentUrl);
			}
			catch (Exception ex)
			{
				TempData["error"] = "Cổng thanh toán đang gặp sự cố, vui lòng thử lại sau.";
				return RedirectToAction(nameof(Detail), new { id = OrderVM.OrderHeader.Id });
			}
		}
		public IActionResult PaymentCallback()
		{
			if (_vnPayService.ProcessPaymentResponse(Request.Query, out int vnpOrderId))
			{
				var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == vnpOrderId);
				orderHeader.PaymentDate = DateTime.Now;
				orderHeader.PaymentStatus = SD.PaymentStatusApproved;
				_unitOfWork.OrderHeader.Update(orderHeader);
				_unitOfWork.Save();
				TempData["success"] = "Thanh toán lại thành công!";
			}
			else
			{
				TempData["error"] = "Thanh toán thất bại.";
			}
			return RedirectToAction(nameof(Index));
		}
		#region call api

		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderList;

			if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
				orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				orderList = _unitOfWork.OrderHeader.GetAll(o=>o.ApplicationUserId == userId, includeProperties: "ApplicationUser").OrderByDescending(p=>p.Id).ToList();
			}

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
