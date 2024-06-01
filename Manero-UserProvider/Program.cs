using Manero_UserProvider.Handler;
using Manero_UserProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserProviderLibrary.Data.Context;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("SqlServer")));
        services.AddSingleton<ServiceBusHandler>();
        services.AddScoped<ServiceBusHandler>();
        services.AddScoped<CreateService>();
        services.AddScoped<GetAllService>();
        services.AddScoped<UpdateService>();
        services.AddScoped<GetOneService>();
        services.AddScoped<SaveImageService>();
    })
    .Build();

host.Run();
