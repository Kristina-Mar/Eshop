using Eshop.Domain;
using Eshop.Persistence;
using Eshop.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHostedService<PaymentProcessingService>();

    builder.Services.AddDbContext<OrdersContext>();
    builder.Services.AddScoped<IRepositoryAsync<Order>, OrderRepositoryAsync>();
    builder.Services.AddSingleton<IProcessingQueue<Order>, PaymentProcessingQueue>();
}

var app = builder.Build();

{
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "RestApiEshop API V1"));
}

app.Run();
