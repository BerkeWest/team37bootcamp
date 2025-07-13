using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerControllerS player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.animator.SetFloat("speedFloat", 0);
    }

    public override void HandleInput()
    {
        Vector2 moveInput = player.inputManager.GetMoveInput();
        if (moveInput.sqrMagnitude > 0.1f)
        {
            stateMachine.ChangeState(new MoveState(player, stateMachine));
        }

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
