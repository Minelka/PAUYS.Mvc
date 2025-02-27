namespace PAUYS.Mvc.Areas.Admin.Models
{
    public class ConfirmEmailViewModel
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
