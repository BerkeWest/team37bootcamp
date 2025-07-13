using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerControllerS player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void HandleInput()
    {
        if (player.inputManager.GetDashInput() && !player.isDashing && player.dashCooldownTimer <= 0f)
        {
            stateMachine.ChangeState(new DashState(player, stateMachine));
        }

        if (player.inputManager.GetJumpInput())
        {
            player.jumpBufferCounter = player.jumpBufferTime;
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.GroundCheck();
        player.HandleInputSmoothing();
        player.HandleMovement();
        player.CheckJumpInputBuffer();

        if (player.jumpBufferCounter > 0f && player.coyoteTimeCounter > 0f)
        {
            stateMachine.ChangeState(new JumpState(player, stateMachine));
        }

        player.ApplyGravity();
        player.HandleRotation();

    }

    public override void PhysicsUpdate()
    {
    }
}
