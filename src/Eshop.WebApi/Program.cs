using Eshop.Domain;
using Eshop.Persistence;
using Eshop.Persistence.Repositories;
using Eshop.WebApi.KafkaProducers;
using Eshop.KafkaBackgroundService;

var builder = WebApplication.CreateBuilder(args);
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5247);
    });
    
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHostedService<KafkaBackgroundService>();

    builder.Services.AddDbContext<OrdersContext>();
    builder.Services.AddScoped<IRepositoryAsync<Order>, OrderRepositoryAsync>();
    builder.Services.AddSingleton<IKafkaProducer, KafkaProducerOrderStatusUpdate>();
}

var app = builder.Build();

{
    app.MapControllers();
    app.UseSwagger();
    app.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "RestApiEshop API V1"));
}

app.Run();
