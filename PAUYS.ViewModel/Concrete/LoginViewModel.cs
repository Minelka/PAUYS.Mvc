using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class LoginViewModel
    {
        [EmailAddress]
        public string UserName { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
