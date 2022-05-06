using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MassTransit;
using Sample.Components.Consumers;
using Sample.Contracts;
using med = MassTransit.Mediator;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MassTransit.Definition;
using GreenPipes;
using System;
using Sample.Components.StateMachines;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Sample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {



            //services.AddMediator(cfg =>
            //{
            //    cfg.AddRequestClient<SubmitOrder>();

            //    cfg.AddConsumer<SubmitOrderConsumer>();
            //});
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);





            services.AddMassTransit(cfg =>
            {
                cfg.AddRequestClient<SubmitOrder>(new Uri($"queue:" +
                    $"{ KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

                cfg.AddRequestClient<CheckOrder>();

                //cfg.AddSagaStateMachine<OrderStateMachine, OrderState>()
                //            .EntityFrameworkRepository(r =>
                //            {
                //                r.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion

                //                r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                //              {


                //                  builder.UseSqlServer("Server=NDOLIDZE-LP;Database=Saga;Trusted_Connection=True", m =>
                //                  {
                //                      m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                //                      m.MigrationsHistoryTable($"__{nameof(OrderStateDbContext)}");
                //                  });
                //              });
                //            });


            //    cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(x =>
                {
                    //     x.UseMessageRetry(r => r.Immediate(100));
                    x.ConfigureEndpoints(provider);
                 }));



            });

            services.AddMassTransitHostedService();


            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
