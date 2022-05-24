using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Components.Consumers;
using Serilog;
using Serilog.Events;
using GreenPipes;
using MassTransit.Definition;
using Sample.Components.StateMachines;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Sample.Components.CurrierActivities;
using Sample.Components.StateMachines.OrderStateMachineActivities;
using Warehouse.Contracts;
using MassTransit.MessageData;

namespace Sample.Service
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true);
                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<AcceptOrderActivity>();
                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();//(typeof(SubmitOrderConsumerDefinition));
                        cfg.AddActivitiesFromNamespaceContaining<AllocateInventoryActivity>();

                        cfg.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderStateMachineDefinition))
                              .EntityFrameworkRepository(r =>
                              {
                                  //    r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                                  r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                                 {


                                     builder.UseSqlServer("Server=NDOLIDZE-LP;Database=Saga;Trusted_Connection=True", m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                                    });
                                 });
                              });



                        cfg.AddBus(ConfigureBus);
                        cfg.AddRequestClient<AllocateInventory>();
                    });

                    services.AddHostedService<MassTransitConsoleHostedService>();
                })
                  .ConfigureLogging((hostingContext, logging) =>
                  {
                      logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                      logging.AddConsole();
                  });
            if (isService)
                await builder.UseWindowsService().Build().RunAsync();
            else
                await builder.RunConsoleAsync();
        }

        static IBusControl ConfigureBus(IBusRegistrationContext context)
        {
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.UseMessageData(new FileSystemMessageDataRepository(new System.IO.DirectoryInfo(@"C:\Users\n.dolidze\Desktop\consoleapp")));
                cfg.ConfigureEndpoints(context);
                //cfg.ReceiveEndpoint("submit-order", e =>
                //{

                //});
               
                //  cfg.UseMessageRetry(t => t.Immediate(50));
            });
        }

    }
}
