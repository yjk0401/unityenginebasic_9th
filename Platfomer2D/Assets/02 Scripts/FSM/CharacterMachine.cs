using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using System.Net.NetworkInformation;

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
    LadderClimbing,
    Ledge,
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
    public const int DIRECTION_UP = 1;
    public const int DIRECTION_DOWN = -1;


    // Movement
    public virtual float horizotal { get; set; }
    public virtual float vertical { get; set; }
    public float speed;
    [HideInInspector] public Vector2 move;
    [HideInInspector] public bool isMovable;
    private Rigidbody2D _rigidbody;

    public State current;
    public State previous;

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

    // Ladder detection
    public bool canLaderrUp { get; private set; }
    public bool canLaderrDown { get; private set; }
    public Ladder upLadder { get; private set; }
    public Ladder downLadder { get; private set; }
    [SerializeField] private float _LadderUpDetectOfset;
    [SerializeField] private float _LadderDownDetectOfset;
    [SerializeField] private float _LadderDetectRadius;
    [SerializeField] private LayerMask _ladderMask;

    // Ledge detection
    public bool isLedgeDetected;
    public Vector2 ledgePoint;
    public Vector2 ledgeDectectOffset;
    [SerializeField] private float _ledgeDetectRadius;
    public float ledgeDetectDistance;
    [SerializeField] private LayerMask _ledgeMask;

    public void Initialize(IEnumerable<KeyValuePair<State, IWorkflow<State>>> copy) 
    {
        _states = new Dictionary<State, IWorkflow<State>>(copy);
        current = copy.First().Key;
    }

    public bool ChangeState(State newState, object[] parameters = null) 
    {
        if (_isDirty)
            return false;

        if (newState == current)
            return false;

        if (_states[newState].CanExecute == false)
            return false;

        _states[current].OnExit();
        previous = current;
        _states[current].OnExit();
        current = newState;
        _states[current].OnEnter(parameters);
        _states[newState].Reset();
        ChangeState(_states[newState].OnUpdate());
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
        ChangeState(_states[current].OnUpdate());

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
        _states[current].OnFixedUpdate();
        _rigidbody.position += move * Time.fixedDeltaTime;
        DetectGround();
        DetectLadder();
        DetectLedge();
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

    private void DetectLadder()
    {
        Collider2D upCol =
        Physics2D.OverlapCircle(_rigidbody.position + Vector2.up * _LadderUpDetectOfset,
                                _LadderDetectRadius,
                                _ladderMask);
        upLadder = upCol ? upCol.GetComponent<Ladder>() : null;
        canLaderrUp = upLadder;

        Collider2D downCol =
        Physics2D.OverlapCircle(_rigidbody.position + Vector2.up * _LadderDownDetectOfset,
                        _LadderDetectRadius,
                        _ladderMask);
        downLadder = downCol ? downCol.GetComponent<Ladder>() : null;
        canLaderrDown = downLadder;
    }

    private void DetectLedge() 
    {
        RaycastHit2D hit =
            Physics2D.Raycast(_rigidbody.position + new Vector2(ledgeDectectOffset.x * direction, ledgeDectectOffset.y),
                              Vector2.down,
                              ledgeDetectDistance,
                              _ledgeMask);

        isLedgeDetected = hit.collider;
        ledgePoint = hit.point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)_groundDetectCenter,
                            _groundDetectSize);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + (Vector3)_groundBelowDetectCenter + Vector3.down * _groundBelowDetectDistance / 2.0f,
                            new Vector3(_groundDetectSize.x, _groundDetectSize.y + _groundBelowDetectDistance));

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _LadderUpDetectOfset, _LadderDetectRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _LadderDownDetectOfset, _LadderDetectRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + (Vector3)ledgeDectectOffset,
                        transform.position + (Vector3)ledgeDectectOffset + Vector3.down * ledgeDetectDistance);
    }
}