using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Tên không được để trống")]
		[MaxLength(30)]
		public string Name { get; set; }
		[Range(1, 100,ErrorMessage ="Trong khoảng 1 tới 100")]
		//[DisplayName("đặt tên ở đây")]
		public int DisplayOrder { get; set; }
	}
}
