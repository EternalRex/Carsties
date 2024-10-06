using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /*A method that gets all the Auctions List and Its equivalent Item*/
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctionDto()
        {
            /*Query the database*/
            var auctions = await _context
                .Auctions.Include(property => property.Item)
                .OrderBy(item => item.Item.Make)
                .ToListAsync();
            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        /*Method that gets a particular Auction and Its Item*/
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            /*Query the database*/
            var auction = await _context
                .Auctions.Include(property => property.Item)
                .FirstOrDefaultAsync(property => property.Id == id);

            if (auction == null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            //Todo: Add current user as seller
            auction.Seller = "test";

            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest("Could not save changes to the database!");

            return CreatedAtAction(
                nameof(GetAuctionById),
                new { auction.Id },
                _mapper.Map<AuctionDto>(auction)
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            /*Query the database and look if there are records similar to his id*/
            var auction = await _context
                .Auctions.Include(member => member.Item)
                .FirstOrDefaultAsync(property => property.Id == id);

            if (auction == null)
                return NotFound();

            //Todo: Check seller = username

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Make;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;
            if (result)
                return Ok();
            return BadRequest("Problem Saving Changes");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAcution(Guid id)
        {
            /*Query the database to find the record to be deleted*/
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
                return NotFound();

            //TODO: Check seller == username

            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
                return BadRequest("Could not delete in DB");
            return Ok();
        }
    }
}
