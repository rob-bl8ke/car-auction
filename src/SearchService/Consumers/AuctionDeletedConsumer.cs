using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
    private readonly IMapper mapper;

    public AuctionDeletedConsumer(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionDeleted> context)
    {
        Console.WriteLine("--> Consuming auction deleted: " + context.Message.Id);

        // Retrieve the message from the queue, convert it to an EF entity, and update.
        var item = mapper.Map<Item>(context.Message);

        var result = await DB.DeleteAsync<Item>(context.Message.Id);

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AuctionUpdated), "Problem deleting auction");
    }
}
