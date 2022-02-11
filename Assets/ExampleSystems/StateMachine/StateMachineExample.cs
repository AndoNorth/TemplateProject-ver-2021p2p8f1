using UnityEngine;
using System;

namespace TemplateProject {
    public class StateMachineExample : MonoBehaviour
    {
        // statemachine
        StateMachine _stateMachine;
        private void Awake()
        {
            _stateMachine = new StateMachine();

            IdleState idle = new IdleState();
            AttackState attack = new AttackState();
            BlockState block = new BlockState();

            At(idle, attack, isAttacking());
            At(idle, block, isBlocking());

            _stateMachine.addAnyTransition(idle, isIdle());

            // expression body methods:
            // Add Transition function
            // to: next state, from: previous state, condition
            void At(IState to, IState from, Func<bool> condition) => _stateMachine.addTransition(to, from, condition);

            // func bool methods
            Func<bool> isIdle() => () => !Input.anyKey;
            // Func<bool> isIdle() => () => Input.GetKey("z");
            Func<bool> isAttacking() => () => Input.GetKey("x");
            Func<bool> isBlocking() => () => Input.GetKey("c");

            // TODO: add generic Func<bool> which compares elapsed time vs last state change time
            // TODO: add interupt when "character/NPC" has been hit
        }
        private void Update()
        {
            _stateMachine.Tick();
        }
    }

    // states
    class IdleState : IState
    {
        string stateName = "Idle State";
        public IdleState() // can recieve inputs
        {
        }
        public void Tick()
        {

        }
        public void OnEnter()
        {
            Debug.Log("Entered " + stateName);
        }
        public void OnExit()
        {
            Debug.Log("Exiting " + stateName);
        }
    }
    class AttackState : IState
    {
        string stateName = "Attack State";
        public AttackState() // can recieve inputs
        {
        }
        public void Tick()
        {

        }
        public void OnEnter()
        {
            Debug.Log("Entered " + stateName);
        }
        public void OnExit()
        {
            Debug.Log("Exiting " + stateName);
        }
    }
    class BlockState : IState
    {
        string stateName = "Block State";
        public BlockState() // can recieve inputs
        {
        }
        public void Tick()
        {

        }
        public void OnEnter()
        {
            Debug.Log("Entered " + stateName);
        }
        public void OnExit()
        {
            Debug.Log("Exiting " + stateName);
        }
    }
}

