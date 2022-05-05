using Automatonymous;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sample.Contracts;
using System;
using System.Collections.Generic;

namespace Sample.Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmited, x => x.CorrelateById(m => m.Message.OrderId));

            InstanceState(x => x.CurrentState);



            Initially(
                When(OrderSubmited)
                    .Then(context =>
                    {
                        context.Instance.OrderDate = context.Data.TimeStap;
                    })
                 .TransitionTo(Submeted));
        }
        public State Submeted { get; private set; }
        public Event<OrderSubmitted> OrderSubmited { get; private set; }
    }

    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime? OrderDate { get; set; }

    }

    public class OrderStateMap :
    SagaClassMap<OrderState>
    {
        protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.OrderDate);

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
