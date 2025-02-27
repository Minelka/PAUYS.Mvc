using AutoMapper;
using PAUYS.DTO.Concrete;
using PAUYS.ViewModel.Concrete;

namespace PAUYS.Mapping.MapperProfiles
{
    public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<ProductViewModel, ProductDto>()
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryViewModels))
				.ReverseMap();
        }
	}
}
