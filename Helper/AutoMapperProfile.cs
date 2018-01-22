using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
              CreateMap<User,UserForListDto>()
                    .ForMember(dest => dest.PhotoUrl, opt => {
                            opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                        })
                    .ForMember(dest => dest.Age, opt => {
                        opt.ResolveUsing( d => d.DateOfBirth.CalculateAge());
                    });

            CreateMap<User,UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                            opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                        })
                .ForMember(dest => dest.Age, opt => {
                        opt.ResolveUsing( d => d.DateOfBirth.CalculateAge());
                });

            CreateMap<Photo,PhotosForDetailDto>();
            CreateMap<UserForUpdateDto,User>();
            CreateMap<PhotoForCreateDto,Photo>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<UserForRegisterDto,User>();
        }
    }
}