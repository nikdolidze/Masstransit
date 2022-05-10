using Automatonymous;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Components.StateMachines.OrderStateMachineActivities;
using Sample.Contracts;
using System;
using System.Collections.Generic;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(()=> OrderAccepted, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderRejected, x => x.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, x =>
            {
                x.CorrelateById(m => m.Message.OrderId);
                x.OnMissingInstance(m => m.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
                    }
                }));
            });

            Event(()=>AccountClosed,x=>x.CorrelateBy((saga,context)=>saga.CustomerNumber == context.Message.CustomerNumber));

            InstanceState(x => x.CurrentState);
            Initially(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate = context.Data.TimeStap;
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.Updated = DateTime.UtcNow;
                    })
                 .TransitionTo(Submeted));

            Initially(
                When(OrderRejected)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate = context.Data.TimeStap;
                        context.Instance.CustomerNumber = context.Data.CustomerNumber;
                        context.Instance.Updated = DateTime.UtcNow;
                    })
                 .TransitionTo(Rejected));

            During(Submeted, Ignore(OrderSubmitted)
                , When(AccountClosed)
                    .TransitionTo(Cancelded),
                    When(OrderAccepted)
                        .Activity(x=> x.OfType<AcceptOrderActivity>()).
                            TransitionTo(Accepted));


            DuringAny(
                When(OrderSubmitted)
                    .Then(context =>
                    {
                        context.Instance.SubmitDate ??= context.Data.TimeStap;
                        context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
                    })
            );

            DuringAny(
                When(OrderStatusRequested)
                .RespondAsync(x => x.Init<OrderStatus>(new
                {

                    OrderId = x.Instance.CorrelationId,
                    CustomerNumber = x.Instance.CustomerNumber,
                    State = x.Instance.CurrentState
                })));
        }
        public State Cancelded { get; private set; }
        public State Accepted { get;private set; }
        public State Rejected { get; private set; }
        public State Submeted { get; private set; }
        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<OrderAccepted> OrderAccepted { get; private set; }
        public Event<OrderRejected> OrderRejected { get; private set; }
        public Event<CheckOrder> OrderStatusRequested { get; private set; }
       public Event<CustomerAccountClosed> AccountClosed { get; private set; } 

    }

    public class OrderStateMap :
    SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.SubmitDate);

            // If using Optimistic concurrency, otherwise remove this property
            // entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
    public class OrderStateDbContext :
    SagaDbContext
    {
        public OrderStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new OrderStateMap(); }
        }
    }
}
