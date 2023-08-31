using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public static class CharacterStateWorkflowsDataSheet 
{
    public abstract class WorkfolwVBase : IWorkflow<State>
    {
        public abstract State ID { get; }

        public int Current => current;
        protected int current;

        protected CharacterMachine machine;
        protected Transform transform;
        protected Rigidbody2D rigidbody;
        protected CapsuleCollider2D collider;
        protected Animator animator;

        public WorkfolwVBase(CharacterMachine machine) 
        {
            this.machine = machine;
            this.transform = machine.transform;
            this.animator = machine.animator;
            this.rigidbody = transform.GetComponent<Rigidbody2D>();
            this.collider = transform.GetComponent<CapsuleCollider2D>();
        }

        public abstract State MoveNext();


        public void Reset()
        {
            current = 0;
        }
    }


    public class Idle : WorkfolwVBase
    {

        public override State ID => State.Idle;

        public Idle(CharacterMachine machine) : base(machine) 
        {
        
        }

        public override State MoveNext()
        {
            State next = ID;

            switch (current)
            {
                case 0: 
                    {
                        machine.isDirectionChangeable = true;
                        machine.isMovable = true;
                        animator.Play("Idle");
                        current++;
                    }
                    break;


                default:
                    {
                        if (Mathf.Abs(machine.horizotal) > 0)
                            next = State.Move;
                        // todo -> Ground 가 감지되지 않으면 next = State.Fall
                    }
                    break;
            }

            return next;
        }
    }

    public class Move : WorkfolwVBase
    {

        public override State ID => State.Move;

        public Move(CharacterMachine machine) : base(machine)
        {

        }

        public override State MoveNext()
        {
            State next = ID;

            switch (current)
            {
                case 0: 
                    {
                        machine.isDirectionChangeable = true;
                        machine.isMovable = true;
                        animator.Play("Move");
                        current++;
                    }
                    break;

                default:
                    {
                        if (machine.horizotal == 0)
                            next = State.Idle;
                        // todo -> Ground 가 감지되지 않으면 next = State.Fall
                    }
                    break;
            }

            return next;
        }
    }

    public class Jump : WorkfolwVBase
    {

        public override State ID => State.Jump;

        public Jump(CharacterMachine machine) : base(machine)
        {

        }

        public override State MoveNext()
        {
            State next = ID;

            switch (current)
            {
                case 0:
                    {
                        machine.isDirectionChangeable = true;
                        machine.isMovable = false;
                        animator.Play("Jump");
                        current++;
                    }
                    break;

                default:
                    {
                        if (machine.horizotal == 0)
                            next = State.Idle;
                        // todo -> Ground 가 감지되지 않으면 next = State.Fall
                    }
                    break;
            }

            return next;
        }
    }

    public static IEnumerable<KeyValuePair<State, IWorkflow<State>>> GetWorkflowForPlayer(CharacterMachine machine) 
    {
        return new Dictionary<State, IWorkflow<State>>()
        {
            { State.Idle, new Idle(machine) },
            { State.Move, new Move(machine) }
        };
    }
}