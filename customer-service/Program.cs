using CustomerService.Data;
using CustomerService.Services;
using CustomerService.Services.Interfaces;
using CustomerService.Workers;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext
builder.Services.AddDbContext<BankContext>(options =>
    options.UseInMemoryDatabase("CustomerServiceDb"));



// Configure RabbitMQ connection factory
builder.Services.AddSingleton<ConnectionFactory>(new ConnectionFactory 
    { 
        HostName = "rabbitmq",
        UserName = "guest",
        Password = "guest"
    });

// Add scoped service for ICustomerServices
builder.Services.AddScoped<ICustomerServices, CustomerServices>();

// Add scoped service for IWorkerServices
builder.Services.AddSingleton<IWorkerServices, WorkerServices>();

// Add the CreditCardQueueWorker service as a hosted service
builder.Services.AddHostedService<CreditCardQueueWorker>();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();