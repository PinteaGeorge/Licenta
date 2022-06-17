using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(d => d.PhotoUrl, o => o
                .MapFrom(s => s.Photos
                .FirstOrDefault(x => x.IsMain).Url))
                .ForMember(x => x.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Message, MessageDto>()
                .ForMember(x => x.SenderPhotoUrl, opt => opt.MapFrom(src => 
                    src.Sender.Photos.FirstOrDefault(z => z.IsMain).Url))
                .ForMember(x => x.RecipientPhotoUrl, opt => opt.MapFrom(src => 
                    src.Recipient.Photos.FirstOrDefault(z => z.IsMain).Url));
        }
    }
}
