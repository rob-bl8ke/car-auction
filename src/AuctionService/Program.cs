using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Look in these assemblies for any class that derives from AutoMapper.Profile
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure Mass Transit for message queueing.
builder.Services.AddMassTransit(x => {
    x.AddEntityFrameworkOutbox<AuctionDbContext>( o => {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

// Seed initial database development data
try
{
    DbInitializer.InitDb(app);
    
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

app.Run();
