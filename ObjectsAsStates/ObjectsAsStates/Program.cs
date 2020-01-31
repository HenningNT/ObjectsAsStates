using System;
using Stateless;

namespace ObjectsAsStates
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello ObjectsAsStates!");

            var oas = new ObjectsAsStates();
            oas.Initialize();

            oas.DoSomething();
            oas.DoSomethingElse(42);
            oas.DoSomething();

            Console.WriteLine("All done! Joy to the world!");
            Console.ReadKey();
        }
    }

    public class ObjectsAsStates
    {
        // All state object, and all trigger objects, need to be instantiated before we
        // can use them when Configuring the state machine.
        private readonly ObjectAsState _initialState = new ObjectAsState("");
        private readonly ObjectAsState _someState = new ObjectAsState("SomeState");
        private readonly AnotherObjectAsState _anotherState = new AnotherObjectAsState("AnotherState");

        private readonly ObjectAsTrigger _someTrigger = new ObjectAsTrigger();
        private readonly AnotherObjectAsTrigger _anotherTrigger = new AnotherObjectAsTrigger();

        private StateMachine<StateClass, TriggerClass> _stateMachine;

        // Public method to make the state machine do something
        public void DoSomething()
        {
            _stateMachine.Fire(_someTrigger);
        }

        // Public method to make the state machine do something else (sic).
        public void DoSomethingElse(int payload)
        {
            _anotherTrigger.Payload = payload;
            _stateMachine.Fire(_anotherTrigger);
        }

        // Use this to initialize the state machine to a know state
        // All objects needed for the states and transitions must be instantied before use. 
        internal void Initialize()
        {
            _stateMachine = new StateMachine<StateClass, TriggerClass>(_initialState);

            _stateMachine.Configure(_initialState)
                .Permit(_someTrigger, _someState);

            _stateMachine.Configure(_someState)
                .OnEntry(_someState.OnEntry)
                .Permit(_anotherTrigger, _anotherState);

            _stateMachine.Configure(_anotherState)
                .OnEntry((t) =>  _anotherState.OnEntry(t))
                .Permit(_someTrigger, _someState);
        }
    }

    public abstract class StateClass
    {
        protected readonly string _stateName;

        protected StateClass(string stateName)
        {
            _stateName = stateName;
        }
    }

    public class ObjectAsState : StateClass
    {

        public ObjectAsState(string stateName) : base(stateName) { }

        // This is a class that you can use as a state.
        // Use it as any other class.
        public void OnEntry(StateMachine<StateClass, TriggerClass>.Transition t)
        {
            Console.WriteLine($"State machine is now in {_stateName}");
        }
    }

    public class AnotherObjectAsState : StateClass
    {
        // This is a class that you can use as a state, if you so fancy.

        public AnotherObjectAsState(string stateName) : base(stateName) { }

        public void OnEntry(StateMachine<StateClass, TriggerClass>.Transition t)
        {
            if (t.Trigger is AnotherObjectAsTrigger trigger)
            {
                Console.WriteLine($"State machine is now in {_stateName}, and the trigger payload is {trigger.Payload}");
            }
        }
    }

    public abstract class TriggerClass    { }

    public class ObjectAsTrigger :  TriggerClass
    {
        // This is a class that you can use as a trigger, if you so fancy.
    }

    public class AnotherObjectAsTrigger : TriggerClass
    {
        // This is a class that you can use as a trigger, if you so fancy.
        // It can have payload, if needed
        public int Payload { get; internal set; }
    }
}
