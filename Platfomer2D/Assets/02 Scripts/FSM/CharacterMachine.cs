using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public enum State 
{
    None,
    Idle,
    Move,
    Jump,
    JumpDown,
    SecondJump,
    Fall,
    Land,
    Crouch,
}

public class CharacterMachine : MonoBehaviour
{
    //Direcrion
    public int direction
    {
        get => _direction;

        set
        {
            if (isActiveAndEnabled == false)
                return;

            if (_direction == value)
                return;

            if (value > 0)
            {
                transform.eulerAngles = Vector3.up * 0.0f;
                _direction = DIRECTION_RIGHT;
            }


            else if (value < 0)
            {
                transform.eulerAngles = Vector3.up * 180.0f;
                _direction = DIRECTION_LEFT;
            }

            else
                throw new System.Exception("[CharacterMachine] : Invalid direction (0).");

        }
    }
    private int _direction;
    [HideInInspector] public bool isDirectionChangeable;
    public const int DIRECTION_RIGHT = 1;
    public const int DIRECTION_LEFT = -1;


    // Movement
    public virtual float horizotal { get; set; }
    public float speed;
    [HideInInspector] public Vector2 move;
    [HideInInspector] public bool isMovable;
    private Rigidbody2D _rigidbody;

    public State current;
    private bool _isDirty;
    private Dictionary<State, IWorkflow<State>> _states;

    public Animator animator;

    //Flags
    public bool hasJumped;
    public bool hasSecondJumped;

    public bool isGrounded { get; private set; }

    public bool isGroundExistBelow { get; private set; }
    public Collider2D ground { get; private set; }
    [Header("Ground Detection")]
    [SerializeField] private Vector2 _groundDetectCenter;
    [SerializeField] private Vector2 _groundDetectSize;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Vector2 _groundBelowDetectCenter;
    [SerializeField] private float _groundBelowDetectDistance;

    public void Initialize(IEnumerable<KeyValuePair<State, IWorkflow<State>>> copy) 
    {
        _states = new Dictionary<State, IWorkflow<State>>(copy);
        current = copy.First().Key;
    }

    public bool ChangeState(State newState) 
    {
        if (_isDirty)
            return false;

        if (newState == current)
            return false;

        if (_states[newState].CanExecute == false)
            return false;

        _states[current].OnExit();
        current = newState;
        _states[current].OnEnter();
        _states[newState].Reset();
        ChangeState(_states[newState].MoveNext());
        _isDirty = true;
        return true;
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _direction = DIRECTION_RIGHT;
    }

    protected virtual void Update()
    {
        ChangeState(_states[current].MoveNext());

        if (isMovable) 
        {
            move = new Vector2(horizotal * speed, 0.0f);
        }

        if (isDirectionChangeable &&
            Mathf.Abs(horizotal) > 0.0f)
        {
            direction = horizotal < 0.0f ? DIRECTION_LEFT : DIRECTION_RIGHT;
        }
    }

    private void FixedUpdate()
    {
        _rigidbody.position += move * Time.fixedDeltaTime;
        DetectGround();
    }


    private void LateUpdate()
    {
        _isDirty = false;
    }

    private void DetectGround() 
    {
        ground = Physics2D.OverlapBox(_rigidbody.position + _groundDetectCenter,
                             _groundDetectSize,
                             0.0f,
                             _groundMask);

        isGrounded = ground;

        if (isGrounded)
        {
            RaycastHit2D hit =
                    Physics2D.BoxCast(origin: _rigidbody.position + _groundBelowDetectCenter,
                                      size: _groundDetectSize,
                                      angle: 0.0f,
                                      direction: Vector2.down,
                                      distance: _groundBelowDetectDistance,
                                      layerMask: _groundMask);
            isGroundExistBelow = hit.collider;
        }
        else 
        {
            isGroundExistBelow = false;
        }

             


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)_groundDetectCenter,
                            _groundDetectSize);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + (Vector3)_groundBelowDetectCenter + Vector3.down * _groundBelowDetectDistance / 2.0f,
                            new Vector3(_groundDetectSize.x, _groundDetectSize.y + _groundBelowDetectDistance));
    }
}