using Microsoft.AspNetCore.Http;
using PAUYS.Common.Validations;
using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class ProductViewModel : BaseViewModel<int>
    {

        public ProductViewModel() : base(0) { }

        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Name { get; set; } = null!;

        [Display(Name = "Ürün Açıklaması")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Description { get; set; } = null!;

        [Display(Name = "Ürün Tutarı")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public decimal Price { get; set; } = 0m;

        [Display(Name = "Fotoğrafı")]
        public byte[]? Picture { get; set; }

        [Display(Name = "Fotoğrafın Adı")]
        public string? PictureFileName { get; set; }
        //----------------------------
        public string? matName { get; set; }
        public int MaterialId { get; set; }
        public MaterialViewModel? MaterialViewModel { get; set; }

        [Display(Name = "Üretildiği Materyal Adı")]
        public string? MadedMaterialName => MaterialViewModel?.Name;

        //[Display(Name = "Üretildiği Materyal Adı")]
        //[Required(ErrorMessage = "Bu alan zorunludur")]


        //---------------------------

        public string? catName { get; set; }
        public int CategoryId { get; set; }
        public CategoryViewModel? CategoryViewModel { get; set; }

        [Display(Name = "Kategori Adı")]
        public string? ProductCategoryName => CategoryViewModel?.Name;

        // public ICollection<CategoryViewModel>? CategoryViewModels { get; set; }  //Book store de entitydeki de Icollection
        //




    }

    public class ProductAddViewModel : ProductViewModel
    {
        [Display(Name = "Fotoğrafı")]
        //[ImageFile("image/png", "image/jpeg")]
        public IFormFile? PictureFormFile { get; set; }

        public void ConvertPicture()
        {
            if (PictureFormFile != null && PictureFormFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    PictureFormFile.CopyTo(memoryStream);
                    Picture = memoryStream.ToArray();
                    PictureFileName = PictureFormFile.FileName;
                }
            }
        }
    }
}
