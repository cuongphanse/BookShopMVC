using DataAccess.Repository.IRepository;
using DataAcess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace BookShopMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			List<Category> categories = _unitOfWork.Category.GetAll().ToList();
			return View(categories);
		}

		public IActionResult Upsert(int? id)
		{
			if(id  == null)
			{
				return View(new Category());
			}
			else
			{
				var category = _unitOfWork.Category.Get(c => c.Id == id);
				return View(category);
			}
			
		}
		[HttpPost]
		public IActionResult Upsert(Category category)
		{
			var isNameExists = _unitOfWork.Category.Get(u =>
		u.Name.ToLower() == category.Name.ToLower() && u.Id != category.Id);

			if (isNameExists != null)
			{
				ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại.");
			}
			var isOrderExists = _unitOfWork.Category.Get(u =>
				u.DisplayOrder == category.DisplayOrder && u.Id != category.Id);

			if (isOrderExists != null)
			{
				ModelState.AddModelError("DisplayOrder", "Thứ tự hiển thị này đã tồn tại.");
			}
			if (ModelState.IsValid)
			{
				if (category.Id == 0)
				{				
					_unitOfWork.Category.Add(category);
					TempData["success"] = "Thêm mới thành công";
				}
				else
				{
					_unitOfWork.Category.Update(category);
					TempData["success"] = "Cập nhật thành công";
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			return View(category);
		}

		#region Api Calls
		public IActionResult GetAll()
		{
			List<Category> categories = _unitOfWork.Category.GetAll().ToList();
			return Json(new { data = categories });
		}
		#endregion
		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Category? category = _unitOfWork.Category.Get(c => c.Id == id);
			if (category == null)
			{
				return NotFound();
			}
			_unitOfWork.Category.Remove(category);
			_unitOfWork.Save();				
			return Json(new { success = true, message = "Xóa danh mục thành công" });
		}
	}
}
