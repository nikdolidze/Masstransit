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
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));
            Event(() => ReleaseRequested, x => x.CorrelateById(m => m.Message.AllocationId));

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
                .TransitionTo(Allocated),
                When(ReleaseRequested)
                .TransitionTo(Released));

            During(Released,
              When(AllocationCreated)
                  .Then(context => Console.Out.WriteAsync($"Allocation already released: { context.Instance.CorrelationId}"))
                  .Finalize()
          );


            During(Allocated,
                When(HoldExpiration.Received)
                .ThenAsync(context => Console.Out.WriteLineAsync($"Allocation expired:-nika------------ {context.Instance.CorrelationId}"))
                  .Finalize()
                  ,When(ReleaseRequested)
                  .Unschedule(HoldExpiration)
                    .ThenAsync(context => Console.Out.WriteLineAsync($"Allocation realise requset : granted------------ {context.Instance.CorrelationId}"))
                  .Finalize());

            SetCompletedWhenFinalized();
        }

        public Schedule<AllocationState, AllocationHoldDurationExpired> HoldExpiration { get; set; }
        public State Released { get; set; }
        public State Allocated { get; set; }
        public Event<AllocatoinCreated> AllocationCreated { get; set; }
        public Event<AllocationReleaseRequested> ReleaseRequested { get; set; }
    }

    public class AllocationState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? HoldDurationToken { get; set; }
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
