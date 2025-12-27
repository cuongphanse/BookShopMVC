using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Models.VnPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.VnPay
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _config;

		public VnPayService(IConfiguration config) => _config = config;

		public string CreatePaymentUrl(HttpContext context, VnPayRequestModel model)
		{
			var vnpay = new VnPayLibrary();
			var vnpayConfig = _config.GetSection("Vnpay");

			vnpay.AddRequestData("vnp_Version", vnpayConfig["Version"]);
			vnpay.AddRequestData("vnp_Command", vnpayConfig["Command"]);
			vnpay.AddRequestData("vnp_TmnCode", vnpayConfig["TmnCode"]);
			vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
			vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", vnpayConfig["CurrCode"]);
			vnpay.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
			vnpay.AddRequestData("vnp_Locale", vnpayConfig["Locale"]);
			vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + model.OrderId);
			vnpay.AddRequestData("vnp_OrderType", "other");
			vnpay.AddRequestData("vnp_ReturnUrl", vnpayConfig["ReturnUrl"]);
			vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());

			return vnpay.CreateRequestUrl(vnpayConfig["BaseUrl"], vnpayConfig["HashSecret"]);
		}

		public bool ValidateSignature(IQueryCollection collections)
		{
			var vnpay = new VnPayLibrary();
			foreach (var (key, value) in collections)
			{
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
					vnpay.AddResponseData(key, value);
			}

			string secureHash = collections["vnp_SecureHash"];
			string hashSecret = _config["Vnpay:HashSecret"];
			return vnpay.ValidateSignature(secureHash, hashSecret);
		}
	}
}
