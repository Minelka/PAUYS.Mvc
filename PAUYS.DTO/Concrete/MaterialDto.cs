using PAUYS.DTO.Abstract;

namespace PAUYS.DTO.Concrete
{
    public class MaterialDto : BaseDto
    {
        public string Name { get; set; }

        //public string Description { get; set; }

        public List<ProductDto> Products { get; set; }
    }
}
