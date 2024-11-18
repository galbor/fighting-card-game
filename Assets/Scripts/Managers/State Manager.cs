using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Managers
{
    public class StateManager : Singleton<StateManager>
    {
        private enum State
        {
            PLAYING,
            DRAFTING,
            DECKVIEW,
            NONE
        }

        private Stack<State> _states = new Stack<State>();

        private void Awake()
        {
            _states.Push(State.PLAYING);
        }

        /**
         * returns state that fits the obj (if it's a manager with a state)
         */
        private State GetState(UnityEngine.Object obj)
        {
            if (obj == PlayerTurn.Instance) return State.PLAYING;
            if (obj == CardDraftManager.Instance) return State.DRAFTING;
            if (obj == DeckDisplayManager.Instance) return State.DECKVIEW;
            return State.NONE;
        } 
        
        /**
         * adds a state
         * @return True if the obj is a manager of a valid state
         */
        public bool AddState(UnityEngine.Object obj)
        {
            var state = GetState(obj);
            if (state == State.NONE) return false;
            OnStateExit(_states.Peek());
            _states.Push(state);
            OnStateEnter(state);

            return true;
        }

        /**
         * pops current state
         */
        public void RemoveState()
        {
            var state = _states.Pop();
            OnStateExit(state);
            OnStateEnter(_states.Peek());
        }

        private static void OnStateEnter(State state)
        {
            switch (state)
            {
                case State.PLAYING:
                    PlayerTurn.Instance.enabled = true;
                    PlayerTurn.Instance.ResetAction();
                    break;
                case State.DRAFTING:
                    CardDraftManager.Instance.enabled = true;
                    CardDraftManager.Instance.SetDraftActive(true);
                    break;
                case State.DECKVIEW:  //deckdisplaymanager should always be on
                default:
                    break;
            }
        }

        private static void OnStateExit(State state)
        {
            switch (state)
            {
                case State.PLAYING:
                    PlayerTurn.Instance.enabled = false;
                    PlayerTurn.Instance.StopAction();
                    break;
                case State.DRAFTING:
                    CardDraftManager.Instance.enabled = false;
                    CardDraftManager.Instance.SetDraftActive(false);
                    break;
                case State.DECKVIEW: //deckdisplaymanager should always be on
                default: 
                    break;
            }
        }
    }
}