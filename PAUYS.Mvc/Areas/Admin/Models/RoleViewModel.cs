using Microsoft.AspNetCore.Mvc;

namespace PAUYS.Mvc.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public string Name { get; set; } = null!;
    }
    public class RoleCreateViewModel : RoleViewModel
    {

    }

    public class RoleUpdateViewModel : RoleViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; } = null!;
    }

    public class RoleWithUsersViewModel : RoleUpdateViewModel
    {
        public List<UserResponseViewModel> Users { get; set; } = null!;
    }
}
