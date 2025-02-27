using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.Mvc.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? PictureFileName { get; set; } = string.Empty;
    }
    public class UserCreateViewModel : UserViewModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

    public class UserUpdateViewModel : UserViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; } = null!;
    }

    public class UserWithRolesViewModel : UserUpdateViewModel
    {
        public List<RoleResponseViewModel> Roles { get; set; } = null!;
    }
}
