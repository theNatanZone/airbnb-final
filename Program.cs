using AirbnbProj2.SwaggerExamples;
using AirbnbProj2.DAL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Filters;
using static AirbnbProj2.Controllers.VacationsController;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

// Register DBServices as a scoped service
builder.Services.AddScoped<DBService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<FlatsService>();
builder.Services.AddScoped<VacationsService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    var dateConverter = new IsoDateTimeConverter
    {
        DateTimeFormat = "yyyy-MM-dd"
    };

    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    options.SerializerSettings.Converters.Add(dateConverter);
    options.SerializerSettings.ContractResolver = new DefaultContractResolver
    {
        NamingStrategy = null
    };
    options.SerializerSettings.FloatFormatHandling = FloatFormatHandling.String;
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<VacationExamples>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// allow any origin- fix cors limit
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.MapControllers();

app.Run();
