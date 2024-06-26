
using RabbitMQ.Client;
using System.Text.Json.Serialization;
using CreditOfferService.Services.Interfaces;
using CreditOfferService.Services;
using CreditOfferService.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// Configure RabbitMQ connection factory
builder.Services.AddSingleton<ConnectionFactory>(new ConnectionFactory 
{ 
    HostName = "rabbitmq",
    UserName = "guest",
    Password = "guest"
});

// Add scoped service for ICustomerServices
builder.Services.AddSingleton<ICreditOfferServices, CreditOfferServices>();

// Add the CreditOfferQueueWorker service as a hosted service
builder.Services.AddHostedService<CreditOfferQueueWorker>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
        c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
    });
    //app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();