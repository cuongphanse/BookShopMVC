using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Utility;

namespace BookShopMVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
			return View(companyList);
		}
		public IActionResult Upsert(int? id)
		{
			if (id == null || id == 0)
			{
				return View(new Company());
			}
			else { 
				Company companyObj = _unitOfWork.Company.Get(u=>u.Id == id);
				return View(companyObj);
			}
		}
		[HttpPost]
		public IActionResult Upsert(Company companyObj)
		{
			if (ModelState.IsValid)
			{
				if (companyObj.Id == 0)
				{
					_unitOfWork.Company.Add(companyObj);
				}
				else
				{
					_unitOfWork.Company.Update(companyObj);
				}
				_unitOfWork.Save();
				TempData["success"] = "Them thanh cong";
				return RedirectToAction("Index");
			}
			else { 
				return View(companyObj);
			}
		}
		#region Call API
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
			return Json(new { data = objCompanyList });
		}
		#endregion

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			Company companyObj = _unitOfWork.Company.Get(c => c.Id == id);
			if(companyObj == null)
			{
				return Json(new { success = false , message = "Loi tim khong duoc cong ty" });
			}
			_unitOfWork.Company.Remove(companyObj);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Xoa thanh cong" });
		}
	}
}
