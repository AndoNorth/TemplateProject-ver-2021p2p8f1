using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// generic state machine class
public class StateMachine
{
    private IState _currentState;
    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>(0);
    // update state
    public void Tick()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            SetState(transition.To);
        }
        _currentState?.Tick();
    }
    // change state if state is different
    public void SetState(IState state)
    {
        if (state == _currentState)
        {
            return;
        }

        _currentState?.OnExit();
        _currentState = state;

        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
        {
            _currentTransitions = EmptyTransitions;
        }
        _currentState.OnEnter();
    }
    // add transition - transition to a state from a state
    // to: next state, from: previous state, predicate: condition
    public void addTransition(IState from, IState to, Func<bool> predicate)
    {
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }
        transitions.Add(new Transition(to, predicate));
    }
    // add any transition - transition to a state from any state ( similar to an interupt )
    // state: any state, predicate: condition
    public void addAnyTransition(IState state, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(state, predicate));
    }
    
    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition()
    {
        foreach(var transition in _anyTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }
        foreach(var transition in _currentTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }
        return null;
    }
}
// shortcut function for "add transition"
// void At(IState to, IState from, Func<bool> condition) => _stateMachine.addTransition(to, from, condition);