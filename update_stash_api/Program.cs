using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using update_stash_api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IStashGraphQlService, StashGraphQlService>();

builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
{
    var queueCapacity = 100;
    return new BackgroundTaskQueue(queueCapacity);
});

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Run();
