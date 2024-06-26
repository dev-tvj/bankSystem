using CustomerService.Data;
using CustomerService.Services;
using CustomerService.Services.Interfaces;
using CustomerService.Workers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BankContext>(options =>
    options.UseNpgsql(connectionString));

// Configure RabbitMQ connection factory
builder.Services.AddSingleton<ConnectionFactory>(new ConnectionFactory 
{ 
    HostName = "rabbitmq",
    UserName = "guest",
    Password = "guest"
});

// Add scoped service for ICustomerServices
builder.Services.AddScoped<ICustomerServices, CustomerServices>();

// Add singleton service for IWorkerServices
builder.Services.AddSingleton<IWorkerServices, WorkerServices>();

// Add the CreditCardQueueWorker service as a hosted service
builder.Services.AddHostedService<CreditCardQueueWorker>();

// Add the CreditOfferQueueWorker service as a hosted service
builder.Services.AddHostedService<CreditOfferQueueWorker>();

var app = builder.Build();


// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BankContext>();
    context.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My service");
        c.RoutePrefix = string.Empty;  // Set Swagger UI at apps root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();