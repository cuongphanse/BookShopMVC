using DataAccess.Repository.IRepository;
using DataAcess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModel;
using Utility;

namespace BookShopMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _hostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_hostEnvironment = hostEnvironment;
		}
		public IActionResult Index()
		{
			List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
			
			return View(products);
		}

		public IActionResult Upsert(int? id)
		{	
			//ViewBag.CategoryList = categoryList;

			ProductVM productVM = new ProductVM()
			{
				Product = new Product(),
				CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				})
			};
			if(id == null || id == 0)
			{
				//Insert
				return View(productVM);
			}
			else
			{
				//Update
				productVM.Product = _unitOfWork.Product.Get(c => c.Id == id);
				return View(productVM);
			}
				
		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _hostEnvironment.WebRootPath;
				if(file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					var productPath = Path.Combine(wwwRootPath, @"images\products");
					if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//Xóa ảnh cũ
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
						if(System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\products\" + fileName;
				}
				if(productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
				}
				_unitOfWork.Save();
				TempData["success"] = "Thêm mới thành công";
				return RedirectToAction("Index");
			}else
			{
				//Nếu không hợp lệ thì load lại CategoryList
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Id.ToString()
				});
				return View(productVM);
			}
			
		}
	
	
		#region Api Calls
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new {data = products });
		}
		#endregion
		[HttpDelete]		
		public IActionResult Delete(int? id)
		{
			var productDelete = _unitOfWork.Product.Get(c => c.Id == id);
			if(productDelete == null)
			{
				return Json(new {success = false, message="Loi khi xoa"});
			}
			if (!string.IsNullOrEmpty(productDelete.ImageUrl))
			{
				var oldImage = Path.Combine(_hostEnvironment.WebRootPath, productDelete.ImageUrl.TrimStart('\\'));
				if (System.IO.File.Exists(oldImage))
				{
					System.IO.File.Delete(oldImage);
				}
			}
			_unitOfWork.Product.Remove(productDelete);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Xóa thành công" });
		}
	}
}
