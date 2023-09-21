using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public static class CharacterStateWorkflowsDataSheet 
{
    public abstract class WorkfolwVBase : IWorkflow<State>
    {
        public abstract State ID { get; }

        public int Current => current;

        public virtual bool CanExecute => true;

        protected int current;

        protected CharacterMachine machine;
        protected Transform transform;
        protected Rigidbody2D rigidbody;
        protected CapsuleCollider2D[] colliders;
        protected Animator animator;

        public WorkfolwVBase(CharacterMachine machine) 
        {
            this.machine = machine;
            this.transform = machine.transform;
            this.animator = machine.animator;
            this.rigidbody = transform.GetComponent<Rigidbody2D>();
            this.colliders = transform.GetComponentsInChildren<CapsuleCollider2D>();
        }

        public abstract State OnUpdate();
        public virtual void OnFixedUpdate() 
        {
        
        }


        public void Reset()
        {
            current = 0;
        }

        public virtual void OnEnter(object[] parameters) 
        {
            Reset();
        }

        public virtual void OnExit() 
        {
            
        }
    }

    #region States
    public class Idle : WorkfolwVBase
    {

        public override State ID => State.Idle;

        public Idle(CharacterMachine machine) : base(machine) 
        {
        
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            if (machine.isGrounded) 
            {
                machine.hasJumped = false;
                machine.hasSecondJumped = false;
            }
            machine.isDirectionChangeable = true;
            machine.isMovable = true;
            animator.Play("Idle");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (Mathf.Abs(machine.horizontal) > 0)
                            next = State.Move;
                        if (machine.isGrounded == false)
                            next = State.Fall;
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

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = true;
            animator.Play("Move");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (machine.horizontal == 0)
                            next = State.Idle;
                        if (machine.isGrounded == false)
                            next = State.Fall;
                    }
                    break;
            }

            return next;
        }
    }

    public class Jump : WorkfolwVBase
    {

        public override State ID => State.Jump;
        public override bool CanExecute => base.CanExecute &&
                                             machine.hasJumped == false &&
                                          (((machine.current == State.Idle || machine.current == State.Move) && machine.isGrounded) ||
                                             machine.current == State.LadderClimbing || 
                                             machine.current == State.Ledge ||
                                             machine.current == State.WallSlide);

        private float _force;

        public Jump(CharacterMachine machine, float force) : base(machine)
        {
            _force = force;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"> 0 : previous state </param>
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasJumped = true;
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            rigidbody.velocity = machine.previous == State.LadderClimbing || machine.previous == State.Ledge || machine.previous == State.WallSlide ?
                new Vector2(machine.horizontal * machine.speed, 0.0f) : new Vector2(rigidbody.velocity.x, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("Jump");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f) 
                        {
                            if (machine.isGrounded)
                                next = State.Idle;
                            else
                                next = State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class JumpDown : WorkfolwVBase
    {

        public override State ID => State.JumpDown;
        public override bool CanExecute => base.CanExecute &&
                                           machine.current == State.Crouch &&
                                           machine.isGroundExistBelow;

        private float _force;
        private float _groundIgnoreTime;
        private float _timeMark;
        private Collider2D _ground;

        public JumpDown(CharacterMachine machine, float force, float groundIgnoreTime) : base(machine)
        {
            _force = force;
            _groundIgnoreTime = groundIgnoreTime;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            _ground = machine.ground;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("Jump");
        }

        public override void OnExit()
        {
            base.OnExit();
            for (int i = 0; i < colliders.Length; i++)
            {
                Physics2D.IgnoreCollision(colliders[i], _ground, false);
            }
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                case 0: 
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            Physics2D.IgnoreCollision(colliders[i], _ground, true);
                        }
                        _timeMark = Time.time;
                        current++;
                    }
                    break;
                case 1:
                    {
                        if (rigidbody.velocity.y <= 0)
                        {
                            animator.Play("Fall");
                            current++;
                        }
                    }
                    break;
                case 2: 
                    {
                        if (Time.time - _timeMark > _groundIgnoreTime)
                        {
                            for (int i = 0; i < colliders.Length; i++)
                            {
                                Physics2D.IgnoreCollision(colliders[i], _ground, false);
                            }
                            current++;
                        }
                    }
                    break;

                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f)
                        {
                            if (machine.isGrounded)
                                next = State.Idle;
                            else
                                next = State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class SecondJump : WorkfolwVBase
    {

        public override State ID => State.SecondJump;
        public override bool CanExecute => base.CanExecute &&
                                            machine.hasSecondJumped == false &&
                                           (machine.current == State.Jump ||
                                            machine.current == State.Fall) &&
                                            machine.isGrounded == false;

        private float _force;

        public SecondJump(CharacterMachine machine, float force) : base(machine)
        {
            _force = force;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.hasSecondJumped = true;
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = new Vector2(machine.horizontal * machine.speed, 0.0f);
            rigidbody.AddForce(Vector2.up * _force, ForceMode2D.Impulse);
            animator.Play("SecondJump");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (rigidbody.velocity.y <= 0.0f)
                        {
                            if (machine.isGrounded)
                                next = State.Idle;
                            else
                                next = State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class Fall : WorkfolwVBase
    {

        public override State ID => State.Fall;
        private float _landingDistance;
        private float _startPosY;

        public Fall(CharacterMachine machine, float landingDistance) : base(machine)
        {
            _landingDistance = landingDistance;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            _startPosY = rigidbody.position.y;
            animator.Play("Fall");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (machine.isGrounded) 
                        {
                            next = (_startPosY - rigidbody.position.y) < _landingDistance ? State.Idle : State.Land;
                        }

                    }
                    break;
            }

            return next;
        }
    }

    public class Land : WorkfolwVBase
    {

        public override State ID => State.Land;

        public Land(CharacterMachine machine) : base(machine)
        {

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Land");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        // 현재 애니메이터의 재생중인 상태의 정보에서 일반화도니 시간이 1.0f 이 된다.
                        // == 현재 상태 애니메이션 클립 재생이 끝났다.
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) 
                        {
                            next = State.Idle;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class Crouch : WorkfolwVBase
    {

        public override State ID => State.Crouch;

        public override bool CanExecute => base.CanExecute &&
                                           (machine.current == State.Idle ||
                                            machine.current == State.Move) &&
                                           machine.isGrounded;

        private Vector2 _offsetCrouched;
        private Vector2 _sizeCrouched;
        private Vector2 _offsetOrigin;
        private Vector2 _sizeOrigin;

        public Crouch(CharacterMachine machine, Vector2 offsetCrouched, Vector2 sizeCrouched) : base(machine)
        {
            _offsetCrouched = offsetCrouched;
            _sizeCrouched = sizeCrouched;
            _offsetOrigin = colliders[0].offset;
            _sizeOrigin = colliders[0].size;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = true;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetCrouched;
                colliders[i].size = _sizeCrouched;
            }
            animator.Play("Crouch");
        }

        public override void OnExit()
        {
            base.OnExit();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetOrigin;
                colliders[i].size = _sizeOrigin;
            }
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                case 0:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                        {
                            animator.Play("CrouchIdle");
                            current++;
                        }
                    }
                    break;

                default: 
                    {
                        if (machine.isGrounded ==  false) 
                        {
                            next = State.Fall;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class LadderClimbing : WorkfolwVBase
    {

        public override State ID => State.LadderClimbing;

        public override bool CanExecute => base.CanExecute &&
                                            (machine.current == State.Idle ||
                                             machine.current == State.Move ||
                                             machine.current == State.Jump ||
                                             machine.current == State.Fall);

        private Ladder _ladder;
        private float _climbingSpeed;
        private float _vertical;

        public LadderClimbing(CharacterMachine machine, float climbingSpeed) : base(machine)
        {
            _climbingSpeed = climbingSpeed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"> 0 : (Ladder), 1 : 위/아래(int) </param>
        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            animator.Play("LadderClimbing");
            animator.speed = 0.0f;
            _ladder = (Ladder)parameters[0];
            int toward = (int)parameters[1];
            if (toward > 0)
                transform.position = transform.position.y > _ladder.upStartPos.y ?
                                         new Vector3(_ladder.upStartPos.x, transform.position.y) : _ladder.upStartPos;
            else if (toward < 0) 
            {
                transform.position = transform.position.y < _ladder.downStartPos.y ?
                                         new Vector3(_ladder.downStartPos.x, transform.position.y) : _ladder.downStartPos;
            }
            else
                throw new System.Exception($"[{machine.gameObject.name} - LadderClimbing] : toward wrong");
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            animator.speed = 1.0f;
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                case 0:
                    {
                        // nothing to do;
                    }
                    break;

                default:
                    {
                        if (machine.isGrounded)
                        {
                            next = State.Idle;
                        }
                        else if (transform.position.y > _ladder.upEndPos.y)
                        {
                            transform.position = _ladder.top;
                            next = State.Idle;
                        }
                        else if (transform.position.y < _ladder.downEndPos.y)
                        {
                            next = State.Idle;
                        }
                        else 
                        {
                            _vertical = machine.vertical;
                        }
                    }                  
                    break;
            }

            return next;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            switch (current)
            {
                case 0: 
                    {
                        if (machine.isGrounded == false)
                            current++;
                    }
                    break;

                default:
                    {
                        if (machine.isGrounded)
                        {
                        }
                        else if (transform.position.y > _ladder.upEndPos.y)
                        {
                        }
                        else if (transform.position.y < _ladder.downEndPos.y)
                        {
                        }
                        else
                        {
                            animator.speed = Mathf.Abs(_vertical);
                            transform.position += Vector3.up * machine.vertical * _climbingSpeed * Time.fixedDeltaTime;
                        }
                    }
                    break;
            }
        }
    }

    public class Ledge : WorkfolwVBase
    {

        public override State ID => State.Ledge;
        public override bool CanExecute => base.CanExecute &&
                                           machine.isLedgeDetected &&
                                          (machine.current == State.Jump ||
                                           machine.current == State.SecondJump ||
                                           machine.current == State.JumpDown ||
                                           machine.current == State.Fall ||
                                           machine.current == State.Move ||
                                           machine.current == State.Dash);

        public Ledge(CharacterMachine machine) : base(machine)
        {

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            transform.position = machine.ledgePoint - new Vector2(machine.ledgeDetectOffset.x * machine.direction,
                                                                  machine.ledgeDetectOffset.y);
            animator.Play("LedgeStart");
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                case 0: 
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            current++;

                    }
                    break;

                case 1: 
                    {
                        animator.Play("LedgeIdle");
                        current++;
                    }
                    break;

                default:
                    {

                    }
                    break;
            }

            return next;
        }
    }

    public class LedgeClimb : WorkfolwVBase
    {

        public override State ID => State.LedgeClimb;
        public override bool CanExecute => base.CanExecute &&
                                           machine.current == State.Ledge;

        private Vector2 _different;
        private Vector2 _differentRatio;
        private Vector2 _startPos;
        private float _clipLengh;
        private float _timer;

        public LedgeClimb(CharacterMachine machine) : base(machine)
        {

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            _different = machine.ledgePoint - (Vector2)transform.position;
            Vector2 distance = new Vector2(Mathf.Abs(_different.x), Mathf.Abs(_different.y));
            _differentRatio = new Vector2(distance.x / (distance.x + distance.y), distance.y / (distance.x + distance.y));
            _startPos = transform.position;
            animator.Play("LedgeClimb");
            _clipLengh = animator.GetCurrentAnimatorClipInfo(0).Length;
            _timer = 0.0f;
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                case 0:
                    {
                        transform.position =
                            Vector2.Lerp(_startPos, 
                                         _startPos + Vector2.up * _different.y,
                                         _timer / (_clipLengh * _differentRatio.x));

                        if (_timer >= _clipLengh * _differentRatio.y) 
                        {
                            _startPos = transform.position;
                            _timer = 0.0f;
                            current++;
                        }

                    }
                    break;

                case 1:
                    {
                        transform.position =
                            Vector2.Lerp(_startPos,
                                         _startPos + Vector2.right * _different.x,
                                         _timer / (_clipLengh * _differentRatio.y));

                        if (_timer >= _clipLengh * _differentRatio.x) 
                        {
                            current++;
                        }
                    }
                    break;

                default:
                    {
                        next = State.Idle;
                    }
                    break;
            }

            _timer += Time.deltaTime * 5.0f;
            return next;
        }
    }

    public class WallSlide : WorkfolwVBase
    {

        public override State ID => State.WallSlide;
        public override bool CanExecute => base.CanExecute &&
                                           machine.isWallDetected &&
                                           (machine.current == State.Fall);

        private float _dampingFactor;
        private Vector2 _velocity;

        public WallSlide(CharacterMachine machine, float dampingFactor) : base(machine)
        {
            _dampingFactor = dampingFactor;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.hasJumped = false;
            machine.hasSecondJumped = false;
            machine.move = Vector2.zero;
            _velocity = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            animator.Play("WallSlide");
        }

        public override void OnExit()
        {
            base.OnExit();
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (machine.isWallDetected == false) 
                        {
                            next = State.Idle;
                        }
                    }
                    break;
            }

            return next;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            _velocity += Physics2D.gravity * (1.0f - _dampingFactor) * Time.fixedDeltaTime;
            transform.position += (Vector3)_velocity * Time.fixedDeltaTime;
        }
    }

    public class Attack : WorkfolwVBase
    {
        public override State ID => State.Attack;

        public override bool CanExecute 
        {
            get 
            {
                if (_combo > 0 &&
                    Time.time - _exitTimeMark >= _comboResetTime) 
                {
                    _combo = 0;
                    _hasHit = false;
                }

                if (_combo > _comboMax) 
                {
                    return false;
                }

                if (base.CanExecute &&
                    ((_combo == 0) || (_combo > 0 && _hasHit)) &&
                    (machine.current == State.Idle ||
                     machine.current == State.Move ||
                     machine.current == State.Fall ||
                     machine.current == State.Jump ||
                     machine.current == State.SecondJump)) 
                {
                    return true;
                }

                return false;
            }
        }

        private int _comboMax;
        private int _combo;
        private float _comboResetTime;
        private float _exitTimeMark;
        private bool _hasHit;
        private AnimatorEvents _animatorEvents;

        public Attack(CharacterMachine machine, int comboMax, float comboResetTime) : base(machine)
        {
            _comboMax = comboMax;
            _comboResetTime = comboResetTime;
            _animatorEvents = machine.GetComponentInChildren<AnimatorEvents>();
            _animatorEvents.onAttackHit += () => _hasHit = true;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            if (machine.isGrounded) 
            {
                machine.move = Vector2.zero;
                rigidbody.velocity = Vector2.zero;
            }
            _hasHit = false;
            animator.SetFloat("attackComboStack", _combo++);
            animator.Play("Attack");
        }

        public override void OnExit()
        {
            base.OnExit();
            _exitTimeMark = Time.time;
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) 
                        {
                            next = State.Idle;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class Dash : WorkfolwVBase
    {

        public override State ID => State.Dash;
        public override bool CanExecute => base.CanExecute &&
                                           (machine.current == State.Idle || 
                                            machine.current == State.Move ||
                                            machine.current == State.Jump ||
                                            machine.current == State.SecondJump ||
                                            machine.current == State.Fall);

        private float _force;

        public Dash(CharacterMachine machine, float force) : base(machine)
        {
            _force = force;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            if (machine.direction > 0)
                machine.move = Vector2.right * _force * 1.5f;
            if (machine.direction < 0)
                machine.move = Vector2.left * _force * 1.5f;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Dash");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                            next = State.Idle;
                    }
                    break;
            }

            return next;
        }
    }

    public class Slide : WorkfolwVBase
    {

        public override State ID => State.Slide;
        public override bool CanExecute => base.CanExecute &&
                                           machine.current == State.Crouch;

        private float _force;

        private Vector2 _offsetCrouched;
        private Vector2 _sizeCrouched;
        private Vector2 _offsetOrigin;
        private Vector2 _sizeOrigin;

        public Slide(CharacterMachine machine, float force ,Vector2 offsetCrouched, Vector2 sizeCrouched) : base(machine)
        {
            _offsetCrouched = offsetCrouched;
            _sizeCrouched = sizeCrouched;
            _offsetOrigin = colliders[0].offset;
            _sizeOrigin = colliders[0].size;
            _force = force;
        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetCrouched;
                colliders[i].size = _sizeCrouched;
            }
            if (machine.direction > 0)
                machine.move = Vector2.right * _force * 1.5f;
            if (machine.direction < 0)
                machine.move = Vector2.left * _force * 1.5f;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Slide");
        }

        public override void OnExit()
        {
            base.OnExit();
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].offset = _offsetOrigin;
                colliders[i].size = _sizeOrigin;
            }
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) 
                        {
                            if (machine.vertical < 0)
                                next = State.Crouch;
                            else
                                next = State.Idle;
                        }
                    }
                    break;
            }

            return next;
        }
    }

    public class Hurt : WorkfolwVBase
    {
        public override State ID => State.Hurt;

        public override bool CanExecute => base.CanExecute &&
                                           (machine.current == State.Idle ||
                                            machine.current == State.Move ||
                                            machine.current == State.Jump ||
                                            machine.current == State.SecondJump ||
                                            machine.current == State.Fall ||
                                            machine.current == State.Crouch ||
                                            machine.current == State.JumpDown ||
                                            machine.current == State.Ledge ||
                                            machine.current == State.LedgeClimb ||
                                            machine.current == State.LadderClimbing ||
                                            machine.current == State.WallSlide ||
                                            machine.current == State.Land ||
                                            machine.current == State.Dash ||
                                            machine.current == State.Slide);

        public Hurt(CharacterMachine machine) : base(machine)
        {

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Hurt");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        next = State.Idle;
                    }
                    break;
            }

            return next;
        }
    }

    public class Die : WorkfolwVBase
    {

        public override State ID => State.Die;

        public Die(CharacterMachine machine) : base(machine)
        {

        }

        public override void OnEnter(object[] parameters)
        {
            base.OnEnter(parameters);
            machine.isInvincible = true;
            machine.isDirectionChangeable = false;
            machine.isMovable = false;
            machine.move = Vector2.zero;
            rigidbody.velocity = Vector2.zero;
            animator.Play("Die");
        }

        public override State OnUpdate()
        {
            State next = ID;

            switch (current)
            {
                default:
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 2.0f) 
                        {
                            GameObject.Destroy(machine.gameObject);
                        }
                    }
                    break;
            }

            return next;
        }
    }

    #endregion

    public static IEnumerable<KeyValuePair<State, IWorkflow<State>>> GetWorkflowForPlayer(CharacterMachine machine) 
    {
        return new Dictionary<State, IWorkflow<State>>()
        {
            { State.Idle, new Idle(machine) },
            { State.Move, new Move(machine) },
            { State.Jump, new Jump(machine, 3.5f) },
            { State.JumpDown, new JumpDown(machine, 1.0f, 0.5f) },
            { State.SecondJump, new SecondJump(machine, 3.5f) },
            { State.Fall, new Fall(machine, 1.0f) },
            { State.Land, new Land(machine) },
            { State.Crouch, new Crouch(machine, new Vector2(0.0f, 0.12f), new Vector2(0.12f, 0.24f)) },
            { State.LadderClimbing, new LadderClimbing(machine, 1.0f) },
            { State.Ledge, new Ledge(machine) },
            { State.LedgeClimb, new LedgeClimb(machine) },
            { State.WallSlide, new WallSlide(machine, 0.8f) },
            { State.Attack, new Attack(machine, 2, 0.2f) },
            { State.Dash, new Dash(machine, 1.5f)},
            { State.Slide, new Slide(machine, 1.5f, new Vector2(0.0f, 0.12f), new Vector2(0.12f, 0.24f)) },
            { State.Hurt, new Hurt(machine) },
            { State.Die, new Die(machine) },
        };
    }

    public static IEnumerable<KeyValuePair<State, IWorkflow<State>>> GetWorkflowsForEmemy(CharacterMachine machine)
    {
        return new Dictionary<State, IWorkflow<State>>()
        {
            { State.Idle, new Idle(machine) },
            { State.Move, new Move(machine) },
            { State.Jump, new Jump(machine, 3.5f) },
            { State.JumpDown, new JumpDown(machine, 1.0f, 0.5f) },
            { State.SecondJump, new SecondJump(machine, 3.5f) },
            { State.Fall, new Fall(machine, 1.0f) },
            { State.Land, new Land(machine) },
            { State.Crouch, new Crouch(machine, new Vector2(0.0f, 0.12f), new Vector2(0.12f, 0.24f)) },
            { State.LadderClimbing, new LadderClimbing(machine, 1.0f) },
            { State.Ledge, new Ledge(machine) },
            { State.LedgeClimb, new LedgeClimb(machine) },
            { State.WallSlide, new WallSlide(machine, 0.8f) },
            { State.Attack, new Attack(machine, 0, 0.0f) },
            { State.Dash, new Dash(machine, 0.0f)},
            { State.Hurt, new Hurt(machine) },
            { State.Die, new Die(machine) },
        };
    }
}