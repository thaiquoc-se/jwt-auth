using AutoMapper;
using BusinessObjects.Models;
using DataTransferObject.DTO;
namespace DataAccessLayer.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
        }
    }
}
