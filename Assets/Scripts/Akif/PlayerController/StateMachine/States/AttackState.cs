using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f; 
    private float attackTimer = 0f;

    public AttackState(PlayerControllerS player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {  
        AudioManager.Instance.Play("PlayerAttack", true);
    }

    public override void Enter()
    {
        attackDuration = player.attackDuration;

        attackTimer = attackDuration;
        player.HandleAttack();
    }

    public override void LogicUpdate()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (player.currentSmoothedInput.magnitude > 0.1f)
                player.stateMachine.ChangeState(new MoveState(player, player.stateMachine));
            else
                player.stateMachine.ChangeState(new IdleState(player, player.stateMachine));
        }
    }
}
