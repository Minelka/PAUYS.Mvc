using Microsoft.AspNetCore.Http;
using PAUYS.ViewModel.Abstract;
using System.ComponentModel.DataAnnotations;

namespace PAUYS.ViewModel.Concrete
{
    public class NewsViewModel : BaseViewModel<int>
    {
        public NewsViewModel() : base(0) { }

        [Display(Name = "Başlık Adı")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Title { get; set; } = null!;

        [Display(Name = "Yazı")]
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string Text { get; set; } = null!;

        [Display(Name = "Fotoğrafı")]
        public byte[]? Picture { get; set; }

        [Display(Name = "Fotoğrafın Adı")]
        public string? PictureFileName { get; set; }

        public IFormFile? PictureFormFile { get; set; }

    }

    public class NewsAddViewModel : NewsViewModel
    {
        [Display(Name = "Fotoğrafı")]
        //[ImageFile("image/png", "image/jpeg")]
        //public IFormFile? PictureFormFile { get; set; }

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