using UnityEngine;

public class AttackState : State
{
    private float attackTimer;

    public AttackState(EnemyAI enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.agent.ResetPath();
        attackTimer = 0f;
    }

    public override void Update()
    {
        // 공격 범위에서 벗어나면 다시 추적
        if (!enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        // 플레이어 방향 바라보기
        Vector3 direction =
            enemy.player.position - enemy.transform.position;

        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            enemy.transform.rotation =
                Quaternion.LookRotation(direction);
        }

        // 공격 쿨다운
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            enemy.Attack();
            attackTimer = enemy.attackCooldown;
        }
    }

    public override void Exit()
    {
    }
}