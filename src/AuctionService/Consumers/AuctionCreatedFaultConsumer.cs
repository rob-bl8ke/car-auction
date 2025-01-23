using Contracts;
using MassTransit;

// TODO: Fix namespace (look for any others)
namespace AuctionService;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    // For demonstration purposes only.
    // All items with a make of "Foo" will end up causing an exception when the
    // SearchService consumes the created message.
    // The message will be put into the "fault" queue "search-auction-created_error"
    // This consumer listens to the fault queue and processes the message. If the
    // message matches a condition, it is modified and inserted back into the
    // original queue to be processed once again.
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("--> Consuming faulty auction created message");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Make = "Foobar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Not an argument exception - update error dashboard somewhere");
        }
    }
}