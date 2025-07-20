using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerControllerS player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        if (player.jumpBufferCounter > 0 && player.coyoteTimeCounter > 0)
        {
            player.animator.SetTrigger("jumpTrigger");
            player.velocity.y = Mathf.Sqrt(player.jumpHeight * -2f * player.normalGravity);
            player.jumpBufferCounter = 0;
            player.coyoteTimeCounter = 0;
        }
    }

    public override void HandleInput()
    {
        if (player.inputManager.GetDashInput() && !player.isDashing && player.dashCooldownTimer <= 0f)
        {
            stateMachine.ChangeState(new DashState(player, stateMachine));
        }

        if (player.inputManager.GetAttackInput())
        {
            stateMachine.ChangeState(new AttackState(player, stateMachine));
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
        player.HandleInputSmoothing();
        player.HandleMovement();

        if (player.isGrounded && player.velocity.y < 0)
        {
            stateMachine.ChangeState(new IdleState(player, stateMachine));
        }

        player.ApplyGravity();
        player.HandleRotation();

    }

    public override void PhysicsUpdate()
    {
    }
}
