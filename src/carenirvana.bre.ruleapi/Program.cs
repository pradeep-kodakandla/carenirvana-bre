using carenirvana.bre;
using carenirvana.bre.engine;
using carenirvana.bre.engine.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var configBuilder = builder.Configuration.AddJsonFile(
                "appsettings.json", 
                optional: false, 
                reloadOnChange: true);
ConfigReader.Configuration = configBuilder.Build();

// Add services to the container.
var services = builder.Services;
services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSingleton<IRuleEngine, RuleEngine>(re =>
{
    return new RuleEngine(File.ReadAllText(@"BREConfigDataEx.json"));
});

var app = builder.Build();



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
