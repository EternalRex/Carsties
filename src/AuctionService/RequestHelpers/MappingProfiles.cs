using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.Execution;

namespace AuctionService.RequestHelpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            /*Since auction contains both the values of the Auction and the Item, After mapping the DTO
            to the item, we add "IncludeMembers" so that the remaining values of the AuctionDto will also
            be mapped to the Item properties cause Item is a member of Auction in our Data Model or Entity*/
            CreateMap<Auction, AuctionDto>()
                .IncludeMembers(property => property.Item);

            /*Mapping to ensure that incase we need Item to be mapped to AuctionDto
            atleast its already ready*/
            CreateMap<Item, AuctionDto>();

            /*ForMember: The Auction entity contains an Item as a navigation property, but the CreateAuctionDto contains
             flat properties for both the Auction and Item. This ForMember configuration is telling AutoMapper
             to map the relevant properties from CreateAuctionDto (which are item-specific) into the Item navigation
             property within Auction.*/
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(member => member.Item, value => value.MapFrom(source => source));

            /*Mapping to ensure that incase we need Item to be mapped to CreateAuctionDto
           atleast its already ready*/
            CreateMap<CreateAuctionDto, Item>();
        }
    }
}
