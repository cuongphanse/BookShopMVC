using System.Diagnostics;
using System.Security.Claims;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace BookShopMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
	public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;    
		public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
		}

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
			return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(c => c.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
		public IActionResult Details(ShoppingCart obj)
		{
			var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            obj.ApplicationUserId = userId;
            ShoppingCart cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.ProductId == obj.ProductId && u.ApplicationUserId == userId);
            if(cartFromDB != null)
            {
                cartFromDB.Count += obj.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDB);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(obj);
            }
            _unitOfWork.Save();
            TempData["success"] = "Thêm giỏ hàng thành công";
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
