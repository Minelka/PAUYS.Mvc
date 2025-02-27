using System.ComponentModel.DataAnnotations;

namespace PAUYS.AspNetCoreMvc.Models
{
    public class MailSendViewModel
    {
        public string? FromEmail { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [EmailAddress(ErrorMessage = "Email adresiniz yanlış yazıldı.")]
        [Display(Name = "Alıcı Mail Adresi")]
        public string ToEmail { get; set; } = null!;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [MinLength(5, ErrorMessage = "Bu alan en az 5 karakterden oluşmalıdır.")]
        [MaxLength(50, ErrorMessage = "Bu alan en az 50 karakterden oluşmalıdır.")]
        [Display(Name = "Konu")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "İçerik")]
        public string Body { get; set; } = null!;
    }
}
