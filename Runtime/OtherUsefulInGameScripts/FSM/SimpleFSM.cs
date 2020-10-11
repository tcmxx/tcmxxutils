using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public class SimpleFSM<TStateKey>
    {
        public ISimpleFSMState<TStateKey> InitialState { get; protected set; } = null;
        public ISimpleFSMState<TStateKey> CurrentState { get; protected set; } = null;

        protected Dictionary<TStateKey, ISimpleFSMState<TStateKey>> states = new Dictionary<TStateKey, ISimpleFSMState<TStateKey>>();

        public event Action<ISimpleFSMState<TStateKey>,ISimpleFSMState<TStateKey>> onStateChanged;

        public void SetInitialState(TStateKey key)
        {
            InitialState = states[key];
        }

        public void AddState(TStateKey key, Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> stateEnter,
            Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> stateExit,
            Action<SimpleFSM<TStateKey>, float> stateUpdate)
        {
            AddState(new SimpleFSMState<TStateKey>(key, stateEnter, stateExit, stateUpdate));
        }

        public void AddState(ISimpleFSMState<TStateKey> state)
        {
            Debug.Assert(!states.ContainsKey(state.Key), "State with key" + state.Key + " already exist while adding it!");
            states[state.Key] = state;
            if (InitialState == null)
                SetInitialState(state.Key);
        }

        public bool RemoveState(TStateKey stateKey)
        {
            return states.Remove(stateKey);
        }

        public void GoToState(TStateKey stateKey)
        {
            var newState = states[stateKey];
            var oldState = CurrentState;

            if (CurrentState != null)
                CurrentState.OnStatExit(this, newState);
            newState.OnStateEnter(this, oldState);

            CurrentState = newState;
            onStateChanged?.Invoke(oldState, newState);
        }

        public void Update(float deltaTime)
        {
            if (CurrentState == null)
            {
                CurrentState = InitialState;
                CurrentState.OnStateEnter(this, null);
            }
            if (CurrentState != null)
                CurrentState.Update(this, deltaTime);
        }

        public void Reset(bool callStateExit = false)
        {
            if (CurrentState == null)
                CurrentState.OnStatExit(this, null);
            CurrentState = InitialState;
        }
    }

    public interface ISimpleFSMState<TStateKey>
    {
        TStateKey Key { get; }
        void OnStateEnter(SimpleFSM<TStateKey> stateMachine, ISimpleFSMState<TStateKey> fromState);
        void OnStatExit(SimpleFSM<TStateKey> stateMachine, ISimpleFSMState<TStateKey> toState);
        void Update(SimpleFSM<TStateKey> stateMachine, float deltaTime);
    }

    public class SimpleFSMState<TStateKey> : ISimpleFSMState<TStateKey>
    {
        protected Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> enter;
        protected Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> exit;
        protected Action<SimpleFSM<TStateKey>, float> update;
        protected TStateKey key;

        public SimpleFSMState(TStateKey key,
            Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> stateEnter,
            Action<SimpleFSM<TStateKey>, ISimpleFSMState<TStateKey>> stateExit,
            Action<SimpleFSM<TStateKey>, float> stateUpdate)
        {
            enter = stateEnter;
            exit = stateExit;
            update = stateUpdate;
            this.key = key;
        }

        public TStateKey Key => key;

        public void OnStateEnter(SimpleFSM<TStateKey> stateMachine, ISimpleFSMState<TStateKey> fromState)
        {
            enter?.Invoke(stateMachine, fromState);
        }

        public void OnStatExit(SimpleFSM<TStateKey> stateMachine, ISimpleFSMState<TStateKey> toState)
        {
            exit?.Invoke(stateMachine, toState);
        }

        public void Update(SimpleFSM<TStateKey> stateMachine, float deltaTime)
        {
            update?.Invoke(stateMachine, deltaTime);
        }
    }
}