using AutoMapper;
using PAUYS.DTO.Concrete;
using PAUYS.ViewModel.Concrete;

namespace PAUYS.Mapping.MapperProfiles
{
    public class MaterialProfile : Profile
	{
		public MaterialProfile()
		{
            CreateMap<MaterialViewModel, MaterialDto>()
             .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductViewModels))
             .ReverseMap();
        }


	}
}
