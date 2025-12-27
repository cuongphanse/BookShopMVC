using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModel;
using Models.VnPay;
using System.Security.Claims;
using Utility;
using Utility.VnPay;

namespace BookShopMVC.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IVnPayService _vnPayService;
		[BindProperty]
		public ShoppingCartVM shoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork, IVnPayService vnPayService)
		{
			_unitOfWork = unitOfWork;
			_vnPayService = vnPayService;		
		}

		public IActionResult Index()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId,
				includeProperties: "Product"),
				OrderHeader = new()
			};
			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(shoppingCartVM);
		}

		public IActionResult Summary()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId,
				includeProperties: "Product"),
				OrderHeader = new()
			};

			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
			shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
			shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(shoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
				includeProperties: "Product");

			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			shoppingCartVM.OrderHeader.ApplicationUserId = userId;

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if(applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//la customer
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
			}
			_unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
			_unitOfWork.Save();
			foreach(var cart in shoppingCartVM.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = shoppingCartVM.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count,
				};
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}
			// Thêm vào trong SummaryPOST, phần xử lý cho Customer
			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				var vnModel = new VnPayRequestModel
				{
					Amount = shoppingCartVM.OrderHeader.OrderTotal,
					OrderId = shoppingCartVM.OrderHeader.Id,
					CreatedDate = DateTime.Now
				};
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnModel));// Chuyển hướng sang trang VNPay
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id});
		}

		public IActionResult OrderConfirmation(int id)
		{
			// 1. Lấy ID đơn hàng từ VNPay nếu redirect về không kèm id route
			if (id == 0 && Request.Query.ContainsKey("vnp_TxnRef"))
			{
				int.TryParse(Request.Query["vnp_TxnRef"], out id);
			}

			if (id == 0) return NotFound("Không tìm thấy mã đơn hàng.");

			// 2. Xử lý logic thanh toán VNPay nếu có dữ liệu trả về
			if (Request.Query.ContainsKey("vnp_ResponseCode"))
			{
				ProcessVnPayReturn(id);
			}

			// 3. Lấy thông tin đơn hàng để hiển thị và dọn dẹp giỏ hàng
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id);
			if (orderHeader == null) return NotFound();

			// Chỉ xóa giỏ hàng nếu đơn hàng đã được xác nhận thanh toán hoặc duyệt
			if (orderHeader.PaymentStatus == SD.PaymentStatusApproved ||
				orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
				ClearUserCart(orderHeader.ApplicationUserId);
			}

			return View(id);
		}

		private void ProcessVnPayReturn(int orderId)
		{
			// Sử dụng Service đã viết ở bước trước để check chữ ký
			if (_vnPayService.ValidateSignature(Request.Query))
			{
				string responseCode = Request.Query["vnp_ResponseCode"];
				string transactionNo = Request.Query["vnp_TransactionNo"];

				if (responseCode == "00") // Giao dịch thành công
				{
					var orderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderId);
					if (orderFromDb != null && orderFromDb.PaymentStatus != SD.PaymentStatusApproved)
					{
						orderFromDb.OrderStatus = SD.StatusApproved;
						orderFromDb.PaymentStatus = SD.PaymentStatusApproved;
						orderFromDb.PaymentDate = DateTime.Now;
						orderFromDb.PaymentIntentId = transactionNo;

						_unitOfWork.OrderHeader.Update(orderFromDb);
						_unitOfWork.Save();
					}
				}
			}
		}

		private void ClearUserCart(string userId)
		{
			var cartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).ToList();
			if (cartList.Any())
			{
				_unitOfWork.ShoppingCart.RemoveRange(cartList);
				_unitOfWork.Save();
			}
		}

		public IActionResult Plus(int cartId)
		{
			var cartFormDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId);
			cartFormDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFormDb);
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}
		public IActionResult Minus(int cartId)
		{
			var cartFormDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId);
			if (cartFormDb.Count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(cartFormDb);
			}
			else { 			
				cartFormDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFormDb);
			}
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}
		public IActionResult Remove(int cartId)
		{
			var cartFormDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId);
			_unitOfWork.ShoppingCart.Remove(cartFormDb);
			_unitOfWork.Save();
			return RedirectToAction("Index");
		}
		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
				if (shoppingCart.Count <= 100)
			{
				return shoppingCart.Product.Price100;
			}
			else
			{
				return shoppingCart.Product.Price500;
			}
		}
	}
}
