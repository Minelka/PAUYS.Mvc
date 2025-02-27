using AutoMapper;
using PAUYS.DTO.Concrete;
using PAUYS.ViewModel.Concrete;

namespace PAUYS.Mapping.MapperProfiles
{
    public class CategoryProfile : Profile
	{
		public CategoryProfile()
		{
			CreateMap<CategoryViewModel, CategoryDto>()
				.ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductViewModels))
				.ReverseMap();

        }
	}
}
