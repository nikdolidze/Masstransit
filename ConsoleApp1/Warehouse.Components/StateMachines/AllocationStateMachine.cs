using Automatonymous;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;

using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines
{
    public class AllocationStateMachine : MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine()
        {
            Event(() => AllocationCreated, x=>x.CorrelateById(m=>m.Message.AllocationId));

            Schedule(() => HoldExpiration, x => x.HoldDurationToken, s =>
            {
                s.Delay = TimeSpan.FromHours(1);
                s.Received = x => x.CorrelateById(m => m.Message.AllocationId);

            });

            InstanceState(x => x.CurrentState);

            //Initially(
            //    When(AllocationCreated)
            //    .TransitionTo(Allocated));


            Initially(
                When(AllocationCreated)
                .Schedule(HoldExpiration, context => context.Init<AllocationHoldDurationExpired>(new
                {
                    context.Data.AllocationId
                }), context => context.Data.HolDuration)
                .TransitionTo(Allocated));

            During(Allocated,
                When(HoldExpiration.Received)
                .ThenAsync(context => Console.Out.WriteLineAsync($"Allocation was released :-nika------------ {context.Instance.CorrelationId}"))
                .TransitionTo(Test));

            SetCompletedWhenFinalized();
        }

        public Schedule<AllocationState,AllocationHoldDurationExpired> HoldExpiration { get; set; }
        public State Allocated { get; set; }
        public State Test { get; set; }
        public Event<AllocatoinCreated> AllocationCreated { get; set; } 
    }

    public class AllocationState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid CorrelationId {get; set;}
        public Guid?  HoldDurationToken{get; set;}
    }

    public class AllocationMap :
   SagaClassMap<AllocationState>
    {
        protected override void Configure(EntityTypeBuilder<AllocationState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.HoldDurationToken);
            entity.Property(x => x.CorrelationId);

            // If using Optimistic concurrency, otherwise remove this property
            // entity.Property(x => x.RowVersion).IsRowVersion();
        }
    }
    public class AllocationStateDbContext :
    SagaDbContext
    {
        public AllocationStateDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new AllocationMap(); }
        }
    }
}
