using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.VnPay
{
	public class VnPayRequestModel
	{
		public int OrderId { get; set; }
		public double Amount { get; set; }
		public DateTime CreatedDate { get; set; }
	}
}
