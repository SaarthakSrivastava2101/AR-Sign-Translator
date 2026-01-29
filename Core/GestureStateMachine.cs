using System;

namespace ARSignTranslator.Core
{
    public class GestureStateMachine
    {
        public GestureState CurrentState { get; private set; } = GestureState.Idle;

        public event Action<GestureState, GestureState>? OnStateChanged;

        public void TransitionTo(GestureState newState)
        {
            if (newState == CurrentState) return;

            var old = CurrentState;
            CurrentState = newState;

            OnStateChanged?.Invoke(old, newState);
        }

        public void Reset()
        {
            TransitionTo(GestureState.Reset);
            TransitionTo(GestureState.Idle);
        }
    }
}
