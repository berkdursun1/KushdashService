using Microsoft.EntityFrameworkCore;
using playerService.Infrastructure;
using playerService.Mapping;
using playerService.Service;
using playerService.Service.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("transferMarkt", c => c.BaseAddress = new System.Uri("https://transfermarkt-api.fly.dev/"));
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<PlayerContext>(opt =>
    opt.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<ITransferMarktService, TransferMarktService>(client =>
{
    client.BaseAddress = new Uri("https://transfermarkt-api.fly.dev/");
});
builder.Services.AddTransient<IPlayerService, PlayerService>();
var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PlayerContext>();
    var initializer = new DbInitializer(scope.ServiceProvider.GetRequiredService<ITransferMarktService>());
    await initializer.InitializeAsync(dbContext);
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
