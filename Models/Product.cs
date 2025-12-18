using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		public string Description { get; set; }
		[Required]
		public string ISBN { get; set; }
		[Required]
		public string Author { get; set; }
		[Required]
		[Display(Name = "List Price")]
		[Range(10000, 1000000)]
		public double ListPrice { get; set; }
		[Required]
		[Display(Name = "Price for 10,000-100,000")]
		[Range(10000, 1000000)]
		public double Price { get; set; }
		[Required]
		[Display(Name = "Price for 100,000+")]
		[Range(10000, 1000000)]
		public double Price100 { get; set; }
		[Required]
		[Display(Name = "Price for 500,000+")]
		[Range(10000, 1000000)]
		public double Price500 { get; set; }

		public int CategoryId { get; set; }
		[ValidateNever]
		[ForeignKey("CategoryId")]
		public Category Category { get; set; }
		[ValidateNever]
		public string ImageUrl { get; set; }
	}
}
