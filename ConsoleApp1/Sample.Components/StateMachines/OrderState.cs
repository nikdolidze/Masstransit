using Automatonymous;
using System;

namespace Sample.Components.StateMachines
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public string CustomerNumber { get; set; }

    //    public string FauldReason { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? Updated { get; set; }

    }


}
