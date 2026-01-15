using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Models.VnPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Utility.VnPay
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _config;

		public VnPayService(IConfiguration config) => _config = config;

		public string CreatePaymentUrl(HttpContext context, VnPayRequestModel model, string returnUrl)
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
			vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
			vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());

			//huy sau bao nhieu phut
			//vnpay.AddRequestData("vnp_ExpireDate", model.CreatedDate.AddMinutes(1).ToString("yyyyMMddHHmmss"));

			return vnpay.CreateRequestUrl(vnpayConfig["BaseUrl"], vnpayConfig["HashSecret"]);
		}
		public bool ProcessPaymentResponse(IQueryCollection query, out int orderId)
		{
			int.TryParse(query["vnp_TxnRef"], out orderId);
			if (!ValidateSignature(query)) return false;

			string responseCode = query["vnp_ResponseCode"];
			return responseCode == "00";
		}
		public async Task<string> Refund(VnPayRefundRequest request)
		{
			var vnpay = new VnPayLibrary();
			var vnpayConfig = _config.GetSection("Vnpay");
			var vnp_Api = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction"; // URL API Refund (Sandbox)
			var vnp_RequestId = DateTime.Now.Ticks.ToString(); // Mã ID định danh yêu cầu
			var vnp_Command = "refund";
			string vnp_TmnCode = vnpayConfig["TmnCode"];
			var vnp_TransactionType = "02"; // 02: Hoàn tiền toàn phần, 03: Hoàn tiền một phần
			var vnp_Amount = (request.Amount * 100).ToString();
			var vnp_TxnRef = request.OrderId;
			var vnp_OrderInfo = "Hoan tien don hang: " + request.OrderId;
			var vnp_TransactionDate = request.TransactionDate;
			var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
			var vnp_CreateBy = request.CreateBy;
			var vnp_IpAddr = "127.0.0.1";

			// Tạo chuỗi dữ liệu để ký Hash (Dòng này VNPay yêu cầu rất khắt khe về thứ tự)
			string hashData = vnp_RequestId + "|" + "2.1.0" + "|" + vnp_Command + "|" + vnp_TmnCode + "|" +
							  vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" +
							  "" + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" +
							  vnp_IpAddr + "|" + vnp_OrderInfo;
			var vnp_SecureHash = vnpay.HmacSHA512(vnpayConfig["HashSecret"], hashData);
			var rfObject = new
			{
				vnp_RequestId,
				vnp_Version = "2.1.0",
				vnp_Command,
				vnp_TmnCode = vnpayConfig["TmnCode"],
				vnp_TransactionType,
				vnp_TxnRef,
				vnp_Amount,
				vnp_OrderInfo,
				vnp_TransactionDate,
				vnp_CreateBy,
				vnp_CreateDate,
				vnp_IpAddr,
				vnp_SecureHash
			};
			using var client = new HttpClient();
			// Thêm User-Agent để tránh bị coi là bot/spam
			client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

			var response = await client.PostAsJsonAsync(vnp_Api, rfObject);
			return await response.Content.ReadAsStringAsync();
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
