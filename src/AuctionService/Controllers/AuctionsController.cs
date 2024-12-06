using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext context;
    private readonly IMapper mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAllAuctionById(Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();
        
        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<CreateAuctionDto>> CreateAuction([FromBody] CreateAuctionDto auctionDto)
    {
        var auction = mapper.Map<Auction>(auctionDto);
        // TODO: Add current user as seller
        auction.Seller = "test-user";

        context.Add(auction);

        var result = await context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Could not save changes to DB");
        
        // Using "CreatedAtAction" will set the Location header for where the newly created
        // resource can be found.
        // The updated DTO is returned.
        return CreatedAtAction(nameof(GetAllAuctionById), 
            new { auction.Id }, mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto auctionDto)
    {
        var auction = await context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (auction == null) return NotFound();

        // TODO: Check seller == username

        auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = auctionDto.Year ?? auction.Item.Year;
        
        var result = await context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Problem saving changes");
        
        // A Location header does not need to be returned since the client already knows
        // the location of the resource.
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions.FindAsync(id);
        
        if (auction == null) return NotFound();

        // TODO: Check seller == username

        context.Remove(auction);
        var result = await context.SaveChangesAsync() > 0;
        
        if (!result) return BadRequest("Problem saving changes");
        
        return NoContent();
    }
}
