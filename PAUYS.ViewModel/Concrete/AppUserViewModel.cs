
using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class AppUserViewModel : BaseViewModel<int>
    {
        public AppUserViewModel() : base(0) { }

        [Display(Name = "Adı Soyadı")]
        public string Name { get; set; } = null!;

        [Display(Name = "E-Posta Adresi")]
        [EmailAddress(ErrorMessage = "E-Posta adresiniz doğru girmediniz.")]
        public string Email { get; set; } = null!;

        [Display(Name = "Kullanıcı Adı")]
        [Range(3, 10, ErrorMessage = "Kullanıcı adınız en az 3 en fazla 10 karakterden oluşmalıdır.")]
        [RegularExpression("([a-z]{3,10})\\w+", ErrorMessage = "Kullanıcı adınız küçük harf ve türkçe karakter içermemelidir.")]
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? PictureFileName { get; set; } = string.Empty;

    }
}
