using UnityEngine;

public class PlayerMachine : CharacterMachine 
{
    public override float horizotal 
    {
        get => Input.GetAxisRaw("Horizontal");
        set => base.horizotal = value; 
    }

    private void Start()
    {
        Initialize(CharacterStateWorkflowsDataSheet.GetWorkflowForPlayer(this));
    }
}