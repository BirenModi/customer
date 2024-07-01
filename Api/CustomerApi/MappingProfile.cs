using AutoMapper;
namespace Customer.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer.Data.Model.Customers, Dto.Customer.Response.Customers>().ReverseMap();
            CreateMap<Customer.Data.Model.Customers, Dto.Customer.Request.Customers>().ReverseMap();
        }
    }
}
