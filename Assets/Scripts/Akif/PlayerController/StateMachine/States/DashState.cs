using UnityEngine;

public class DashState : PlayerState
{
    public DashState(PlayerControllerS player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.isDashing = true;
        player.dashTimer = player.dashDuration;
        player.dashCooldownTimer = player.dashCooldown;

        Vector3 moveInput = new Vector3(player.currentSmoothedInput.x, 0, player.currentSmoothedInput.y);
        if (moveInput.magnitude > 0.1f)
            player.dashDirection = moveInput.normalized;
        else
            player.dashDirection = player.transform.forward;

        player.velocity.y = 0;

        player.animator.SetTrigger("dashTrigger");
        player.animator.SetBool("isDashingBool", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.dashTimer -= Time.deltaTime;

        float dashProgress = 1f - (player.dashTimer / player.dashDuration);
        float curveValue = player.dashSpeedCurve.Evaluate(dashProgress);
        Vector3 dashMove = player.dashDirection * player.dashSpeed * curveValue;

        player.controller.Move(dashMove * Time.deltaTime);

        if (player.dashTimer <= 0f)
        {
            player.isDashing = false;
            player.animator.SetBool("isDashingBool", false);

            if (player.isGrounded)
                stateMachine.ChangeState(new IdleState(player, stateMachine));
            else
                stateMachine.ChangeState(new JumpState(player, stateMachine));
        }
        player.HandleRotation();

    }

    public override void PhysicsUpdate()
    {
    }
}
