using System.ComponentModel.DataAnnotations;

namespace PAUYS.Mvc.Areas.Admin.Models
{
    public class LoginRequestViewModel
    {
        [EmailAddress]
        public string UserName { get; set; } = null!;
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
