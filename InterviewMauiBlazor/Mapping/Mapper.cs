using AutoMapper;
using InterviewMauiBlazor.Database.Entities;
using InterviewMauiBlazor.DTO;


namespace InterviewMauiBlazor.Mapping
{
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderDTO, Order>().ReverseMap();
                CreateMap<ProductDTO, Product>().ReverseMap();
                CreateMap<TransactionDTO,Transaction>().ReverseMap();
            }
        }
}
