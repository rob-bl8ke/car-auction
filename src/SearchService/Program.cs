using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Look in these assemblies for any class that derives from AutoMapper.Profile
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    // Add the retry policy
    .AddPolicyHandler(GetPolicy());

// Configure Mass Transit for message queueing.
builder.Services.AddMassTransit(x => {
    // Find all my consumers here (in this namespace)...
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    // Make sure the queue is called "search-auction-created"
    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async() => {
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
});

app.Run();

// If the AuctionService is down, keep trying every 3 seconds until it's up again.
// The example includes "NotFound" that is not transient (just to show how policies
// can be chained).
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
