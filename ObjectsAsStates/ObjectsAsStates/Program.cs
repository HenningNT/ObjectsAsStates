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
            oas.DoSomethingElse();
            oas.DoSomething();

            Console.WriteLine("All done! Joy to the world!");
            Console.ReadKey();
        }
    }

    public class ObjectsAsStates
    {
        private readonly ObjectAsState _initialState = new ObjectAsState();
        private readonly ObjectAsState _someState = new ObjectAsState();
        private readonly ObjectAsState _anotherState = new ObjectAsState();

        private readonly ObjectAsTrigger _someTrigger = new ObjectAsTrigger();
        private readonly ObjectAsTrigger _anotherTrigger = new ObjectAsTrigger();

        private StateMachine<ObjectAsState, ObjectAsTrigger> _stateMachine;

        public void DoSomething()
        {
            _stateMachine.Fire(_someTrigger);
        }

        public void DoSomethingElse()
        {
            _stateMachine.Fire(_anotherTrigger);
        }

        internal void Initialize()
        {
            _stateMachine = new StateMachine<ObjectAsState, ObjectAsTrigger>(_initialState);

            _stateMachine.Configure(_initialState)
                .Permit(_someTrigger, _someState);

            _stateMachine.Configure(_someState)
                .OnEntry(() => Console.WriteLine("State machine is now in '_someState'"))
                .Permit(_anotherTrigger, _anotherState);

            _stateMachine.Configure(_anotherState)
                .OnEntry(() => Console.WriteLine("State machine is now in '_anotherState'"))
                .Permit(_someTrigger, _someState);
        }
    }

    public class ObjectAsState
    {
        // This is a class that you can use as a state, if you so fancy.
    }

    public class ObjectAsTrigger
    {
        // This is a class that you can use as a state, if you so fancy.
    }
}
