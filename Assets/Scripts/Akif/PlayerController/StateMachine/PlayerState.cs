using UnityEngine;

public abstract class PlayerState
{
    protected PlayerControllerS player;
    protected PlayerStateMachine stateMachine;

    public PlayerState(PlayerControllerS player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void HandleInput() { }

    public virtual void LogicUpdate() 
    {
        player.dashCooldownTimer -= Time.deltaTime;
    }

    public virtual void PhysicsUpdate() { }
}
