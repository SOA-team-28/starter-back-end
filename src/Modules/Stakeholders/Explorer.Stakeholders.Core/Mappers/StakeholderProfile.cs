using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
	public StakeholderProfile()
    {
		CreateMap<ClubInvitationDto, ClubInvitation>().ReverseMap();
		CreateMap<ClubDto, Club>().ReverseMap();
		CreateMap<UserClubDto, UserClub>().ReverseMap();
	}
}