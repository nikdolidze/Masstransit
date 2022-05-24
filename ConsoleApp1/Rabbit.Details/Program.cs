using MassTransit;
using MassTransit.Topology;
using RabbitMQ.Client;
using Sample.Conponents;
using Sample.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Contracts
{
    public interface UpdateAccount
    {
        string AccountNumber { get; }
    }
    public interface DeleteAccount
    {
        string AccountNumber { get; }
    }
}
namespace Sample.Conponents
{
    public class AccountConsumer : IConsumer<UpdateAccount>
    {
        public Task Consume(ConsumeContext<UpdateAccount> context)
        {
            Console.WriteLine("Command received :{0} on {1}", context.Message.AccountNumber,context.ReceiveContext.InputAddress);
            return Task.CompletedTask;
        }
    }

    public class AnotherAccountConsumer : IConsumer<UpdateAccount>
    {
        public Task Consume(ConsumeContext<UpdateAccount> context)
        {
            Console.WriteLine("another Command received :{0} on {1}", context.Message.AccountNumber,context.ReceiveContext.InputAddress);
            return Task.CompletedTask;
        }
    }
}

namespace Rabbit.Details
{
    class MyFormater : IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return "რიმდარიროოო რამდარიროო დაირაი დაირაი დაირაროოოოო";
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", h =>
                 {
                     h.Username("guest");
                     h.Password("guest");

                 });
                cfg.Publish<UpdateAccount>(m =>
                {
                    m.ExchangeType = ExchangeType.Direct;
                    m.BindAlternateExchangeQueue("unmached");
                });


                cfg.ReceiveEndpoint("unmached", e =>
                {
                    e.Consumer<AccountConsumer>();

                });
                //     cfg.MessageTopology.SetEntityNameFormatter(new MyFormater());   

                //   cfg.Message<UpdateAccount>(x => x.SetEntityName("update-account"));
                cfg.ReceiveEndpoint("account-service-a", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Consumer<AccountConsumer>();
                    e.Bind<UpdateAccount>(b =>
                    {
                        b.ExchangeType = ExchangeType.Direct;
                        b.RoutingKey = "A";
                    });
                });
                cfg.ReceiveEndpoint("account-service-b", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Consumer<AccountConsumer>();
                    e.Bind<UpdateAccount>(b =>
                    {
                        b.ExchangeType = ExchangeType.Direct;
                        b.RoutingKey = "B";
                    });
                });
                //cfg.ReceiveEndpoint("anothe-raccount-service", e =>
                //{
                //    e.Consumer<AnotherAccountConsumer>();
                //});
            });

            using var cancelation = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            await busControl.StartAsync(cancelation.Token);
            try
            {
                Console.WriteLine("Bus was started.");
                    await busControl.Publish<UpdateAccount>(new { AccountNumber = "123" },x=>x.SetRoutingKey("B"));

                await busControl.Publish<UpdateAccount>(new { AccountNumber = "456" }, x => x.SetRoutingKey("C"));
 

                //var endpoint = await busControl.GetSendEndpoint(new Uri("exchange:account"));
                //await endpoint.Send<UpdateAccount>(new { AccountNumber = "123" });


                //         await endpoint.Send<DeleteAccount>(new { AccountNumber = "456" });

                await Task.Run(Console.ReadLine);
            }
            finally
            {

                await busControl.StopAsync(CancellationToken.None);
            }
        }
    }
}
