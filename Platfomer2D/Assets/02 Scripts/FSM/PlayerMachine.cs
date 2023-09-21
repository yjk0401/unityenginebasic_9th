using UnityEngine;

public class PlayerMachine : CharacterMachine 
{
    public override float horizontal 
    {
        get => Input.GetAxisRaw("Horizontal");
        set => base.horizontal = value; 
    }

    public override float vertical 
    {
        get => Input.GetAxisRaw("Vertical");
        set => base.vertical = value; 
    }

    private void Start()
    {
        Initialize(CharacterStateWorkflowsDataSheet.GetWorkflowForPlayer(this));
        onHpDepleted += (amount) =>
        {
            if (hpValue > hpMin)
                ChangeState(State.Hurt);
        };
        onHpDepleted += (amount) =>
        {
            isInvincible = true;

            if (hpValue > hpMin)
                Invoke("ResetInvincible", 0.5f);
        };
        onHpMin += () => ChangeState(State.Die);
    }

    private void ResetInvincible() 
    {
        isInvincible = false;
    }

    protected override void Update() 
    {
        base.Update();
        if (Input.GetKey(KeyCode.C))
            if (current == State.Dash)
                ChangeState(State.Idle);


        if (Input.GetKeyDown(KeyCode.C))
        {
                if (ChangeState(State.JumpDown) == false)
                if (ChangeState(State.Jump) == false)
                    ChangeState(State.SecondJump);
        }

        if (Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeState(State.WallSlide);
        }
        else if (current == State.WallSlide) 
        {
            ChangeState(State.Idle);
        }

        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            if (canLaderrUp)
                ChangeState(State.LadderClimbing, new object[] { upLadder, DIRECTION_UP });
            ChangeState(State.Ledge);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeState(State.LedgeClimb);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (canLaderrDown && isGrounded && (upLadder != downLadder))
                ChangeState(State.LadderClimbing, new object[] { downLadder, DIRECTION_DOWN });
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            ChangeState(State.Crouch);
        }

        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (current == State.Crouch)
                ChangeState(State.Idle);
        }

        if (Input.GetKeyDown(KeyCode.X)) 
        {
            ChangeState(State.Attack);
        }

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            if (ChangeState(State.Slide)) { }
            else if (ChangeState(State.Dash)) { }
        }
    }
}