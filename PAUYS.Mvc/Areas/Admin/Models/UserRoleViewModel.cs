using Microsoft.AspNetCore.Mvc;

namespace PAUYS.Mvc.Areas.Admin.Models
{
    public class UserRoleViewModel
    {
        public string RoleId { get; set; } = null!;

        [HiddenInput(DisplayValue = false)]
        public string UserId { get; set; } = null!;
    }
}
