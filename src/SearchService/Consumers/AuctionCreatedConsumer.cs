using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
    private readonly IMapper mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        this.mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

        // Retrieve the message from the queue, convert it to an EF entity, and persist.
        var item = mapper.Map<Item>(context.Message);

        // Manufacture an exception that can't be resolved and will always fail after retries.
        // (not network related)
        if (item.Make == "Foo") throw new ArgumentException("Cannot sell cars with name of Foo");

        await item.SaveAsync();
    }
}
