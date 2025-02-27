using PAUYS.DTO.Abstract;

namespace PAUYS.DTO.Concrete
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; } // Kategori adı

        public string Shape { get; set; }
        public string Description { get; set; } // Kategori açıklaması

        public string UsingArea { get; set; } //Kullanım Alanı

        public List<ProductDto> Products { get; set; }
    }
}
