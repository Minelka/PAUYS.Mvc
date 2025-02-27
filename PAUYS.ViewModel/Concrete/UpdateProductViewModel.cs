using Microsoft.AspNetCore.Http;
using PAUYS.Common.Validations;
using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class UpdateProductViewModel : BaseViewModel<int>
    {

        public UpdateProductViewModel() : base(0) { }

        [Display(Name = "Ürün Açıklaması")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Description { get; set; } = null!;

        [Display(Name = "Fotoğrafı")]
        [ImageFile("image/png", "image/jpeg")]
        public IFormFile? PictureFormFile { get; set; } = null!;

        [Display(Name = "Fotoğrafı")]
        public byte[]? Picture { get; set; }

        [Display(Name = "Fotoğrafın Adı")]
        public string? PictureFileName { get; set; }


        //----------------------------
        //public string? matName { get; set; }
        //public int MaterialId { get; set; }
        //public MaterialViewModel? MaterialViewModel { get; set; }

        //[Display(Name = "Üretildiği Materyal Adı")]
        //public string? MadedMaterialName => MaterialViewModel?.Name;

        ////[Display(Name = "Üretildiği Materyal Adı")]
        ////[Required(ErrorMessage = "Bu alan zorunludur")]


        ////---------------------------

        ////entityde Category listesi var
        //public ICollection<CategoryViewModel>? CategoryViewModels { get; set; }  //Book store de entitydeki de Icollection
        ////




    }
}
