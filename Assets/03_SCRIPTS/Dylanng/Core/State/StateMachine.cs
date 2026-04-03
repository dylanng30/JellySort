using Dylanng.Core.State;

namespace Dylanng.Core.State
{
    public class StateMachine
    {
        private StateBase _currentState;

        public void Initialize(StateBase startingState)
        {
            _currentState = startingState;
            _currentState?.Enter();
        }

        public void TransitionTo(StateBase nextState)
        {
            if (_currentState == nextState) return;

            _currentState?.Exit();
            _currentState = nextState;
            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }
    }
}
