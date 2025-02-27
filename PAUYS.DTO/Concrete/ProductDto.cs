using PAUYS.DTO.Abstract;

namespace PAUYS.DTO.Concrete
{
    public class ProductDto : BaseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } // Her ürün için resim URL'si

        public int MaterialId { get; set; }
        public MaterialDto MaterialDto { get; set; } // Hangi Material olduğunu belirliyoruz

        public List<CategoryDto> Categories
        {
            get; set;
        }


    }   
    //burası entityle refeanstan olmicak commonla referanslama olacak
}
