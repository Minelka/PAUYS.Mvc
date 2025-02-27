using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class MaterialViewModel : BaseViewModel<int>
    {
        public MaterialViewModel() : base(0) { }

        [Display(Name = "Materyal Adı")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Name { get; set; } = null!;

        //entityde Product listesi var
        public ICollection<ProductViewModel>? ProductViewModels { get; set; }  //Book store de entitydeki de Icollection
        //


    }
}
