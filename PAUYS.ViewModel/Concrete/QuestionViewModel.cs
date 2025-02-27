using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class QuestionViewModel : BaseViewModel<int>
    {
        public QuestionViewModel() : base(0) { }

        [Display(Name = "Soru")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Questions { get; set; } = null!;

        [Display(Name = "Yanıt")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Answer { get; set; } = null!;



    }
}
