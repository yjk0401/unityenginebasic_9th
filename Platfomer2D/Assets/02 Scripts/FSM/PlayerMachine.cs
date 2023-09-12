using UnityEngine;

public class PlayerMachine : CharacterMachine 
{
    public override float horizotal 
    {
        get => Input.GetAxisRaw("Horizontal");
        set => base.horizotal = value; 
    }

    public override float vertical 
    {
        get => Input.GetAxisRaw("Vertical");
        set => base.vertical = value; 
    }

    private void Start()
    {
        Initialize(CharacterStateWorkflowsDataSheet.GetWorkflowForPlayer(this));
    }

    protected override void Update() 
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (ChangeState(State.JumpDown) == false)
            if (ChangeState(State.Jump) == false)
                ChangeState(State.SecondJump);
        }

        if (Input.GetKey(KeyCode.UpArrow)) 
        {
            if (canLaderrUp)
                ChangeState(State.LadderClimbing, new object[] { upLadder, DIRECTION_UP });
            ChangeState(State.Ledge);
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
    }
}