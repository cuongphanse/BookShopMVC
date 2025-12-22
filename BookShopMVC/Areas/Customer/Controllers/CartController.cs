using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModel;
using System.Security.Claims;

namespace BookShopMVC.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private ShoppingCartVM shoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			shoppingCartVM = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId,
				includeProperties: "Product")
			};
			foreach (var cart in shoppingCartVM.ShoppingCartList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				shoppingCartVM.OrderTotal += (cart.Price * cart.Count);
			}
			return View(shoppingCartVM);
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
