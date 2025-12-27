using Microsoft.AspNetCore.Http;
using Models.VnPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
	public interface IVnPayService
	{
		string CreatePaymentUrl(HttpContext context, VnPayRequestModel model);
		bool ValidateSignature(IQueryCollection collections);
	}
}
