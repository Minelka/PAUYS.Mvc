using PAUYS.ViewModel.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAUYS.ViewModel.Concrete
{
    public class PackingGuideViewModel : BaseViewModel<int>
    {
        public PackingGuideViewModel() : base(0) { }

        [Display(Name = "Onay")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Confirmation { get; set; } = null!;

        [Display(Name = "Numune")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Sample { get; set; } = null!;

        [Display(Name = "Kalıp Üretimi")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string MoldProduction { get; set; } = null!;

        [Display(Name = "Finalizasyon")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Finalization { get; set; } = null!;



    }
}
