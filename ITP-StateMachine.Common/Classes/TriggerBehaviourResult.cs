using System.Collections.Generic;

namespace Classes
{
    public partial class StateMachine<TState, TTrigger>
    {
        public class TriggerBehaviourResult
        {
            public TriggerBehaviourResult(TriggerBehaviour handler, ICollection<string> unmetGuardConditions)
            {
                Handler = handler;
                UnmetGuardConditions = unmetGuardConditions;
            }
            public TriggerBehaviour Handler { get; }
            public ICollection<string> UnmetGuardConditions { get; }
        }
    }
}
